using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = default!;
}


