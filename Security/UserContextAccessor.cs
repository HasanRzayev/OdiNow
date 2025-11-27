using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OdiNow.Security;

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            throw new UnauthorizedAccessException("User context unavailable.");
        }

        var subClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                       user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (subClaim == null || !Guid.TryParse(subClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User identifier missing.");
        }

        return userId;
    }
}

