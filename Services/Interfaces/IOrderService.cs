using OdiNow.Contracts.Requests.Orders;
using OdiNow.Contracts.Responses.Orders;
using OdiNow.Models;

namespace OdiNow.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetOrderAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderResponse>> GetOrdersAsync(Guid userId, OrderStatus? status, CancellationToken cancellationToken = default);
    Task<OrderResponse?> ConfirmOrderAsync(Guid userId, Guid orderId, ConfirmOrderRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelOrderAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
    Task<PaymentResponse> ProcessPaymentAsync(Guid userId, PaymentRequest request, CancellationToken cancellationToken = default);
}


