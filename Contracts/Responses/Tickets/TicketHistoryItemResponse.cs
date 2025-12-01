namespace OdiNow.Contracts.Responses.Tickets;

public class TicketHistoryItemResponse
{
    public Guid TicketId { get; set; }
    public Guid OfferId { get; set; }
    public string OfferTitle { get; set; } = default!;
    public string? RestaurantName { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTimeOffset UsedAt { get; set; }
}





