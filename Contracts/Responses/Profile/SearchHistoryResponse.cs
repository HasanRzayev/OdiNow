namespace OdiNow.Contracts.Responses.Profile;

public class SearchHistoryResponse
{
    public Guid Id { get; set; }
    public string Query { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}


