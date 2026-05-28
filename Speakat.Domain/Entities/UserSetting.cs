namespace Speakat.Domain.Entities;

public class UserSetting
{
    public long UserSettingId { get; set; }
    public long UserId { get; set; }
    public bool ShowNpcScript { get; set; } = true;
    public int? StreakGoal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
