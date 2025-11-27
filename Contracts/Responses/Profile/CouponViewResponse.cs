namespace OdiNow.Contracts.Responses.Profile;

public class CouponViewResponse
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public DateTimeOffset ViewedAt { get; set; }
}


