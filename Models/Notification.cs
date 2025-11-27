using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class Notification
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(120)]
    public string Title { get; set; } = default!;

    [MaxLength(500)]
    public string Message { get; set; } = default!;

    public NotificationType Type { get; set; } = NotificationType.General;

    public bool IsRead { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ReadAt { get; set; }

    public string? MetadataJson { get; set; }

    public User User { get; set; } = default!;
}


