using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Profile;

public class UpdateProfileRequest
{
    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = default!;

    [Required]
    [MaxLength(80)]
    public string LastName { get; set; } = default!;

    [EmailAddress]
    [MaxLength(160)]
    public string? Email { get; set; }

    [Url]
    public string? ProfilePhotoUrl { get; set; }
}


