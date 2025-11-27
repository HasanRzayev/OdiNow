using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class Offer
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(160)]
    public string Title { get; set; } = default!;

    [MaxLength(400)]
    public string? Description { get; set; }

    public decimal DiscountPercent { get; set; }

    public DateTimeOffset StartAt { get; set; }

    public DateTimeOffset EndAt { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public Guid? RestaurantId { get; set; }

    [ForeignKey(nameof(MenuItem))]
    public Guid? MenuItemId { get; set; }

    public bool IsPersonalized { get; set; }

    public bool IsActive { get; set; } = true;

    public Restaurant? Restaurant { get; set; }

    public MenuItem? MenuItem { get; set; }

    public ICollection<CouponView> CouponViews { get; set; } = new List<CouponView>();

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}


