using System.ComponentModel.DataAnnotations;
using OdiNow.Models;

namespace OdiNow.Contracts.Requests.Orders;

public class CreateOrderRequest
{
    [Required]
    public Guid RestaurantId { get; set; }

    public OrderType OrderType { get; set; } = OrderType.Pickup;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<OrderItemRequest> Items { get; set; } = Array.Empty<OrderItemRequest>();
}

public class OrderItemRequest
{
    [Required]
    public Guid MenuItemId { get; set; }

    [Range(1, 20)]
    public int Quantity { get; set; } = 1;
}


