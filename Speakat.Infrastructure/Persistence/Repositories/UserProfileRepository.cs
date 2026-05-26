using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(AppDbContext db) : IUserProfileRepository
{
    public async Task<UserProfileData?> GetProfileDataAsync(long userId)
    {
        var user = await db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new { u.UserUuid, u.Nickname, u.ProfileImageKey })
            .FirstOrDefaultAsync();

        if (user is null) return null;

        var scores = await db.GameSessions
            .Where(gs => gs.UserId == userId && gs.Status == "COMPLETED")
            .GroupBy(_ => 0)
            .Select(g => new
            {
                AvgSemantic = g.Average(gs => (double?)gs.SemanticScore),
                AvgGrammar = g.Average(gs => (double?)gs.GrammarScore),
                AvgNaturalness = g.Average(gs => (double?)gs.NaturalnessScore)
            })
            .FirstOrDefaultAsync();

        return new UserProfileData(
            user.UserUuid,
            user.Nickname,
            user.ProfileImageKey,
            scores?.AvgSemantic,
            scores?.AvgGrammar,
            scores?.AvgNaturalness
        );
    }
}
