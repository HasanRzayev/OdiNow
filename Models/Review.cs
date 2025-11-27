using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public Guid RestaurantId { get; set; }

    [ForeignKey(nameof(Order))]
    public Guid? OrderId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(400)]
    public string? Comment { get; set; }

    public bool IsAnonymous { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = default!;

    public Restaurant Restaurant { get; set; } = default!;

    public Order? Order { get; set; }
}


