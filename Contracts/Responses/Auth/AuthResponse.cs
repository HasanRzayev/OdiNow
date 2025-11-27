namespace OdiNow.Contracts.Responses.Auth;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public bool IsPhoneVerified { get; set; }
    public string AccessToken { get; set; } = default!;
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = default!;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}


