using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OdiNow.Contracts.Requests.Auth;
using OdiNow.Contracts.Responses.Auth;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        var existingUser = await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);

        if (existingUser is not null && !existingUser.IsDeleted)
        {
            throw new InvalidOperationException("Phone number already registered.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _dbContext.Users.IgnoreQueryFilters()
                .AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
            {
                throw new InvalidOperationException("Email already registered.");
            }
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = normalizedPhone,
            PasswordEmb5 = _passwordHasher.Hash(request.Password)
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = await IssueTokensAsync(user, cancellationToken);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = NormalizePhone(request.Identifier);
        var user = await _dbContext.Users
            .Include(u => u.PhoneVerifications)
            .FirstOrDefaultAsync(u =>
                u.PhoneNumber == normalizedIdentifier ||
                u.Email == request.Identifier, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!_passwordHasher.Verify(user.PasswordEmb5, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashRefreshToken(request.RefreshToken);

        var refreshToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.PhoneVerifications)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresAt <= DateTimeOffset.UtcNow || refreshToken.RevokedAt is not null)
        {
            throw new UnauthorizedAccessException("Refresh token invalid.");
        }

        refreshToken.RevokedAt = DateTimeOffset.UtcNow;

        return await IssueTokensAsync(refreshToken.User, cancellationToken);
    }

    public async Task<bool> SendPhoneCodeAsync(SendPhoneCodeRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Account not found.");
        }

        var code = GenerateVerificationCode();
        var verification = new PhoneVerification
        {
            UserId = user.Id,
            Code = code,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        await _dbContext.PhoneVerifications.AddAsync(verification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Verification code {Code} generated for user {UserId}", code, user.Id);

        // TODO: integrate SMS provider
        return true;
    }

    public async Task<bool> VerifyPhoneAsync(VerifyPhoneRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        var user = await _dbContext.Users
            .Include(u => u.PhoneVerifications)
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Account not found.");
        }

        var verification = user.PhoneVerifications
            .FirstOrDefault(pv => pv.Code == request.Code && pv.ExpiresAt >= DateTimeOffset.UtcNow);

        if (verification is null)
        {
            return false;
        }

        verification.VerifiedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, accessExpires) = _jwtTokenService.GenerateAccessToken(user);
        var (refreshToken, refreshExpires) = _jwtTokenService.GenerateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashRefreshToken(refreshToken),
            ExpiresAt = refreshExpires
        };

        await _dbContext.RefreshTokens.AddAsync(refreshEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<AuthResponse>(user);
        response.AccessToken = accessToken;
        response.AccessTokenExpiresAt = accessExpires;
        response.RefreshToken = refreshToken;
        response.RefreshTokenExpiresAt = refreshExpires;
        return response;
    }

    private static string NormalizePhone(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Contains('@', StringComparison.Ordinal))
        {
            return trimmed;
        }

        return new string(trimmed.Where(char.IsDigit).ToArray());
    }

    private static string GenerateVerificationCode()
    {
        var random = RandomNumberGenerator.GetInt32(100000, 999999);
        return random.ToString();
    }

    private static string HashRefreshToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes);
    }
}

