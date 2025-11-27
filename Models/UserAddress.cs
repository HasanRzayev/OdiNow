using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

/// <summary>
/// Stores one of potentially many delivery or pickup locations the user may use.
/// </summary>
public class UserAddress
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(40)]
    public string Label { get; set; } = "Home";

    [MaxLength(160)]
    public string Line1 { get; set; } = default!;

    [MaxLength(160)]
    public string? Line2 { get; set; }

    [MaxLength(80)]
    public string City { get; set; } = default!;

    [MaxLength(80)]
    public string District { get; set; } = default!;

    [MaxLength(16)]
    public string PostalCode { get; set; } = default!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool IsDefault { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = default!;
}


