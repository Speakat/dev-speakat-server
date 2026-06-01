using Microsoft.EntityFrameworkCore;
using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Stages.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class StageRepository : IStageRepository
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;
    private const string CompletedStatus = "COMPLETED";

    public StageRepository(AppDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(string userUuid)
    {
        var userId = await _userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var completedQuestIds = await _context.GameSessions
            .Where(gs => gs.UserId == userId && gs.Status == CompletedStatus)
            .Select(gs => gs.QuestId)
            .Distinct()
            .ToListAsync();

        return await _context.Stages
            .OrderBy(s => s.SortOrder)
            .Select(s => new StageProgressData(
                s.StageId,
                s.Title,
                s.Description,
                _context.Quests.Count(q => q.StageId == s.StageId),
                _context.Quests.Count(q => q.StageId == s.StageId && completedQuestIds.Contains(q.QuestId))
            ))
            .ToListAsync();
    }

    public async Task<StageDetailData?> GetStageDetailAsync(long stageId, string userUuid)
    {
        var userId = await _userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var stage = await _context.Stages.FindAsync(stageId);
        if (stage == null) return null;

        var previousStage = await _context.Stages
            .Where(s => s.SortOrder < stage.SortOrder)
            .OrderByDescending(s => s.SortOrder)
            .FirstOrDefaultAsync();

        bool previousStageCompleted;
        if (previousStage == null)
        {
            previousStageCompleted = true;
        }
        else
        {
            var prevQuestCount = await _context.Quests.CountAsync(q => q.StageId == previousStage.StageId);
            var prevCompletedCount = await _context.GameSessions
                .Where(gs => gs.UserId == userId && gs.Status == CompletedStatus &&
                             _context.Quests.Any(q => q.QuestId == gs.QuestId && q.StageId == previousStage.StageId))
                .Select(gs => gs.QuestId)
                .Distinct()
                .CountAsync();

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
                _context.GameSessions.Count(gs => gs.UserId == userId && gs.QuestId == q.QuestId),
                _context.GameSessions.Any(gs => gs.UserId == userId && gs.QuestId == q.QuestId && gs.Status == CompletedStatus)
            ))
            .ToListAsync();

        return new StageDetailData(stage.StageId, stage.Title, stage.Description, previousStageCompleted, quests);
    }
}
