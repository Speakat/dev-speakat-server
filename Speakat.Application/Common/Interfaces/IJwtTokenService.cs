namespace Speakat.Application.Auth.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(long userId);
    string GenerateRefreshToken();
}