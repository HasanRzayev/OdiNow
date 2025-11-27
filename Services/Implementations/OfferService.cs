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

    public async Task<IEnumerable<OfferDetailResponse>> GetOffersAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Offers
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Include(o => o.MenuItem)
            .AsQueryable();
        if (!includeInactive)
        {
            var now = DateTimeOffset.UtcNow;
            query = query.Where(o => o.IsActive && o.StartAt <= now && o.EndAt >= now);
        }

        var offers = await query.OrderByDescending(o => o.StartAt).ToListAsync(cancellationToken);
        return offers.Select(MapOffer).ToList();
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

    private static OfferDetailResponse MapOffer(Offer offer)
    {
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
            MenuItemId = offer.MenuItemId,
            MenuItemTitle = offer.MenuItem?.Title
        };
    }

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

