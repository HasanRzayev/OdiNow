using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class Payment
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Order))]
    public Guid OrderId { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public PaymentType PaymentType { get; set; } = PaymentType.Deposit;

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTimeOffset? ProcessedAt { get; set; }

    [MaxLength(80)]
    public string? Reference { get; set; }

    [MaxLength(40)]
    public string Method { get; set; } = "Card";

    public User User { get; set; } = default!;

    public Order Order { get; set; } = default!;
}


