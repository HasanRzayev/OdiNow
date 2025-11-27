using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Requests.Notifications;
using OdiNow.Contracts.Responses.Notifications;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public NotificationService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications.Select(_mapper.Map<NotificationResponse>).ToList();
    }

    public async Task<NotificationResponse?> GetNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

        return notification is null ? null : _mapper.Map<NotificationResponse>(notification);
    }

    public async Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("User not found.");
        }

        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            MetadataJson = request.MetadataJson
        };

        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationResponse>(notification);
    }

    public async Task<NotificationResponse?> UpdateNotificationAsync(Guid notificationId, UpdateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
        if (notification is null)
        {
            return null;
        }

        notification.Title = request.Title;
        notification.Message = request.Message;
        notification.Type = request.Type;
        notification.MetadataJson = request.MetadataJson;
        notification.IsRead = request.IsRead;
        notification.ReadAt = request.IsRead ? DateTimeOffset.UtcNow : null;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<NotificationResponse>(notification);
    }

    public async Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);
        if (notification is null)
        {
            return false;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return notifications.Count;
    }

    public async Task<bool> DeleteNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);
        if (notification is null)
        {
            return false;
        }

        _dbContext.Notifications.Remove(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}


