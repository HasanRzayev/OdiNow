using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _catalogService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("restaurants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRestaurants(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? search,
        [FromQuery] bool onlyDiscounted,
        [FromQuery] double? lat,
        [FromQuery] double? lng,
        [FromQuery] double? radiusMeters,
        CancellationToken cancellationToken)
    {
        var restaurants = await _catalogService.GetRestaurantsAsync(
            categoryId,
            search,
            onlyDiscounted,
            lat,
            lng,
            radiusMeters,
            cancellationToken);
        return Ok(restaurants);
    }

    [HttpGet("restaurants/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRestaurant(Guid id, CancellationToken cancellationToken)
    {
        var restaurant = await _catalogService.GetRestaurantDetailsAsync(id, cancellationToken);
        return restaurant is not null ? Ok(restaurant) : NotFound();
    }

    [HttpGet("restaurants/{id:guid}/menu-items")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRestaurantMenu(Guid id, CancellationToken cancellationToken)
    {
        var menuItems = await _catalogService.GetMenuItemsAsync(id, cancellationToken);
        return Ok(menuItems);
    }

    [HttpGet("offers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOffers([FromQuery] bool personalizedOnly = false, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var offers = await _catalogService.GetActiveOffersAsync(personalizedOnly, Math.Clamp(limit, 1, 100), cancellationToken);
        return Ok(offers);
    }
}


