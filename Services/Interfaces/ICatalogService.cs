using OdiNow.Contracts.Responses.Catalog;

namespace OdiNow.Services.Interfaces;

public interface ICatalogService
{
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RestaurantSummaryResponse>> GetRestaurantsAsync(Guid? categoryId, string? search, bool onlyDiscounted, CancellationToken cancellationToken = default);
    Task<RestaurantDetailResponse?> GetRestaurantDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OfferResponse>> GetActiveOffersAsync(bool personalizedOnly, int limit, CancellationToken cancellationToken = default);
}


