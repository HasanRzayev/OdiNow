using OdiNow.Models;

namespace OdiNow.Contracts.Responses.Profile;

public class FavoriteResponse
{
    public Guid Id { get; set; }
    public FavoriteType FavoriteType { get; set; }
    public Guid TargetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}


