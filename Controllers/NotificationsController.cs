using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Notifications;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserContextAccessor _userContextAccessor;

    public NotificationsController(INotificationService notificationService, IUserContextAccessor userContextAccessor)
    {
        _notificationService = notificationService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetNotification(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var notification = await _notificationService.GetNotificationAsync(userId, id, cancellationToken);
        return notification is not null ? Ok(notification) : NotFound();
    }

    [HttpPost("admin")]
    [Authorize]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.CreateNotificationAsync(request, cancellationToken);
        return Ok(notification);
    }

    [HttpPut("admin/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateNotification(Guid id, [FromBody] UpdateNotificationRequest request, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.UpdateNotificationAsync(id, request, cancellationToken);
        return notification is not null ? Ok(notification) : NotFound();
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var result = await _notificationService.MarkAsReadAsync(userId, id, cancellationToken);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var count = await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(new { updated = count });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotification(Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var deleted = await _notificationService.DeleteNotificationAsync(userId, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

