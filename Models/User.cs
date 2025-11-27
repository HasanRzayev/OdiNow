using System.ComponentModel.DataAnnotations;

namespace OdiNow.Models;

/// <summary>
/// Represents the core account that owns orders, addresses and preferences.
/// </summary>
public class User
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(80)]
    public string FirstName { get; set; } = default!;

    [MaxLength(80)]
    public string LastName { get; set; } = default!;

    [MaxLength(160)]
    public string? Email { get; set; }

    public bool IsEmailVerified { get; set; }

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = default!;

    [MaxLength(6)]
    public string PhoneCountryCode { get; set; } = "+994";

    /// <summary>
    /// Stores the hashed representation of the user's password.
    /// </summary>
    [MaxLength(512)]
    public string PasswordEmb5 { get; set; } = default!;

    public bool IsDeleted { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<PhoneVerification> PhoneVerifications { get; set; } = new List<PhoneVerification>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();

    public ICollection<CouponView> CouponViews { get; set; } = new List<CouponView>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public UserSetting? Setting { get; set; }
}

