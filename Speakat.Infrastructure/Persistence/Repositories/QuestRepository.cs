using Microsoft.EntityFrameworkCore;
using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Application.Quests.Repositories;

public class QuestRepository : IQuestRepository
{
    private readonly AppDbContext _context;

    public QuestRepository(AppDbContext context) => _context = context;

    public async Task<QuestDetailData?> GetQuestDetailAsync(long questId, string userUuid)
    {
        var userId = await FindUserIdAsync(userUuid) ?? 0;
        
        var quest = await _context.Quests.FindAsync(questId);
        if (quest == null) return null;
        
        // 이전 Quest 완료 확인
        var previousQuest = await _context.Quests
            .Where(q => q.StageId == quest.StageId 
                        && q.SortOrder < quest.SortOrder)
            .OrderByDescending(q => q.SortOrder)
            .FirstOrDefaultAsync();

        bool previousQuestCompleted;
        if (previousQuest == null)
        {
            previousQuestCompleted = true; // 첫 번째 퀘스트
        }
        else
        {
            previousQuestCompleted = await _context.GameSessions
                .AnyAsync(s => s.QuestId == previousQuest.QuestId 
                               && s.UserId == userId 
                               && s.Status == "COMPLETED");
        }

        // BestScore, AttemptCount
        var sessions = await _context.GameSessions
            .Where(s => s.QuestId == questId 
                        && s.UserId == userId)
            .ToListAsync();

        var attemptCount = sessions.Count;
        
        var bestScore = sessions
            .Where(s => s.SemanticScore.HasValue
                        && s.GrammarScore.HasValue
                        && s.NaturalnessScore.HasValue)
            .Select(s => (s.SemanticScore!.Value 
                          + s.GrammarScore!.Value 
                          + s.NaturalnessScore!.Value) / 3)
            .Cast<int?>()
            .Max(); // 없으면 null 반환

        // Objectives
        var objectives = await _context.QuestObjectives
                .Where(qo => qo.QuestId == questId)
                .OrderBy(qo => qo.SortOrder)
                .Join(_context.Objectives, 
                    qo => qo.ObjectiveId, 
                    o => o.ObjectiveId, 
                    (qo, o) => o.Description ?? o.Name)
                .ToListAsync();
        
        var isCompleted = await _context.GameSessions
            .AnyAsync(s => s.QuestId == questId
                           && s.UserId == userId
                           && s.Status == "COMPLETED");

        return new QuestDetailData(
            quest.QuestId,
            quest.StageId,
            quest.Title,
            quest.Description,
            quest.Thumbnail,
            objectives,
            isCompleted,
            previousQuestCompleted,
            bestScore,
            attemptCount
        );
    }

    public async Task<Quest> GetByIdAsync(long questId)
    {
        return await _context.Quests.FindAsync(questId)
               ?? throw new NotFoundException($"Quest {questId} 없음");
    }

    private async Task<long?> FindUserIdAsync(string userUuid)
    {
        return await _context.Users
            .Where(u => u.UserUuid == userUuid)
            .Select(u => (long?)u.UserId)
            .FirstOrDefaultAsync();
    }
}