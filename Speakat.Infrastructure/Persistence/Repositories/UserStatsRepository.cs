using Microsoft.EntityFrameworkCore;
using Speakat.Application.Users.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserStatsRepository(AppDbContext db) : IUserStatsRepository
{
    public async Task<UserStatsData> GetStatsAsync(long userId)
    {
        var sessionStats = await db.GameSessions
            .Where(gs => gs.UserId == userId)
            .GroupBy(_ => 0)
            .Select(g => new
            {
                TotalSessionsPlayed = g.Count(),
                TotalQuestsCompleted = g.Where(gs => gs.Status == "COMPLETED")
                    .Select(gs => gs.QuestId)
                    .Distinct()
                    .Count(),
                AvgSemantic = g.Where(gs => gs.Status == "COMPLETED")
                    .Average(gs => (double?)gs.SemanticScore),
                AvgGrammar = g.Where(gs => gs.Status == "COMPLETED")
                    .Average(gs => (double?)gs.GrammarScore),
                AvgNaturalness = g.Where(gs => gs.Status == "COMPLETED")
                    .Average(gs => (double?)gs.NaturalnessScore)
            })
            .FirstOrDefaultAsync();

        var totalStagesCompleted = await db.UserStages
            .CountAsync(us => us.UserId == userId && us.CompletedAt != null);

        return new UserStatsData(
            TotalQuestsCompleted: sessionStats?.TotalQuestsCompleted ?? 0,
            TotalSessionsPlayed: sessionStats?.TotalSessionsPlayed ?? 0,
            TotalStagesCompleted: totalStagesCompleted,
            AvgSemanticScore: sessionStats?.AvgSemantic,
            AvgGrammarScore: sessionStats?.AvgGrammar,
            AvgNaturalnessScore: sessionStats?.AvgNaturalness
        );
    }
}
