namespace Speakat.Application.Users.Repositories;

public interface IUserStreakRepository
{
    Task<IReadOnlyList<DateOnly>> GetCompletedSessionDatesAsync(long userId);
    Task<int?> GetStreakGoalAsync(long userId);
}
