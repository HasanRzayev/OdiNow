using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(32)]
    public string OrderNumber { get; set; } = default!;

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public Guid RestaurantId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public OrderType OrderType { get; set; } = OrderType.Pickup;

    [MaxLength(16)]
    public string? ReservationCode { get; set; }

    public DateTimeOffset? ReservationExpiresAt { get; set; }

    public decimal DepositAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public int? PickupEtaMinutes { get; set; }

    public string? QrCode { get; set; }

    public int CancellationCountUsed { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ConfirmedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public User User { get; set; } = default!;

    public Restaurant Restaurant { get; set; } = default!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}


