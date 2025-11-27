namespace OdiNow.Contracts.Responses.Catalog;

public class RestaurantDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string? Email { get; set; }
    public string AddressLine { get; set; } = default!;
    public string City { get; set; } = default!;
    public string District { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public IEnumerable<RestaurantAttributeResponse> Attributes { get; set; } = Array.Empty<RestaurantAttributeResponse>();
    public IEnumerable<MenuItemResponse> MenuItems { get; set; } = Array.Empty<MenuItemResponse>();
    public IEnumerable<OfferResponse> ActiveOffers { get; set; } = Array.Empty<OfferResponse>();
}


