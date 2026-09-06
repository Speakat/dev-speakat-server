using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(AppDbContext context) : IUserProfileRepository
{
    public async Task<UserProfileData?> GetProfileDataAsync(long userId)
    {
        var user = await context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new { u.UserUuid, u.Nickname, u.ProfileImageKey })
            .FirstOrDefaultAsync();

        if (user is null) return null;

        var scores = await context.GameSessions
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

    public async Task<(string UserUuid, string Nickname, string? ProfileImageKey)> UpdateProfileAsync(
        long userId, string? nickname, string? profileImageKey)
    {
        var user = await context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"User {userId} 없음");

        if (nickname is not null)
            user.Nickname = nickname;

        if (profileImageKey is not null)
            user.ProfileImageKey = profileImageKey;

        await context.SaveChangesAsync();

        return (user.UserUuid, user.Nickname, user.ProfileImageKey);
    }
}
