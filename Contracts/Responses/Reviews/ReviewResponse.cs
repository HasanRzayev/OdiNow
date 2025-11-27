namespace OdiNow.Contracts.Responses.Reviews;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string AuthorName { get; set; } = default!;
}


