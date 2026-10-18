using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserStreakRepository(AppDbContext context) : IUserStreakRepository
{
    public async Task<IReadOnlyList<DateOnly>> GetCompletedSessionDatesAsync(long userId)
    {
        var dateTimes = await context.GameSessions
            .Where(gs => gs.UserId == userId && gs.Status == "COMPLETED")
            .Select(gs => gs.StartedAt.Date)
            .Distinct()
            .ToListAsync();

        return dateTimes.Select(DateOnly.FromDateTime).ToList();
    }

    public async Task<int?> GetStreakGoalAsync(long userId)
    {
        return await context.UserSettings
            .Where(s => s.UserId == userId)
            .Select(s => s.StreakGoal)
            .FirstOrDefaultAsync();
    }
}
