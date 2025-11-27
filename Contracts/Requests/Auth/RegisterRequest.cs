using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Auth;

public class RegisterRequest
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

    [Phone]
    [Required]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = default!;
}


