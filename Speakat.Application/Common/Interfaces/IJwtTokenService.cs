namespace Speakat.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(long userId);
    string GenerateRefreshToken();
}