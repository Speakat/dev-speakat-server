using Microsoft.EntityFrameworkCore;
using Speakat.Application.Stages.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class StageRepository : IStageRepository
{
    private readonly AppDbContext _context;

    public StageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(string userId)
    {
        return await _context.Stages
            .OrderBy(s => s.SortOrder)
            .Select(s => new StageProgressData(
                s.StageId,
                s.Title,
                s.Description,
                _context.Quests.Count(q => q.StageId == s.StageId),
                _context.Quests.Count(q => q.StageId == s.StageId &&
                    _context.UserQuests.Any(uq => uq.QuestId == q.QuestId
                        && uq.UserId == userId
                        && uq.CompletedAt != null))
            ))
            .ToListAsync();
    }

    public async Task<StageDetailData?> GetStageDetailAsync(long stageId, string userId)
    {
        var stage = await _context.Stages.FindAsync(stageId);
        if (stage == null) return null;

        // 이전 스테이지 완료 여부 확인
        var previousStage = await _context.Stages
            .Where(s => s.SortOrder < stage.SortOrder)
            .OrderByDescending(s => s.SortOrder)
            .FirstOrDefaultAsync();

        bool previousStageCompleted;
        if (previousStage == null)
        {
            previousStageCompleted = true; // 첫 스테이지
        }
        else
        {
            var prevQuestCount = await _context.Quests.CountAsync(q => q.StageId == previousStage.StageId);
            var prevCompletedCount = await _context.Quests.CountAsync(q => q.StageId == previousStage.StageId &&
                _context.UserQuests.Any(uq => uq.QuestId == q.QuestId && uq.UserId == userId && uq.CompletedAt != null));
            previousStageCompleted = prevQuestCount > 0 && prevCompletedCount == prevQuestCount;
        }

        var quests = await _context.Quests
            .Where(q => q.StageId == stageId)
            .OrderBy(q => q.SortOrder)
            .Select(q => new QuestProgressData(
                q.QuestId,
                q.Title,
                q.Description,
                q.SortOrder,
                _context.UserQuests.Count(uq => uq.QuestId == q.QuestId && uq.UserId == userId),
                _context.UserQuests.Any(uq => uq.QuestId == q.QuestId && uq.UserId == userId && uq.CompletedAt != null)
            ))
            .ToListAsync();

        return new StageDetailData(stage.StageId, stage.Title, stage.Description, previousStageCompleted, quests);
    }
}
