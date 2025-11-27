using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class UserSetting
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "az";

    public bool ReceivePushNotifications { get; set; } = true;

    public bool ReceiveEmailNotifications { get; set; } = false;

    public bool MarketingOptIn { get; set; } = false;

    public Guid? DefaultAddressId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = default!;

    public UserAddress? DefaultAddress { get; set; }
}


