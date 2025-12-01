namespace OdiNow.Options;

public class TicketOptions
{
    public const string SectionName = "Ticket";

    public int GenerationIntervalMinutes { get; set; } = 30;

    public int MaxActiveTickets { get; set; } = 5;
}





