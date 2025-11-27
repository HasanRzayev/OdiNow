using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Reviews;

public class CreateReviewRequest
{
    [Required]
    public Guid RestaurantId { get; set; }

    public Guid? OrderId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(400)]
    public string? Comment { get; set; }

    public bool IsAnonymous { get; set; }
}


