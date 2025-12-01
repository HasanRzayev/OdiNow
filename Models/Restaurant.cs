using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

public class Restaurant
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(160)]
    public string Name { get; set; } = default!;

    [MaxLength(400)]
    public string? Description { get; set; }

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = default!;

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string AddressLine { get; set; } = default!;

    [MaxLength(80)]
    public string City { get; set; } = default!;

    [MaxLength(80)]
    public string District { get; set; } = default!;

    [MaxLength(16)]
    public string PostalCode { get; set; } = default!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public TimeSpan OpeningTime { get; set; }

    public TimeSpan ClosingTime { get; set; }

    public decimal AverageRating { get; set; }

    public int TotalReviews { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<RestaurantAttribute> Attributes { get; set; } = new List<RestaurantAttribute>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}


