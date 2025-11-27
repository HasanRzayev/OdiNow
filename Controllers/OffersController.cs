using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Offers;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;
    private readonly IUserContextAccessor _userContextAccessor;

    public OffersController(IOfferService offerService, IUserContextAccessor userContextAccessor)
    {
        _offerService = offerService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetOffers([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var offers = await _offerService.GetOffersAsync(includeInactive, cancellationToken);
        return Ok(offers);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOffer(Guid id, CancellationToken cancellationToken)
    {
        var offer = await _offerService.GetOfferAsync(id, cancellationToken);
        return offer is not null ? Ok(offer) : NotFound();
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


