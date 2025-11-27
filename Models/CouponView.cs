using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class CouponView
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(Offer))]
    public Guid OfferId { get; set; }

    public DateTimeOffset ViewedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = default!;

    public Offer Offer { get; set; } = default!;
}


