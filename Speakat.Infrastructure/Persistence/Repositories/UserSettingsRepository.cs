using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserSettingsRepository(AppDbContext db) : IUserSettingsRepository
{
    public Task<UserSetting?> GetByUserIdAsync(long userId) =>
        db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
}
