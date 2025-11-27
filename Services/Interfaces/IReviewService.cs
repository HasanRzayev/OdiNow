using OdiNow.Contracts.Requests.Reviews;
using OdiNow.Contracts.Responses.Reviews;

namespace OdiNow.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponse>> GetRestaurantReviewsAsync(Guid restaurantId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewResponse?> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken cancellationToken = default);
    Task<RestaurantRatingResponse> RecalculateRestaurantRatingAsync(Guid restaurantId, CancellationToken cancellationToken = default);
}


