namespace Speakat.Application.Auth.Interfaces;

// OAuth에서 받아온 사용자 정보를 담는 내부 DTO
public class OAuthUserInfo
{ 
    public required string SocialId { get; set; } 
    public string? Email { get; set; } 
    public required string Nickname { get; set; }
    public required string ProfileImageUrl { get; set; }
}