namespace OdiNow.Contracts.Responses.Offers;

public class CouponResponse
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public string Title { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public DateTimeOffset ViewedAt { get; set; }
}


