using Speakat.Domain.Entities;

namespace Speakat.Application.Users.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSetting?> GetByUserIdAsync(long userId);
}
