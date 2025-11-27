using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Auth;

public class LoginRequest
{
    [Required]
    [MaxLength(160)]
    public string Identifier { get; set; } = default!; // email or phone

    [Required]
    public string Password { get; set; } = default!;
}


