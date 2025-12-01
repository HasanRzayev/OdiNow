namespace OdiNow.Contracts.Responses;

public class CancellationRightsResponse
{
    public int AvailableRights { get; set; }
    public int MaxRights { get; set; } = 5;
    public DateTimeOffset? NextRenewalAt { get; set; }
    public List<CancellationRightItem> Rights { get; set; } = new();
}

public class CancellationRightItem
{
    public Guid Id { get; set; }
    public bool IsUsed { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
}




