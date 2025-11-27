using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class PhoneVerification
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(6)]
    public string Code { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? VerifiedAt { get; set; }

    public VerificationChannel Channel { get; set; } = VerificationChannel.Sms;

    public User User { get; set; } = default!;
}


