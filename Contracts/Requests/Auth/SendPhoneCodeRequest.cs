using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Auth;

public class SendPhoneCodeRequest
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = default!;
}


