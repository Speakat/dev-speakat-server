namespace Speakat.Application.Users.Dtos;

public class UserStreakDto
{
    public int CurrentStreak { get; init; }
    public int LongestStreak { get; init; }
    public bool TodayCompleted { get; init; }
    public int? StreakGoal { get; init; }
}
