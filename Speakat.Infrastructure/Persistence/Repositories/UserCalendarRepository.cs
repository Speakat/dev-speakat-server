using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserCalendarRepository(AppDbContext context) : IUserCalendarRepository
{
    public async Task<IReadOnlyList<CalendarDayData>> GetMonthlyActivityAsync(long userId, int year, int month)
    {
        var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1);

        var rows = await context.GameSessions
            .Where(gs => gs.UserId == userId
                      && gs.Status == "COMPLETED"
                      && gs.StartedAt >= from
                      && gs.StartedAt < to)
            .GroupBy(gs => gs.StartedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows
            .Select(r => new CalendarDayData(DateOnly.FromDateTime(r.Date), r.Count))
            .OrderBy(r => r.Date)
            .ToList();
    }
}
