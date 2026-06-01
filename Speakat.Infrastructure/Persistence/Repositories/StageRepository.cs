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
            .ToHashSetAsync();

        var questsByStage = await _context.Quests
            .GroupBy(q => q.StageId)
            .Select(g => new { StageId = g.Key, QuestIds = g.Select(q => q.QuestId).ToList() })
            .ToListAsync();

        var questsByStageDict = questsByStage.ToDictionary(x => x.StageId, x => x.QuestIds);

        var stages = await _context.Stages
            .OrderBy(s => s.SortOrder)
            .Select(s => new { s.StageId, s.Title, s.Description })
            .ToListAsync();

        return stages.Select(s =>
        {
            var questIds = questsByStageDict.GetValueOrDefault(s.StageId, []);
            return new StageProgressData(
                s.StageId,
                s.Title,
                s.Description,
                questIds.Count,
                questIds.Count(id => completedQuestIds.Contains(id))
            );
        }).ToList();
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
            var prevQuestIds = await _context.Quests
                .Where(q => q.StageId == previousStage.StageId)
                .Select(q => q.QuestId)
                .ToListAsync();

            var prevCompletedCount = await _context.GameSessions
                .Where(gs => gs.UserId == userId && gs.Status == CompletedStatus && prevQuestIds.Contains(gs.QuestId))
                .Select(gs => gs.QuestId)
                .Distinct()
                .CountAsync();

            previousStageCompleted = prevQuestIds.Count > 0 && prevCompletedCount == prevQuestIds.Count;
        }

        var questData = await _context.Quests
            .Where(q => q.StageId == stageId)
            .OrderBy(q => q.SortOrder)
            .Select(q => new { q.QuestId, q.Title, q.Description, q.SortOrder })
            .ToListAsync();

        var questIds = questData.Select(q => q.QuestId).ToList();

        var sessions = await _context.GameSessions
            .Where(gs => gs.UserId == userId && questIds.Contains(gs.QuestId))
            .Select(gs => new { gs.QuestId, gs.Status })
            .ToListAsync();

        var sessionsByQuest = sessions
            .GroupBy(gs => gs.QuestId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var quests = questData.Select(q =>
        {
            var questSessions = sessionsByQuest.GetValueOrDefault(q.QuestId, []);
            return new QuestProgressData(
                q.QuestId,
                q.Title,
                q.Description,
                q.SortOrder,
                questSessions.Count,
                questSessions.Any(s => s.Status == CompletedStatus)
            );
        }).ToList();

        return new StageDetailData(stage.StageId, stage.Title, stage.Description, previousStageCompleted, quests);
    }
}
