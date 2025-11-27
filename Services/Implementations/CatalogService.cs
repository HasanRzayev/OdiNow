using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Responses.Catalog;
using OdiNow.Data;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CatalogService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ProjectTo<CategoryResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RestaurantSummaryResponse>> GetRestaurantsAsync(
        Guid? categoryId,
        string? search,
        bool onlyDiscounted,
        double? latitude,
        double? longitude,
        double? radiusMeters,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var restaurantsQuery = _dbContext.Restaurants
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (categoryId.HasValue)
        {
            restaurantsQuery = restaurantsQuery.Where(r => r.MenuItems.Any(mi => mi.CategoryId == categoryId));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            restaurantsQuery = restaurantsQuery.Where(r => r.Name.Contains(term));
        }

        if (onlyDiscounted)
        {
            restaurantsQuery = restaurantsQuery.Where(r => r.Offers.Any(o => o.IsActive && o.StartAt <= now && o.EndAt >= now));
        }

        var restaurants = await restaurantsQuery
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.AverageRating,
                r.TotalReviews,
                r.City,
                r.District,
                r.Latitude,
                r.Longitude,
                HasActiveOffer = r.Offers.Any(o => o.IsActive && o.StartAt <= now && o.EndAt >= now),
                EstimatedDeliveryMinutes = r.MenuItems.Any()
                    ? (int)r.MenuItems.Average(mi => mi.PreparationTimeMinutes)
                    : 30
            })
            .ToListAsync(cancellationToken);

        var maxRadius = radiusMeters.HasValue ? Math.Clamp(radiusMeters.Value, 100, 50000) : (double?)null;

        var projected = restaurants
            .Select(r =>
            {
                double? distance = null;
                if (latitude.HasValue && longitude.HasValue && r.Latitude.HasValue && r.Longitude.HasValue)
                {
                    distance = HaversineDistance(r.Latitude.Value, r.Longitude.Value, latitude.Value, longitude.Value);
                }

                return new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.AverageRating,
                    r.TotalReviews,
                    r.City,
                    r.District,
                    r.HasActiveOffer,
                    r.EstimatedDeliveryMinutes,
                    DistanceMeters = distance
                };
            });

        if (maxRadius.HasValue && latitude.HasValue && longitude.HasValue)
        {
            projected = projected.Where(p => p.DistanceMeters.HasValue && p.DistanceMeters.Value <= maxRadius.Value);
        }

        return projected
            .OrderByDescending(p => p.HasActiveOffer)
            .ThenBy(p => p.DistanceMeters ?? double.MaxValue)
            .ThenByDescending(p => p.AverageRating)
            .Select(p => new RestaurantSummaryResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AverageRating = p.AverageRating,
                TotalReviews = p.TotalReviews,
                City = p.City,
                District = p.District,
                HasActiveOffer = p.HasActiveOffer,
                EstimatedDeliveryMinutes = p.EstimatedDeliveryMinutes,
                DistanceMeters = p.DistanceMeters
            })
            .ToList();
    }

    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Earth radius in meters
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreesToRadians(double degrees)
        => degrees * (Math.PI / 180);

    public async Task<RestaurantDetailResponse?> GetRestaurantDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var restaurant = await _dbContext.Restaurants
            .AsNoTracking()
            .Include(r => r.Attributes)
            .Include(r => r.MenuItems)
            .ThenInclude(mi => mi.Category)
            .Include(r => r.Offers)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

        if (restaurant is null)
        {
            return null;
        }

        var response = _mapper.Map<RestaurantDetailResponse>(restaurant);
        response.MenuItems = restaurant.MenuItems
            .Where(mi => mi.IsAvailable)
            .OrderBy(mi => mi.Title)
            .Select(mi => new MenuItemResponse
            {
                Id = mi.Id,
                Title = mi.Title,
                Description = mi.Description,
                BasePrice = mi.BasePrice,
                IsAvailable = mi.IsAvailable,
                PreparationTimeMinutes = mi.PreparationTimeMinutes,
                ImageUrl = mi.ImageUrl,
                CategoryName = mi.Category.Name
            })
            .ToList();

        response.ActiveOffers = restaurant.Offers
            .Where(o => o.IsActive && o.StartAt <= now && o.EndAt >= now)
            .OrderByDescending(o => o.DiscountPercent)
            .Select(o => new OfferResponse
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                DiscountPercent = o.DiscountPercent,
                StartAt = o.StartAt,
                EndAt = o.EndAt,
                IsPersonalized = o.IsPersonalized,
                RestaurantId = o.RestaurantId,
                RestaurantName = restaurant.Name,
                MenuItemId = o.MenuItemId,
                MenuItemTitle = o.MenuItem?.Title
            })
            .ToList();

        return response;
    }

    public async Task<IEnumerable<MenuItemResponse>> GetMenuItemsAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MenuItems
            .AsNoTracking()
            .Where(mi => mi.RestaurantId == restaurantId && mi.IsAvailable)
            .OrderBy(mi => mi.Title)
            .Select(mi => new MenuItemResponse
            {
                Id = mi.Id,
                Title = mi.Title,
                Description = mi.Description,
                BasePrice = mi.BasePrice,
                IsAvailable = mi.IsAvailable,
                PreparationTimeMinutes = mi.PreparationTimeMinutes,
                ImageUrl = mi.ImageUrl,
                CategoryName = mi.Category.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OfferResponse>> GetActiveOffersAsync(bool personalizedOnly, int limit, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var query = _dbContext.Offers
            .AsNoTracking()
            .Where(o => o.IsActive && o.StartAt <= now && o.EndAt >= now);

        if (personalizedOnly)
        {
            query = query.Where(o => o.IsPersonalized);
        }

        var offers = await query
            .OrderByDescending(o => o.DiscountPercent)
            .ThenBy(o => o.EndAt)
            .Take(limit)
            .Select(o => new OfferResponse
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                DiscountPercent = o.DiscountPercent,
                StartAt = o.StartAt,
                EndAt = o.EndAt,
                IsPersonalized = o.IsPersonalized,
                RestaurantId = o.RestaurantId,
                RestaurantName = o.Restaurant != null ? o.Restaurant.Name : null,
                MenuItemId = o.MenuItemId,
                MenuItemTitle = o.MenuItem != null ? o.MenuItem.Title : null
            })
            .ToListAsync(cancellationToken);

        return offers;
    }
}

