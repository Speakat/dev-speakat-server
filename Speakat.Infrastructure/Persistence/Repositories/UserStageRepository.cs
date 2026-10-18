using Microsoft.EntityFrameworkCore;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserStageRepository(AppDbContext context) : IUserStageRepository
{
    public async Task EnsureStartedAsync(long userId, long questId)
    {
        var quest = await context.Quests.FindAsync(questId)
            ?? throw new InvalidOperationException($"Quest {questId} 없음");

        var exists = await context.UserStages
            .AnyAsync(us => us.UserId == userId && us.StageId == quest.StageId);

        if (!exists)
        {
            context.UserStages.Add(new UserStage
            {
                UserId = userId,
                StageId = quest.StageId,
                StartedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }

    public async Task TryCompleteAsync(long userId, long questId)
    {
        var quest = await context.Quests.FindAsync(questId)
            ?? throw new InvalidOperationException($"Quest {questId} 없음");

        var stageQuestIds = await context.Quests
            .Where(q => q.StageId == quest.StageId)
            .Select(q => q.QuestId)
            .ToListAsync();

        var completedQuestIds = await context.GameSessions
            .Where(gs => gs.UserId == userId && gs.Status == "COMPLETED" && stageQuestIds.Contains(gs.QuestId))
            .Select(gs => gs.QuestId)
            .Distinct()
            .ToListAsync();

        if (stageQuestIds.All(id => completedQuestIds.Contains(id)))
        {
            var userStage = await context.UserStages
                .FirstOrDefaultAsync(us => us.UserId == userId && us.StageId == quest.StageId);

            if (userStage is not null && userStage.CompletedAt is null)
            {
                userStage.CompletedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }
    }
}
