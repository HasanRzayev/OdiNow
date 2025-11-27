using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Offers;

public class UpdateOfferRequest
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

    public bool IsPersonalized { get; set; }

    public bool IsActive { get; set; }
}


