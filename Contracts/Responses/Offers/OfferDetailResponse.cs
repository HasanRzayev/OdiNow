namespace OdiNow.Contracts.Responses.Offers;

public class OfferDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public bool IsPersonalized { get; set; }
    public bool IsActive { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public Guid? MenuItemId { get; set; }
    public string? MenuItemTitle { get; set; }
}


