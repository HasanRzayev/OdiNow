using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class CancellationRight
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;

    public bool IsUsed { get; set; } = false;

    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UsedAt { get; set; }

    public Guid? OrderId { get; set; }

    public Order? Order { get; set; }
}




