using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Tickets;
using OdiNow.Contracts.Responses.Tickets;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _dbContext;

    public TicketService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TicketDropResponse>> GetActiveDropsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await ExpireStaleDropsAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var drops = await _dbContext.TicketDrops
            .AsNoTracking()
            .Include(td => td.Offer)
            .ThenInclude(o => o.Restaurant)
            .Where(td => td.IsActive && td.TicketsRemaining > 0 && td.AvailableFrom <= now && td.ExpiresAt >= now)
            .OrderBy(td => td.AvailableFrom)
            .ToListAsync(cancellationToken);

        return drops.Select(MapDrop).ToList();
    }

    public async Task<TicketClaimResponse> ClaimTicketAsync(Guid userId, Guid dropId, CancellationToken cancellationToken = default)
    {
        await ExpireStaleDropsAsync(cancellationToken);

        var drop = await _dbContext.TicketDrops
            .Include(td => td.Offer)
            .ThenInclude(o => o.Restaurant)
            .FirstOrDefaultAsync(td => td.Id == dropId, cancellationToken);

        if (drop is null)
        {
            throw new InvalidOperationException("Ticket drop not found.");
        }

        var now = DateTimeOffset.UtcNow;
        if (!drop.IsActive || drop.TicketsRemaining <= 0 || drop.AvailableFrom > now || drop.ExpiresAt < now)
        {
            throw new InvalidOperationException("Ticket drop is no longer available.");
        }

        var alreadyClaimed = await _dbContext.TicketClaims
            .AnyAsync(tc => tc.TicketDropId == dropId && tc.UserId == userId, cancellationToken);

        if (alreadyClaimed)
        {
            throw new InvalidOperationException("You have already claimed this ticket.");
        }

        var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
        var payload = $"TKT-{code}-{drop.Id.ToString("N")[..8]}";

        drop.TicketsRemaining--;
        if (drop.TicketsRemaining <= 0)
        {
            drop.IsActive = false;
        }

        var claim = new TicketClaim
        {
            TicketDropId = drop.Id,
            UserId = userId,
            Code = code,
            QrPayload = payload,
            Status = TicketClaimStatus.Claimed
        };

        await _dbContext.TicketClaims.AddAsync(claim, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapClaim(claim, drop);
    }

    public async Task<IEnumerable<TicketClaimResponse>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await ExpireStaleDropsAsync(cancellationToken);

        var claims = await _dbContext.TicketClaims
            .AsNoTracking()
            .Include(tc => tc.TicketDrop)
            .ThenInclude(td => td.Offer)
            .ThenInclude(o => o.Restaurant)
            .Where(tc => tc.UserId == userId)
            .OrderByDescending(tc => tc.ClaimedAt)
            .ToListAsync(cancellationToken);

        return claims.Select(claim => MapClaim(claim, claim.TicketDrop)).ToList();
    }

    public async Task<bool> RedeemTicketAsync(Guid userId, Guid claimId, RedeemTicketRequest request, CancellationToken cancellationToken = default)
    {
        var claim = await _dbContext.TicketClaims
            .Include(tc => tc.TicketDrop)
            .ThenInclude(td => td.Offer)
            .ThenInclude(o => o.Restaurant)
            .FirstOrDefaultAsync(tc => tc.Id == claimId && tc.UserId == userId, cancellationToken);

        if (claim is null)
        {
            return false;
        }

        if (claim.Status != TicketClaimStatus.Claimed)
        {
            return false;
        }

        if (!string.Equals(claim.QrPayload, request.QrPayload, StringComparison.Ordinal))
        {
            return false;
        }

        claim.Status = TicketClaimStatus.Redeemed;
        claim.RedeemedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ExpireStaleDropsAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var expiredDrops = await _dbContext.TicketDrops
            .Where(td => td.IsActive && td.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expiredDrops.Count > 0)
        {
            foreach (var drop in expiredDrops)
            {
                drop.IsActive = false;
                drop.TicketsRemaining = 0;
            }
        }

        var expiredClaims = await _dbContext.TicketClaims
            .Include(tc => tc.TicketDrop)
            .Where(tc => tc.Status == TicketClaimStatus.Claimed && tc.TicketDrop.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expiredClaims.Count > 0)
        {
            foreach (var claim in expiredClaims)
            {
                claim.Status = TicketClaimStatus.Expired;
                claim.ExpiredAt = now;
            }
        }

        if (expiredDrops.Count > 0 || expiredClaims.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static TicketDropResponse MapDrop(TicketDrop drop)
    {
        return new TicketDropResponse
        {
            Id = drop.Id,
            OfferId = drop.OfferId,
            Title = drop.Offer.Title,
            Description = drop.Offer.Description ?? string.Empty,
            DiscountPercent = drop.Offer.DiscountPercent,
            RestaurantName = drop.Offer.Restaurant?.Name ?? string.Empty,
            AvailableFrom = drop.AvailableFrom,
            ExpiresAt = drop.ExpiresAt,
            TicketsTotal = drop.TicketsTotal,
            TicketsRemaining = drop.TicketsRemaining
        };
    }

    private static TicketClaimResponse MapClaim(TicketClaim claim, TicketDrop drop)
    {
        return new TicketClaimResponse
        {
            Id = claim.Id,
            TicketDropId = claim.TicketDropId,
            OfferId = drop.OfferId,
            Title = drop.Offer.Title,
            Description = drop.Offer.Description ?? string.Empty,
            DiscountPercent = drop.Offer.DiscountPercent,
            RestaurantName = drop.Offer.Restaurant?.Name ?? string.Empty,
            Status = claim.Status,
            ClaimedAt = claim.ClaimedAt,
            RedeemedAt = claim.RedeemedAt,
            ExpiredAt = claim.ExpiredAt,
            QrPayload = claim.Status == TicketClaimStatus.Claimed ? claim.QrPayload : null
        };
    }
}


