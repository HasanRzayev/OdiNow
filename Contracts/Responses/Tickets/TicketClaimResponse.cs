using OdiNow.Models;

namespace OdiNow.Contracts.Responses.Tickets;

public class TicketClaimResponse
{
    public Guid Id { get; set; }
    public Guid TicketDropId { get; set; }
    public Guid OfferId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public string RestaurantName { get; set; } = default!;
    public TicketClaimStatus Status { get; set; }
    public DateTimeOffset ClaimedAt { get; set; }
    public DateTimeOffset? RedeemedAt { get; set; }
    public DateTimeOffset? ExpiredAt { get; set; }
    public string? QrPayload { get; set; }
}


