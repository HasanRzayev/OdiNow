namespace OdiNow.Contracts.Responses.Catalog;

public class RestaurantSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string City { get; set; } = default!;
    public string District { get; set; } = default!;
    public string? AddressLine { get; set; }
    public string? ImageUrl { get; set; }
    public bool HasActiveOffer { get; set; }
    public int EstimatedDeliveryMinutes { get; set; }
    public double? DistanceMeters { get; set; }
}


