using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserSettingsRepository(AppDbContext context) : IUserSettingsRepository
{
    public Task<UserSetting?> GetByUserIdAsync(long userId) =>
        context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<UserSetting> UpdateAsync(long userId, bool? showNpcScript, int? streakGoal)
    {
        var settings = await context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId)
            ?? throw new InvalidOperationException($"UserSetting for user {userId} 없음");

        if (showNpcScript.HasValue)
            settings.ShowNpcScript = showNpcScript.Value;

        if (streakGoal.HasValue)
            settings.StreakGoal = streakGoal.Value;

        await context.SaveChangesAsync();
        return settings;
    }
}
