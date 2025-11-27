using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Profile;
using OdiNow.Contracts.Responses.Profile;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProfileService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var response = _mapper.Map<ProfileResponse>(user);
        response.Addresses = user.Addresses.Select(_mapper.Map<UserAddressResponse>).ToList();
        response.FavoritesCount = await _dbContext.Favorites.CountAsync(f => f.UserId == userId, cancellationToken);
        return response;
    }

    public async Task<ProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.ProfilePhotoUrl = request.ProfilePhotoUrl;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetProfileAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<UserAddressResponse>> GetAddressesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var addresses = await _dbContext.UserAddresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Label)
            .ToListAsync(cancellationToken);

        return addresses.Select(_mapper.Map<UserAddressResponse>).ToList();
    }

    public async Task<UserAddressResponse> AddAddressAsync(Guid userId, CreateAddressRequest request, CancellationToken cancellationToken = default)
    {
        if (request.IsDefault)
        {
            await UnsetDefaultAddresses(userId, cancellationToken);
        }

        var address = new UserAddress
        {
            UserId = userId,
            Label = request.Label,
            Line1 = request.Line1,
            Line2 = request.Line2,
            City = request.City,
            District = request.District,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = request.IsDefault
        };

        await _dbContext.UserAddresses.AddAsync(address, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserAddressResponse>(address);
    }

    public async Task<UserAddressResponse?> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request, CancellationToken cancellationToken = default)
    {
        var address = await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId, cancellationToken);
        if (address is null)
        {
            return null;
        }

        if (request.IsDefault)
        {
            await UnsetDefaultAddresses(userId, cancellationToken, address.Id);
        }

        address.Label = request.Label;
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.City = request.City;
        address.District = request.District;
        address.PostalCode = request.PostalCode;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.IsDefault = request.IsDefault;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserAddressResponse>(address);
    }

    public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        var address = await _dbContext.UserAddresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId, cancellationToken);
        if (address is null)
        {
            return false;
        }

        _dbContext.UserAddresses.Remove(address);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<FavoriteResponse>> GetFavoritesAsync(Guid userId, FavoriteType? type, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Favorites.AsNoTracking().Where(f => f.UserId == userId);
        if (type.HasValue)
        {
            query = query.Where(f => f.FavoriteType == type);
        }

        var favorites = await query.OrderByDescending(f => f.CreatedAt).ToListAsync(cancellationToken);
        return favorites.Select(_mapper.Map<FavoriteResponse>).ToList();
    }

    public async Task<FavoriteResponse> AddFavoriteAsync(Guid userId, CreateFavoriteRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.Favorites.AnyAsync(f =>
            f.UserId == userId &&
            f.FavoriteType == request.FavoriteType &&
            f.TargetId == request.TargetId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Already in favorites.");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            FavoriteType = request.FavoriteType,
            TargetId = request.TargetId
        };

        await _dbContext.Favorites.AddAsync(favorite, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FavoriteResponse>(favorite);
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid favoriteId, CancellationToken cancellationToken = default)
    {
        var favorite = await _dbContext.Favorites.FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId, cancellationToken);
        if (favorite is null)
        {
            return false;
        }

        _dbContext.Favorites.Remove(favorite);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<SearchHistoryResponse>> GetSearchHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var history = await _dbContext.SearchHistories
            .AsNoTracking()
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        return history.Select(_mapper.Map<SearchHistoryResponse>).ToList();
    }

    public async Task ClearSearchHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var history = await _dbContext.SearchHistories.Where(sh => sh.UserId == userId).ToListAsync(cancellationToken);
        _dbContext.SearchHistories.RemoveRange(history);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<CouponViewResponse>> GetViewedCouponsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var coupons = await _dbContext.CouponViews
            .AsNoTracking()
            .Where(cv => cv.UserId == userId)
            .OrderByDescending(cv => cv.ViewedAt)
            .ToListAsync(cancellationToken);

        return coupons.Select(_mapper.Map<CouponViewResponse>).ToList();
    }

    private async Task UnsetDefaultAddresses(Guid userId, CancellationToken cancellationToken, Guid? excludeId = null)
    {
        var defaults = await _dbContext.UserAddresses
            .Where(a => a.UserId == userId && a.IsDefault && (!excludeId.HasValue || a.Id != excludeId.Value))
            .ToListAsync(cancellationToken);

        foreach (var address in defaults)
        {
            address.IsDefault = false;
        }
    }
}

