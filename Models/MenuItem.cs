using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class MenuItem
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public Guid RestaurantId { get; set; }

    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }

    [MaxLength(160)]
    public string Title { get; set; } = default!;

    [MaxLength(400)]
    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int PreparationTimeMinutes { get; set; }

    public string? ImageUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Restaurant Restaurant { get; set; } = default!;

    public Category Category { get; set; } = default!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}


