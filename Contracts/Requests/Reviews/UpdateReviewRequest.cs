using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Reviews;

public class UpdateReviewRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(400)]
    public string? Comment { get; set; }

    public bool IsAnonymous { get; set; }
}


