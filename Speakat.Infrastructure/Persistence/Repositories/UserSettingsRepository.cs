using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserSettingsRepository(AppDbContext db) : IUserSettingsRepository
{
    public Task<UserSetting?> GetByUserIdAsync(long userId) =>
        db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<UserSetting> UpdateAsync(long userId, bool? showNpcScript, int? streakGoal)
    {
        var settings = await db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId)
            ?? throw new InvalidOperationException($"UserSetting for user {userId} 없음");

        if (showNpcScript.HasValue)
            settings.ShowNpcScript = showNpcScript.Value;

        if (streakGoal.HasValue)
            settings.StreakGoal = streakGoal.Value;

        await db.SaveChangesAsync();
        return settings;
    }
}
