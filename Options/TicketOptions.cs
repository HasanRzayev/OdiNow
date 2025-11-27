namespace OdiNow.Options;

public class TicketOptions
{
    public const string SectionName = "Ticket";

    public int GenerationIntervalMinutes { get; set; } = 30;

    public int DropDurationMinutes { get; set; } = 30;

    public int TicketsPerInterval { get; set; } = 1;

    public int MaxActiveTickets { get; set; } = 5;
}


