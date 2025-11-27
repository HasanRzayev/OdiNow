using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class TicketDrop
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OfferId { get; set; }

    public Offer Offer { get; set; } = default!;

    [Range(1, int.MaxValue)]
    public int TicketsTotal { get; set; }

    [Range(0, int.MaxValue)]
    public int TicketsRemaining { get; set; }

    public DateTimeOffset AvailableFrom { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<TicketClaim> Claims { get; set; } = new List<TicketClaim>();
}


