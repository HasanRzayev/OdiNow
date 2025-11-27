using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = default!;

    [MaxLength(80)]
    public string Slug { get; set; } = default!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}


