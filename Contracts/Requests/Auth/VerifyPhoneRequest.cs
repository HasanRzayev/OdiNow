using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Auth;

public class VerifyPhoneRequest
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    [MaxLength(6)]
    public string Code { get; set; } = default!;
}


