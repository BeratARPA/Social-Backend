using AuthService.Data.Entities;
using System.Security.Claims;

namespace AuthService.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserCredential user);
        string GenerateRefreshToken();
        string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiresIn);
        ClaimsPrincipal ValidateToken(string token);
        string FindClaimValue(ClaimsPrincipal principal, string claimType);
    }
}
