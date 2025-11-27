using OdiNow.Contracts.Requests.Auth;
using OdiNow.Contracts.Responses.Auth;

namespace OdiNow.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> SendPhoneCodeAsync(SendPhoneCodeRequest request, CancellationToken cancellationToken = default);
    Task<bool> VerifyPhoneAsync(VerifyPhoneRequest request, CancellationToken cancellationToken = default);
}


