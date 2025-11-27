using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Offers;

public class CreateOfferRequest
{
    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = default!;

    [MaxLength(400)]
    public string? Description { get; set; }

    [Range(1, 100)]
    public decimal DiscountPercent { get; set; }

    public DateTimeOffset StartAt { get; set; }

    public DateTimeOffset EndAt { get; set; }

    public Guid? RestaurantId { get; set; }

    public Guid? MenuItemId { get; set; }

    public bool IsPersonalized { get; set; }
}


