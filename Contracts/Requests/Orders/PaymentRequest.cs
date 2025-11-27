using System.ComponentModel.DataAnnotations;
using OdiNow.Models;

namespace OdiNow.Contracts.Requests.Orders;

public class PaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [Range(0.1, double.MaxValue)]
    public decimal Amount { get; set; }

    public PaymentType PaymentType { get; set; } = PaymentType.Deposit;

    [MaxLength(40)]
    public string Method { get; set; } = "Card";
}


