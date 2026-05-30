using Speakat.Domain.Entities;

namespace Speakat.Application.Users.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSetting?> GetByUserIdAsync(long userId);
    Task<UserSetting> UpdateAsync(long userId, bool? showNpcScript, int? streakGoal);
}
