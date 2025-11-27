using OdiNow.Models;

namespace OdiNow.Contracts.Responses.Orders;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string Method { get; set; } = default!;
}


