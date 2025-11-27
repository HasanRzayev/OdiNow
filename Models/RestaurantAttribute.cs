using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class RestaurantAttribute
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public Guid RestaurantId { get; set; }

    [MaxLength(60)]
    public string Key { get; set; } = default!;

    [MaxLength(120)]
    public string Value { get; set; } = default!;

    public Restaurant Restaurant { get; set; } = default!;
}


