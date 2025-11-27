using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class TicketClaim
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid TicketDropId { get; set; }

    public TicketDrop TicketDrop { get; set; } = default!;

    [Required]
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    [Required]
    [MaxLength(160)]
    public string Code { get; set; } = default!;

    [MaxLength(64)]
    public string QrPayload { get; set; } = default!;

    public TicketClaimStatus Status { get; set; } = TicketClaimStatus.Claimed;

    public DateTimeOffset ClaimedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? RedeemedAt { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }
}


