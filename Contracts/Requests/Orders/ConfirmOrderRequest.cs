using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Orders;

public class ConfirmOrderRequest
{
    [Required]
    public string ReservationCode { get; set; } = default!;
}


