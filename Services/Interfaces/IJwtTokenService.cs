using OdiNow.Models;

namespace OdiNow.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTimeOffset ExpiresAt) GenerateRefreshToken();
}


