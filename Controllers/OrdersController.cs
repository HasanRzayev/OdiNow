using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Orders;
using OdiNow.Models;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IUserContextAccessor _userContextAccessor;

    public OrdersController(IOrderService orderService, IUserContextAccessor userContextAccessor)
    {
        _orderService = orderService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderStatus? status, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var orders = await _orderService.GetOrdersAsync(userId, status, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var order = await _orderService.GetOrderAsync(userId, orderId, cancellationToken);
        return order is not null ? Ok(order) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var order = await _orderService.CreateOrderAsync(userId, request, cancellationToken);
        return Ok(order);
    }

    [HttpPost("{orderId:guid}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, [FromBody] ConfirmOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var order = await _orderService.ConfirmOrderAsync(userId, orderId, request, cancellationToken);
        return order is not null ? Ok(order) : NotFound();
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var result = await _orderService.CancelOrderAsync(userId, orderId, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("payments")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var payment = await _orderService.ProcessPaymentAsync(userId, request, cancellationToken);
        return Ok(payment);
    }
}


