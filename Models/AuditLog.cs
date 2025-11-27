using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(80)]
    public string Actor { get; set; } = default!;

    [MaxLength(120)]
    public string Action { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? MetadataJson { get; set; }
}


