using Speakat.Domain.Enums;

namespace Speakat.Domain.Entities;

public class User
{
    public long UserId { get; set; }
    public SocialType SocialType { get; set; }
    public string SocialId { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public Gender? Gender { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageKey { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public UserSetting? Setting { get; set; }
}
