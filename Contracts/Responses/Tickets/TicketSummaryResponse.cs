namespace OdiNow.Contracts.Responses.Tickets;

public class TicketSummaryResponse
{
    public int AvailableTickets { get; set; }
    public int MaxTickets { get; set; }
    public DateTimeOffset? NextTicketAt { get; set; }
    public IReadOnlyCollection<TicketHistoryItemResponse> History { get; set; } = Array.Empty<TicketHistoryItemResponse>();
}





