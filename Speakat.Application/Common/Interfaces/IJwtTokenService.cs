namespace Speakat.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId);
    string GenerateRefreshToken();
}