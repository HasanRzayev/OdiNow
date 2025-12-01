using OdiNow.Contracts.Requests.Offers;
using OdiNow.Contracts.Responses.Offers;

namespace OdiNow.Services.Interfaces;

public interface IOfferService
{
    Task<IEnumerable<OfferDetailResponse>> GetOffersAsync(bool includeInactive, double? latitude = null, double? longitude = null, double? radiusMeters = null, CancellationToken cancellationToken = default);
    Task<OfferDetailResponse?> GetOfferAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OfferDetailResponse> CreateOfferAsync(CreateOfferRequest request, CancellationToken cancellationToken = default);
    Task<OfferDetailResponse?> UpdateOfferAsync(Guid id, UpdateOfferRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteOfferAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CouponResponse>> GetUserCouponsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> TrackCouponViewAsync(Guid userId, Guid offerId, CancellationToken cancellationToken = default);
}


