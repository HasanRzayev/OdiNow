using System.Security.Cryptography;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Orders;
using OdiNow.Contracts.Responses.Orders;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public OrderService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<OrderResponse> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken)
            ?? throw new InvalidOperationException("Restaurant not found.");

        var menuItems = await _dbContext.MenuItems
            .Where(mi => request.Items.Select(i => i.MenuItemId).Contains(mi.Id))
            .ToListAsync(cancellationToken);

        if (menuItems.Count != request.Items.Count())
        {
            throw new InvalidOperationException("One or more menu items are invalid.");
        }

        var orderItems = request.Items.Select(item =>
        {
            var menuItem = menuItems.First(mi => mi.Id == item.MenuItemId);
            return new OrderItem
            {
                MenuItemId = menuItem.Id,
                Name = menuItem.Title,
                Quantity = item.Quantity,
                UnitPrice = menuItem.BasePrice,
                DiscountAmount = 0
            };
        }).ToList();

        var deposit = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity) * 0.2m;
        var remaining = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity) - deposit;

        var order = new Order
        {
            UserId = userId,
            RestaurantId = restaurant.Id,
            OrderType = request.OrderType,
            OrderNumber = GenerateOrderNumber(),
            ReservationCode = GenerateReservationCode(),
            ReservationExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
            // QR kodu sifariş yaradılarkən generasiya edirik ki, həm backend-də yadda qalsın,
            // həm də frontend eyni dəyəri istifadə etsin
            QrCode = null, // müvəqqəti, aşağıda doldurulacaq
            DepositAmount = decimal.Round(deposit, 2),
            RemainingAmount = decimal.Round(remaining, 2),
            Notes = request.Notes,
            Items = orderItems
        };

        // QR kod dəyərini OrderNumber və ReservationCode əsasında generasiya edirik
        order.QrCode = $"ORDER-{order.OrderNumber}-{order.ReservationCode}";

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse?> GetOrderAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

        return order is null ? null : _mapper.Map<OrderResponse>(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersAsync(Guid userId, OrderStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders.AsNoTracking().Include(o => o.Items).Where(o => o.UserId == userId);
        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status);
        }

        var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);
        return orders.Select(_mapper.Map<OrderResponse>).ToList();
    }

    public async Task<OrderResponse?> ConfirmOrderAsync(Guid userId, Guid orderId, ConfirmOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        if (order.ReservationCode != request.ReservationCode)
        {
            throw new InvalidOperationException("Reservation code mismatch.");
        }

        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<bool> CancelOrderAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
        {
            return false;
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(Guid userId, PaymentRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("Order not found.");

        if (request.PaymentType == PaymentType.Deposit && order.DepositAmount <= 0)
        {
            throw new InvalidOperationException("Deposit already paid.");
        }

        if (request.PaymentType == PaymentType.Remaining && order.RemainingAmount <= 0)
        {
            throw new InvalidOperationException("Remaining amount already paid.");
        }

        var payment = new Payment
        {
            OrderId = order.Id,
            UserId = userId,
            Amount = request.Amount,
            PaymentType = request.PaymentType,
            Status = PaymentStatus.Succeeded,
            ProcessedAt = DateTimeOffset.UtcNow,
            Method = request.Method
        };

        if (request.PaymentType == PaymentType.Deposit)
        {
            order.DepositAmount = Math.Max(0, order.DepositAmount - request.Amount);
        }
        else
        {
            order.RemainingAmount = Math.Max(0, order.RemainingAmount - request.Amount);
        }

        if (order.DepositAmount == 0 && order.RemainingAmount == 0)
        {
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTimeOffset.UtcNow;
        }

        await _dbContext.Payments.AddAsync(payment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PaymentResponse>(payment);
    }

    private async Task<OrderResponse> MapOrderResponseAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken)
            ?? throw new InvalidOperationException("Order not found.");

        return _mapper.Map<OrderResponse>(order);
    }

    private static string GenerateOrderNumber()
    {
        return $"ODN-{RandomNumberGenerator.GetInt32(10000, 99999)}";
    }

    private static string GenerateReservationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}

