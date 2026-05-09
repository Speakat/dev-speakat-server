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

    public async Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(string userUuid)
    {
        var userId = await FindUserIdAsync(userUuid) ?? 0;

        return await _context.Stages
            .OrderBy(s => s.SortOrder)
            .Select(s => new StageProgressData(
                s.StageId,
                s.Title,
                s.Description,
                _context.Quests.Count(q => q.StageId == s.StageId),
                0
            ))
            .ToListAsync();
    }

    public async Task<StageDetailData?> GetStageDetailAsync(long stageId, string userUuid)
    {
        var userId = await FindUserIdAsync(userUuid) ?? 0;

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
            var prevCompletedCount = 0;
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
                0,
                false
            ))
            .ToListAsync();

        return new StageDetailData(stage.StageId, stage.Title, stage.Description, previousStageCompleted, quests);
    }

    private async Task<long?> FindUserIdAsync(string userUuid)
    {
        return await _context.Users
            .Where(u => u.UserUuid == userUuid)
            .Select(u => (long?)u.UserId)
            .FirstOrDefaultAsync();
    }
}
