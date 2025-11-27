using OdiNow.Contracts.Requests.Notifications;
using OdiNow.Contracts.Responses.Notifications;

namespace OdiNow.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationResponse?> GetNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<NotificationResponse?> UpdateNotificationAsync(Guid notificationId, UpdateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteNotificationAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
}


