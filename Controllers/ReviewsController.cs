using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Reviews;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IUserContextAccessor _userContextAccessor;

    public ReviewsController(IReviewService reviewService, IUserContextAccessor userContextAccessor)
    {
        _reviewService = reviewService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet("restaurants/{restaurantId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRestaurantReviews(Guid restaurantId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewService.GetRestaurantReviewsAsync(restaurantId, Math.Max(1, page), Math.Clamp(pageSize, 1, 50), cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var review = await _reviewService.CreateReviewAsync(userId, request, cancellationToken);
        return Ok(review);
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var review = await _reviewService.UpdateReviewAsync(userId, reviewId, request, cancellationToken);
        return review is not null ? Ok(review) : NotFound();
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var deleted = await _reviewService.DeleteReviewAsync(userId, reviewId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("restaurants/{restaurantId:guid}/recalculate")]
    [Authorize]
    public async Task<IActionResult> RecalculateRating(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await _reviewService.RecalculateRestaurantRatingAsync(restaurantId, cancellationToken);
        return Ok(result);
    }
}

