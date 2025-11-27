using System.ComponentModel.DataAnnotations;
using OdiNow.Models;

namespace OdiNow.Contracts.Requests.Notifications;

public class CreateNotificationRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = default!;

    public NotificationType Type { get; set; } = NotificationType.General;

    public string? MetadataJson { get; set; }
}


