namespace Speakat.Domain.Entities;

public class UserSetting
{
    public long UserSettingId { get; set; }
    public string UserId { get; set; } = null!;
    public bool ShowNpcScript { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
