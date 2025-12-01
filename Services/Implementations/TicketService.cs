using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OdiNow.Contracts.Responses.Tickets;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Options;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TicketOptions _options;
    private readonly ILogger<TicketService> _logger;

    public TicketService(ApplicationDbContext dbContext, IOptions<TicketOptions> options, ILogger<TicketService> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TicketSummaryResponse> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        _logger.LogInformation("GetSummaryAsync called: UserId={UserId}", userId);
        
        // Get count BEFORE ensuring tickets
        var countBeforeEnsure = await _dbContext.UserTickets
            .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
        _logger.LogInformation("Available tickets BEFORE EnsureTickets in GetSummary: {Count} for UserId={UserId}", countBeforeEnsure, userId);
        
        var latestTicket = await EnsureTicketsAsync(userId, now, cancellationToken);
        
        // Get count AFTER ensuring tickets
        var countAfterEnsure = await _dbContext.UserTickets
            .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
        _logger.LogInformation("Available tickets AFTER EnsureTickets in GetSummary: {Count} for UserId={UserId} (was {Before})", countAfterEnsure, userId, countBeforeEnsure);

        var availableTickets = await _dbContext.UserTickets
            .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
        
        _logger.LogInformation("GetSummaryAsync final count: {Count} for UserId={UserId}", availableTickets, userId);

        DateTimeOffset? nextTicketAt = latestTicket?.GeneratedAt.AddMinutes(_options.GenerationIntervalMinutes) ?? now;

        if (availableTickets >= _options.MaxActiveTickets)
        {
            nextTicketAt = null;
        }

        var historyEntities = await _dbContext.UserTickets
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Status == TicketStatus.Used && t.OfferId != null)
            .Include(t => t.Offer)
            .ThenInclude(o => o.Restaurant)
            .OrderByDescending(t => t.UsedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        var history = historyEntities
            .Select(t => new TicketHistoryItemResponse
            {
                TicketId = t.Id,
                OfferId = t.OfferId ?? Guid.Empty,
                OfferTitle = t.Offer != null ? t.Offer.Title : "Təklif silinib",
                RestaurantName = t.Offer != null
                    ? (t.Offer.Restaurant != null ? t.Offer.Restaurant.Name : null)
                    : null,
                DiscountPercent = t.Offer != null ? t.Offer.DiscountPercent : 0,
                UsedAt = t.UsedAt ?? t.GeneratedAt
            })
            .ToList();

        return new TicketSummaryResponse
        {
            AvailableTickets = availableTickets,
            MaxTickets = _options.MaxActiveTickets,
            NextTicketAt = nextTicketAt,
            History = history
        };
    }

    public async Task<bool> ConsumeTicketForOfferAsync(Guid userId, Guid offerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsumeTicketForOfferAsync called: UserId={UserId}, OfferId={OfferId}", userId, offerId);
        
        var offerExists = await _dbContext.Offers.AnyAsync(o => o.Id == offerId, cancellationToken);
        if (!offerExists)
        {
            _logger.LogWarning("Offer not found: OfferId={OfferId}", offerId);
            return false;
        }

        // Check if user already viewed this offer - if yes, don't consume ticket
        var couponViewCount = await _dbContext.CouponViews
            .CountAsync(cv => cv.UserId == userId, cancellationToken);
        var alreadyViewed = await _dbContext.CouponViews
            .AnyAsync(cv => cv.UserId == userId && cv.OfferId == offerId, cancellationToken);

        _logger.LogInformation("🔵 [Service] Already viewed check: UserId={UserId}, OfferId={OfferId}, AlreadyViewed={AlreadyViewed}, TotalCouponViews={Total}", 
            userId, offerId, alreadyViewed, couponViewCount);

        if (alreadyViewed)
        {
            var existingCouponView = await _dbContext.CouponViews
                .Where(cv => cv.UserId == userId && cv.OfferId == offerId)
                .FirstOrDefaultAsync(cancellationToken);
            _logger.LogInformation("🔵 [Service] Offer already viewed by user: UserId={UserId}, OfferId={OfferId}, ViewedAt={ViewedAt} - No ticket consumed", 
                userId, offerId, existingCouponView?.ViewedAt);
            // User already viewed this offer, don't consume ticket
            // Return false to indicate ticket was NOT consumed (but offer can still be viewed)
            return false;
        }
        
        _logger.LogInformation("🔵 [Service] Offer NOT viewed yet, proceeding with ticket consumption");

        // Use a transaction to prevent race conditions
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var now = DateTimeOffset.UtcNow;
            
            // Get available ticket count BEFORE consumption (do NOT call EnsureTicketsAsync here)
            // We only want to consume existing tickets, not create new ones
            var availableBefore = await _dbContext.UserTickets
                .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
            
            _logger.LogInformation("🔵 [Service] Available tickets before consumption: {Count} for UserId={UserId}", availableBefore, userId);

            var ticket = await _dbContext.UserTickets
                .Where(t => t.UserId == userId && t.Status == TicketStatus.Available)
                .OrderBy(t => t.GeneratedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (ticket is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning("No available tickets for user: UserId={UserId}", userId);
                // No tickets available
                return false;
            }

            _logger.LogInformation("Consuming ticket: TicketId={TicketId} for OfferId={OfferId}", ticket.Id, offerId);

            // Consume the ticket
            ticket.Status = TicketStatus.Used;
            ticket.OfferId = offerId;
            ticket.UsedAt = now;

            // Track that user viewed this offer
            var couponView = new CouponView
            {
                UserId = userId,
                OfferId = offerId,
                ViewedAt = now
            };
            await _dbContext.CouponViews.AddAsync(couponView, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Ticket saved to database: TicketId={TicketId}, Status={Status}, OfferId={OfferId}", ticket.Id, ticket.Status, offerId);
            
            // Commit transaction
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transaction committed successfully");

            // Verify ticket was actually consumed - reload from database to ensure it was saved
            var savedTicket = await _dbContext.UserTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == ticket.Id, cancellationToken);

            if (savedTicket == null || savedTicket.Status != TicketStatus.Used)
            {
                _logger.LogError("Ticket was not saved correctly: TicketId={TicketId}, Status={Status}", ticket.Id, savedTicket?.Status);
                return false;
            }

            // Verify ticket count decreased
            var availableAfter = await _dbContext.UserTickets
                .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);

            _logger.LogInformation("Available tickets after consumption: {Count} for UserId={UserId} (was {Before})", availableAfter, userId, availableBefore);

            // Ticket should have decreased by 1
            if (availableAfter != availableBefore - 1)
            {
                _logger.LogError("Ticket consumption verification failed: Expected {Expected}, got {Actual}. TicketId={TicketId}, Status={Status}", 
                    availableBefore - 1, availableAfter, ticket.Id, savedTicket.Status);
                // Something went wrong, but ticket was marked as used, so return true anyway
                // The count might be off due to concurrent requests or other issues
                return true; // Changed from false to true - ticket was consumed even if count is off
            }

            _logger.LogInformation("✅ Ticket successfully consumed: UserId={UserId}, OfferId={OfferId}, Tickets remaining={Remaining}", userId, offerId, availableAfter);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Unexpected error in ConsumeTicketForOfferAsync: UserId={UserId}, OfferId={OfferId}", userId, offerId);
            return false;
        }
    }

    private async Task<UserTicket?> EnsureTicketsAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        _logger.LogInformation("EnsureTicketsAsync called: UserId={UserId}", userId);
        
        var latestTicket = await _dbContext.UserTickets
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.GeneratedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var availableCount = await _dbContext.UserTickets
            .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);

        _logger.LogInformation("EnsureTicketsAsync: AvailableCount={Count}, MaxTickets={Max}, LatestTicket={Latest}", 
            availableCount, _options.MaxActiveTickets, latestTicket?.Id);

        var createdAny = false;
        if (latestTicket is null)
        {
            _logger.LogInformation("EnsureTicketsAsync: No tickets exist, creating {Count} initial tickets", _options.MaxActiveTickets);
            for (int i = 0; i < _options.MaxActiveTickets; i++)
            {
                var ticket = new UserTicket
                {
                    UserId = userId,
                    GeneratedAt = now.AddMinutes(-_options.GenerationIntervalMinutes * (_options.MaxActiveTickets - i))
                };
                await _dbContext.UserTickets.AddAsync(ticket, cancellationToken);
                latestTicket = ticket;
            }
            createdAny = true;
        }
        else
        {
            var nextGenerationAt = latestTicket.GeneratedAt.AddMinutes(_options.GenerationIntervalMinutes);
            _logger.LogInformation("EnsureTicketsAsync: Latest ticket GeneratedAt={GeneratedAt}, NextGenerationAt={Next}, Now={Now}", 
                latestTicket.GeneratedAt, nextGenerationAt, now);
            
            var ticketsCreated = 0;
            // Re-check availableCount from database in each iteration to avoid creating too many tickets
            while (nextGenerationAt <= now)
            {
                // Re-check available count from database to get accurate count
                var currentAvailableCount = await _dbContext.UserTickets
                    .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
                
                if (currentAvailableCount >= _options.MaxActiveTickets)
                {
                    _logger.LogInformation("EnsureTicketsAsync: Max tickets reached ({Count}), stopping ticket generation", currentAvailableCount);
                    break;
                }
                
                var ticket = new UserTicket
                {
                    UserId = userId,
                    GeneratedAt = nextGenerationAt
                };
                await _dbContext.UserTickets.AddAsync(ticket, cancellationToken);
                latestTicket = ticket;
                nextGenerationAt = nextGenerationAt.AddMinutes(_options.GenerationIntervalMinutes);
                createdAny = true;
                ticketsCreated++;
                
                // Save after each ticket to ensure accurate count on next iteration
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            
            if (ticketsCreated > 0)
            {
                var finalAvailableCount = await _dbContext.UserTickets
                    .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
                _logger.LogInformation("EnsureTicketsAsync: Created {Count} new tickets. AvailableCount now: {Available}", ticketsCreated, finalAvailableCount);
            }
            else
            {
                var finalAvailableCount = await _dbContext.UserTickets
                    .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
                _logger.LogInformation("EnsureTicketsAsync: No new tickets created. AvailableCount: {Available}, MaxTickets: {Max}", 
                    finalAvailableCount, _options.MaxActiveTickets);
            }
        }

        if (createdAny)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return latestTicket;
    }
}


