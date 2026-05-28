using Speakat.Domain.Enums;

namespace Speakat.Application.Auth.Dtos;

public class OAuthLoginResponseDto
{
    public string UserUuid { get; set; } = null!;
    public string? Email { get; set; }
    public required string Nickname { get; set; }
    public string? ProfileImageUrl { get; set; }
    public SocialType Provider { get; set; } 
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public bool IsNewUser { get; set; }
}
