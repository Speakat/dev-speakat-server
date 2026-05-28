using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private const string Algorithm = SecurityAlgorithms.HmacSha256;

    private const int RefreshTokenByteSize = 64;

    private readonly SymmetricSecurityKey _signingKey;

    private readonly int _accessTokenExpirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        _accessTokenExpirationMinutes = int.TryParse(
            configuration["Jwt:AccessTokenExpirationMinutes"], out var minutes)
            ? minutes
            : 30;
    }

    // accessToken 생성
    public string GenerateAccessToken(string userUuid)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userUuid)
        };

        var credentials = new SigningCredentials(_signingKey, Algorithm);

        var now = DateTime.UtcNow;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            Expires = now.AddMinutes(_accessTokenExpirationMinutes),
            SigningCredentials = credentials
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    // refreshToken 생성
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(RefreshTokenByteSize);
        return Convert.ToBase64String(randomBytes);
    }
}
