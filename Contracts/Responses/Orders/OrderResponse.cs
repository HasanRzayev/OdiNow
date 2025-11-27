using OdiNow.Models;

namespace OdiNow.Contracts.Responses.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public Guid RestaurantId { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? ReservationCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IEnumerable<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
}


