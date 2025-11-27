using OdiNow.Contracts.Requests.Profile;
using OdiNow.Contracts.Responses.Profile;
using OdiNow.Models;

namespace OdiNow.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserAddressResponse>> GetAddressesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserAddressResponse> AddAddressAsync(Guid userId, CreateAddressRequest request, CancellationToken cancellationToken = default);
    Task<UserAddressResponse?> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);

    Task<IEnumerable<FavoriteResponse>> GetFavoritesAsync(Guid userId, FavoriteType? type, CancellationToken cancellationToken = default);
    Task<FavoriteResponse> AddFavoriteAsync(Guid userId, CreateFavoriteRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoveFavoriteAsync(Guid userId, Guid favoriteId, CancellationToken cancellationToken = default);

    Task<IEnumerable<SearchHistoryResponse>> GetSearchHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ClearSearchHistoryAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<CouponViewResponse>> GetViewedCouponsAsync(Guid userId, CancellationToken cancellationToken = default);
}


