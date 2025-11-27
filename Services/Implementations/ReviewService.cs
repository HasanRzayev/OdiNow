using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Reviews;
using OdiNow.Contracts.Responses.Reviews;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReviewService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewResponse>> GetRestaurantReviewsAsync(Guid restaurantId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var reviews = await _dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.RestaurantId == restaurantId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return reviews.Select(MapReview).ToList();
    }

    public async Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRestaurantExistsAsync(request.RestaurantId, cancellationToken);

        if (request.OrderId.HasValue)
        {
            var orderExists = await _dbContext.Orders.AnyAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken);
            if (!orderExists)
            {
                throw new InvalidOperationException("Order not found for user.");
            }
        }

        var hasReviewed = await _dbContext.Reviews.AnyAsync(r =>
            r.UserId == userId && r.RestaurantId == request.RestaurantId && r.OrderId == request.OrderId, cancellationToken);

        if (hasReviewed)
        {
            throw new InvalidOperationException("Review already exists for this order.");
        }

        var review = new Review
        {
            UserId = userId,
            RestaurantId = request.RestaurantId,
            OrderId = request.OrderId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsAnonymous = request.IsAnonymous
        };

        await _dbContext.Reviews.AddAsync(review, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await UpdateRestaurantRatingAsync(request.RestaurantId, cancellationToken);

        await _dbContext.Entry(review).Reference(r => r.User).LoadAsync(cancellationToken);
        return MapReview(review);
    }

    public async Task<ReviewResponse?> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId, cancellationToken);
        if (review is null)
        {
            return null;
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment;
        review.IsAnonymous = request.IsAnonymous;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await UpdateRestaurantRatingAsync(review.RestaurantId, cancellationToken);
        return MapReview(review);
    }

    public async Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId, cancellationToken);
        if (review is null)
        {
            return false;
        }

        _dbContext.Reviews.Remove(review);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await UpdateRestaurantRatingAsync(review.RestaurantId, cancellationToken);
        return true;
    }

    public async Task<RestaurantRatingResponse> RecalculateRestaurantRatingAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await UpdateRestaurantRatingAsync(restaurantId, cancellationToken);
    }

    private async Task<RestaurantRatingResponse> UpdateRestaurantRatingAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        var stats = await _dbContext.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .GroupBy(r => r.RestaurantId)
            .Select(g => new
            {
                RestaurantId = g.Key,
                Average = g.Average(r => r.Rating),
                Count = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId, cancellationToken)
            ?? throw new InvalidOperationException("Restaurant not found.");

        if (stats is null)
        {
            restaurant.AverageRating = 0;
            restaurant.TotalReviews = 0;
        }
        else
        {
            restaurant.AverageRating = Math.Round((decimal)stats.Average, 2);
            restaurant.TotalReviews = stats.Count;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RestaurantRatingResponse
        {
            RestaurantId = restaurantId,
            AverageRating = restaurant.AverageRating,
            TotalReviews = restaurant.TotalReviews
        };
    }

    private async Task EnsureRestaurantExistsAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Restaurants.AnyAsync(r => r.Id == restaurantId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("Restaurant not found.");
        }
    }

    private ReviewResponse MapReview(Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            RestaurantId = review.RestaurantId,
            OrderId = review.OrderId,
            Rating = review.Rating,
            Comment = review.Comment,
            IsAnonymous = review.IsAnonymous,
            CreatedAt = review.CreatedAt,
            AuthorName = review.IsAnonymous ? "Anonim" : $"{review.User.FirstName} {review.User.LastName[0]}."
        };
    }
}


