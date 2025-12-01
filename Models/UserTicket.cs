using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class UserTicket
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public TicketStatus Status { get; set; } = TicketStatus.Available;

    public Guid? OfferId { get; set; }

    public Offer? Offer { get; set; }

    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UsedAt { get; set; }
}





