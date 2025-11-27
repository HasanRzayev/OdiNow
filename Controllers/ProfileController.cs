using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Profile;
using OdiNow.Models;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IUserContextAccessor _userContextAccessor;

    public ProfileController(IProfileService profileService, IUserContextAccessor userContextAccessor)
    {
        _profileService = profileService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var profile = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        return Ok(profile);
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var addresses = await _profileService.GetAddressesAsync(userId, cancellationToken);
        return Ok(addresses);
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var address = await _profileService.AddAddressAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, address);
    }

    [HttpPut("addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var address = await _profileService.UpdateAddressAsync(userId, addressId, request, cancellationToken);
        return address is not null ? Ok(address) : NotFound();
    }

    [HttpDelete("addresses/{addressId:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid addressId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var deleted = await _profileService.DeleteAddressAsync(userId, addressId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites([FromQuery] FavoriteType? type, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var favorites = await _profileService.GetFavoritesAsync(userId, type, cancellationToken);
        return Ok(favorites);
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] CreateFavoriteRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var favorite = await _profileService.AddFavoriteAsync(userId, request, cancellationToken);
        return Ok(favorite);
    }

    [HttpDelete("favorites/{favoriteId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid favoriteId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var removed = await _profileService.RemoveFavoriteAsync(userId, favoriteId, cancellationToken);
        return removed ? NoContent() : NotFound();
    }

    [HttpGet("search-history")]
    public async Task<IActionResult> GetSearchHistory(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var history = await _profileService.GetSearchHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    [HttpDelete("search-history")]
    public async Task<IActionResult> ClearSearchHistory(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        await _profileService.ClearSearchHistoryAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpGet("coupons")]
    public async Task<IActionResult> GetViewedCoupons(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var coupons = await _profileService.GetViewedCouponsAsync(userId, cancellationToken);
        return Ok(coupons);
    }
}


