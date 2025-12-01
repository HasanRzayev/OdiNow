using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OdiNow.Contracts.Requests.Offers;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ITicketService _ticketService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OffersController> _logger;

    public OffersController(IOfferService offerService, IUserContextAccessor userContextAccessor, ITicketService ticketService, ApplicationDbContext dbContext, ILogger<OffersController> logger)
    {
        _offerService = offerService;
        _userContextAccessor = userContextAccessor;
        _ticketService = ticketService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetOffers(
        [FromQuery] bool includeInactive = false,
        [FromQuery] double? lat = null,
        [FromQuery] double? lng = null,
        [FromQuery] double? radiusMeters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var offers = await _offerService.GetOffersAsync(includeInactive, lat, lng, radiusMeters, cancellationToken);
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetOffer(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        _logger.LogInformation("GetOffer called: UserId={UserId}, OfferId={OfferId}", userId, id);
        
        var offer = await _offerService.GetOfferAsync(id, cancellationToken);
        if (offer is null)
        {
            _logger.LogWarning("Offer not found: OfferId={OfferId}", id);
            return NotFound();
        }

        // Check if already viewed before trying to consume
        var couponViewCount = await _dbContext.CouponViews
            .CountAsync(cv => cv.UserId == userId, cancellationToken);
        var alreadyViewed = await _dbContext.CouponViews
            .AnyAsync(cv => cv.UserId == userId && cv.OfferId == id, cancellationToken);
        
        _logger.LogInformation("🔵 [Controller] Offer already viewed check: UserId={UserId}, OfferId={OfferId}, AlreadyViewed={AlreadyViewed}, TotalCouponViews={Total}", 
            userId, id, alreadyViewed, couponViewCount);
        
        if (alreadyViewed)
        {
            var couponView = await _dbContext.CouponViews
                .Where(cv => cv.UserId == userId && cv.OfferId == id)
                .FirstOrDefaultAsync(cancellationToken);
            _logger.LogInformation("🔵 [Controller] CouponView found: Id={Id}, ViewedAt={ViewedAt}", 
                couponView?.Id, couponView?.ViewedAt);
        }
        
        bool ticketConsumed = false;
        
        if (!alreadyViewed)
        {
            // Try to consume ticket - service will handle the consumption
            var consumed = await _ticketService.ConsumeTicketForOfferAsync(userId, id, cancellationToken);
            
            _logger.LogInformation("Ticket consumption result: UserId={UserId}, OfferId={OfferId}, Consumed={Consumed}", userId, id, consumed);
            
            if (!consumed)
            {
                // Check if it's because no tickets available
                var availableTickets = await _dbContext.UserTickets
                    .CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Available, cancellationToken);
                
                _logger.LogInformation("Available tickets after failed consumption: {Count} for UserId={UserId}", availableTickets, userId);
                
                // If no tickets available, return error
                if (availableTickets == 0)
                {
                    _logger.LogWarning("No tickets available for user: UserId={UserId}", userId);
                    return BadRequest(new { error = "Ticket qalmayıb. 30 dəqiqə sonra yenidən cəhd edin." });
                }
                
                // If tickets available but consumption failed, something went wrong
                // Still allow viewing the offer
                ticketConsumed = false;
            }
            else
            {
                // Ticket was successfully consumed
                ticketConsumed = true;
                _logger.LogInformation("✅ Ticket successfully consumed: UserId={UserId}, OfferId={OfferId}", userId, id);
            }
        }
        else
        {
            // Already viewed, ticket not consumed
            ticketConsumed = false;
            _logger.LogInformation("⚠️ Offer already viewed, no ticket consumed: UserId={UserId}, OfferId={OfferId}", userId, id);
        }

        // Return offer with ticket consumption info
        // Always allow viewing offer, even if ticket wasn't consumed
        _logger.LogInformation("Returning offer: UserId={UserId}, OfferId={OfferId}, TicketConsumed={TicketConsumed}", userId, id, ticketConsumed);
        return Ok(new { 
            offer = offer,
            ticketConsumed = ticketConsumed
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOffer([FromBody] CreateOfferRequest request, CancellationToken cancellationToken)
    {
        var offer = await _offerService.CreateOfferAsync(request, cancellationToken);
        return Ok(offer);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateOffer(Guid id, [FromBody] UpdateOfferRequest request, CancellationToken cancellationToken)
    {
        var offer = await _offerService.UpdateOfferAsync(id, request, cancellationToken);
        return offer is not null ? Ok(offer) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteOffer(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _offerService.DeleteOfferAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("coupons")]
    [Authorize]
    public async Task<IActionResult> GetCoupons(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var coupons = await _offerService.GetUserCouponsAsync(userId, cancellationToken);
        return Ok(coupons);
    }

    [HttpPost("{id:guid}/track")]
    [Authorize]
    public async Task<IActionResult> TrackCoupon(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var result = await _offerService.TrackCouponViewAsync(userId, id, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}


