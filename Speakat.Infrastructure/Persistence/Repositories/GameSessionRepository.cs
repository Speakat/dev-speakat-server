using Microsoft.EntityFrameworkCore;
using Speakat.Application.Common.Interfaces;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class GameSessionRepository(AppDbContext db) : IGameSessionRepository
{
    public async Task CreateAsync(string sessionId, long userId, long questId)
    {
        db.GameSessions.Add(new GameSession
        {
            SessionId = sessionId,
            UserId    = userId,
            QuestId   = questId,
            Status    = "IN_PROGRESS",
        });
        await db.SaveChangesAsync();
    }

    public async Task CompleteAsync(string sessionId, long questId, QuestResult questResult)
    {
        var session = await db.GameSessions.FindAsync(sessionId)
            ?? throw new InvalidOperationException($"GameSession {sessionId} 없음");

        session.Status           = "COMPLETED";
        session.EndedAt          = DateTime.UtcNow;
        session.SemanticScore    = (int)Math.Round(questResult.AverageContextRelevance    * 100);
        session.GrammarScore     = (int)Math.Round(questResult.AverageGrammarAccuracy     * 100);
        session.NaturalnessScore = (int)Math.Round(questResult.AverageExpressionQuality   * 100);

        var questObjectives = await db.QuestObjectives
            .Include(qo => qo.Objective)
            .Where(qo => qo.QuestId == questId)
            .ToListAsync();

        db.SessionObjectives.AddRange(questObjectives.Select(qo => new SessionObjective
        {
            SessionId        = sessionId,
            QuestObjectiveId = qo.QuestObjectiveId,
            IsAchieved       = questResult.AchievedObjectives.Contains(qo.Objective!.Name)
        }));

        await db.SaveChangesAsync();
    }

    public async Task FailedAsync(string sessionId, long questId, QuestResult questResult)
    {
        var session = await db.GameSessions.FindAsync(sessionId)
            ?? throw new InvalidOperationException($"GameSession {sessionId} 없음");

        session.Status =  "FAILED";
        session.EndedAt = DateTime.UtcNow;
        session.SemanticScore    = (int)Math.Round(questResult.AverageContextRelevance    * 100);
        session.GrammarScore     = (int)Math.Round(questResult.AverageGrammarAccuracy     * 100);
        session.NaturalnessScore = (int)Math.Round(questResult.AverageExpressionQuality   * 100);

        var questObjectives = await db.QuestObjectives
            .Include(qo => qo.Objective)
            .Where(qo => qo.QuestId == questId)
            .ToListAsync();

        db.SessionObjectives.AddRange(questObjectives.Select(qo => new SessionObjective
        {
            SessionId        = sessionId,
            QuestObjectiveId = qo.QuestObjectiveId,
            IsAchieved       = questResult.AchievedObjectives.Contains(qo.Objective!.Name)
        }));

        await db.SaveChangesAsync();
    }
}
