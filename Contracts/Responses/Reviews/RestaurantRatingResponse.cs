namespace OdiNow.Contracts.Responses.Reviews;

public class RestaurantRatingResponse
{
    public Guid RestaurantId { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
}


