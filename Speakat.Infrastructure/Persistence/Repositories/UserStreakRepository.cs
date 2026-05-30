using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserStreakRepository(AppDbContext db) : IUserStreakRepository
{
    public async Task<IReadOnlyList<DateOnly>> GetCompletedSessionDatesAsync(long userId)
    {
        var dateTimes = await db.GameSessions
            .Where(gs => gs.UserId == userId && gs.Status == "COMPLETED")
            .Select(gs => gs.StartedAt.Date)
            .Distinct()
            .ToListAsync();

        return dateTimes.Select(DateOnly.FromDateTime).ToList();
    }

    public async Task<int?> GetStreakGoalAsync(long userId)
    {
        return await db.UserSettings
            .Where(s => s.UserId == userId)
            .Select(s => s.StreakGoal)
            .FirstOrDefaultAsync();
    }
}
