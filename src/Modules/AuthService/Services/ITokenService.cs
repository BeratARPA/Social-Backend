using AuthService.Data.Entities;

namespace AuthService.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserCredential user);
        string GenerateRefreshToken();
    }
}
