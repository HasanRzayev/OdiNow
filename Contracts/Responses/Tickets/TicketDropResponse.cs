namespace OdiNow.Contracts.Responses.Tickets;

public class TicketDropResponse
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public string RestaurantName { get; set; } = default!;
    public DateTimeOffset AvailableFrom { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public int TicketsTotal { get; set; }
    public int TicketsRemaining { get; set; }
}


