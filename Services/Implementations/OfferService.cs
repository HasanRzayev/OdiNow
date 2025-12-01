using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Offers;
using OdiNow.Contracts.Responses.Offers;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class OfferService : IOfferService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public OfferService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OfferDetailResponse>> GetOffersAsync(bool includeInactive, double? latitude = null, double? longitude = null, double? radiusMeters = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbContext.Offers
                .AsNoTracking()
                .Include(o => o.Restaurant)
                .Include(o => o.MenuItem)
                .Where(o => o.Restaurant != null && !o.Restaurant.IsDeleted)
                .AsQueryable();
            
            if (!includeInactive)
            {
                var now = DateTimeOffset.UtcNow;
                query = query.Where(o => o.IsActive && o.StartAt <= now && o.EndAt >= now);
            }

            var offers = await query.OrderByDescending(o => o.StartAt).ToListAsync(cancellationToken);
            var mapped = offers.Select(MapOffer).Where(o => o != null).ToList();

        if (latitude.HasValue && longitude.HasValue && radiusMeters.HasValue)
        {
            var maxRadius = Math.Clamp(radiusMeters.Value, 100, 50000);
            
            // Filter offers by distance
            var filtered = mapped
                .Where(o =>
                {
                    if (o.RestaurantLatitude.HasValue && o.RestaurantLongitude.HasValue)
                    {
                        var distance = HaversineDistance(
                            latitude.Value, longitude.Value,
                            o.RestaurantLatitude.Value, o.RestaurantLongitude.Value);
                        return distance <= maxRadius;
                    }
                    // If restaurant has no coordinates, exclude it from nearby results
                    return false;
                })
                .OrderBy(o =>
                {
                    if (o.RestaurantLatitude.HasValue && o.RestaurantLongitude.HasValue)
                    {
                        return HaversineDistance(
                            latitude.Value, longitude.Value,
                            o.RestaurantLatitude.Value, o.RestaurantLongitude.Value);
                    }
                    return double.MaxValue;
                })
                .ToList();
            
            // Return filtered results only if we have GPS coordinates
            return filtered;
        }

            return mapped;
        }
        catch (Exception ex)
        {
            // Log error here if you have logging
            throw new InvalidOperationException($"Error retrieving offers: {ex.Message}", ex);
        }
    }

    public async Task<OfferDetailResponse?> GetOfferAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offer = await _dbContext.Offers
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return offer is null ? null : MapOffer(offer);
    }

    public async Task<OfferDetailResponse> CreateOfferAsync(CreateOfferRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateOfferReferencesAsync(request.RestaurantId, request.MenuItemId, cancellationToken);

        var offer = new Offer
        {
            Title = request.Title,
            Description = request.Description,
            DiscountPercent = request.DiscountPercent,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            RestaurantId = request.RestaurantId,
            MenuItemId = request.MenuItemId,
            IsPersonalized = request.IsPersonalized,
            IsActive = true
        };

        await _dbContext.Offers.AddAsync(offer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(offer).Reference(o => o.Restaurant).LoadAsync(cancellationToken);
        await _dbContext.Entry(offer).Reference(o => o.MenuItem).LoadAsync(cancellationToken);

        return MapOffer(offer);
    }

    public async Task<OfferDetailResponse?> UpdateOfferAsync(Guid id, UpdateOfferRequest request, CancellationToken cancellationToken = default)
    {
        var offer = await _dbContext.Offers.Include(o => o.Restaurant).Include(o => o.MenuItem).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (offer is null)
        {
            return null;
        }

        offer.Title = request.Title;
        offer.Description = request.Description;
        offer.DiscountPercent = request.DiscountPercent;
        offer.StartAt = request.StartAt;
        offer.EndAt = request.EndAt;
        offer.IsPersonalized = request.IsPersonalized;
        offer.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapOffer(offer);
    }

    public async Task<bool> DeleteOfferAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offer = await _dbContext.Offers.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (offer is null)
        {
            return false;
        }

        _dbContext.Offers.Remove(offer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<CouponResponse>> GetUserCouponsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var coupons = await _dbContext.CouponViews
            .AsNoTracking()
            .Where(cv => cv.UserId == userId)
            .OrderByDescending(cv => cv.ViewedAt)
            .Include(cv => cv.Offer)
            .ToListAsync(cancellationToken);

        return coupons.Select(cv => new CouponResponse
        {
            Id = cv.Id,
            OfferId = cv.OfferId,
            Title = cv.Offer.Title,
            DiscountPercent = cv.Offer.DiscountPercent,
            ViewedAt = cv.ViewedAt
        }).ToList();
    }

    public async Task<bool> TrackCouponViewAsync(Guid userId, Guid offerId, CancellationToken cancellationToken = default)
    {
        var offerExists = await _dbContext.Offers.AnyAsync(o => o.Id == offerId, cancellationToken);
        if (!offerExists)
        {
            return false;
        }

        var exists = await _dbContext.CouponViews.AnyAsync(cv => cv.UserId == userId && cv.OfferId == offerId, cancellationToken);
        if (exists)
        {
            return true;
        }

        var view = new CouponView
        {
            UserId = userId,
            OfferId = offerId
        };

        await _dbContext.CouponViews.AddAsync(view, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static OfferDetailResponse? MapOffer(Offer offer)
    {
        if (offer == null)
        {
            return null;
        }

        return new OfferDetailResponse
        {
            Id = offer.Id,
            Title = offer.Title,
            Description = offer.Description,
            DiscountPercent = offer.DiscountPercent,
            StartAt = offer.StartAt,
            EndAt = offer.EndAt,
            IsPersonalized = offer.IsPersonalized,
            IsActive = offer.IsActive,
            RestaurantId = offer.RestaurantId,
            RestaurantName = offer.Restaurant?.Name,
            RestaurantLatitude = offer.Restaurant?.Latitude,
            RestaurantLongitude = offer.Restaurant?.Longitude,
            RestaurantAddress = offer.Restaurant != null ? $"{offer.Restaurant.AddressLine}, {offer.Restaurant.District}, {offer.Restaurant.City}" : null,
            MenuItemId = offer.MenuItemId,
            MenuItemTitle = offer.MenuItem?.Title
        };
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

    private async Task ValidateOfferReferencesAsync(Guid? restaurantId, Guid? menuItemId, CancellationToken cancellationToken)
    {
        if (restaurantId.HasValue)
        {
            var exists = await _dbContext.Restaurants.AnyAsync(r => r.Id == restaurantId.Value, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException("Restaurant not found.");
            }
        }

        if (menuItemId.HasValue)
        {
            var exists = await _dbContext.MenuItems.AnyAsync(mi => mi.Id == menuItemId.Value, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException("Menu item not found.");
            }
        }
    }
}

