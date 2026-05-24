using Microsoft.EntityFrameworkCore;
using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class GameSessionRepository(AppDbContext db) : IGameSessionRepository
{
    public async Task CreateAsync(string sessionId, long userId, long questId)
    {
        var existingSessions = await db.GameSessions
            .Where(gs => gs.UserId == userId && gs.QuestId == questId && gs.Status == "IN_PROGRESS")
            .ToListAsync();

        foreach (var existing in existingSessions)
        {
            existing.Status  = "ABANDONED";
            existing.EndedAt = DateTime.UtcNow;
        }

        db.GameSessions.Add(new GameSession
        {
            SessionId = sessionId,
            UserId    = userId,
            QuestId   = questId,
            Status    = "IN_PROGRESS",
        });
        await db.SaveChangesAsync();
    }

    public async Task<EndSessionResponseDto> AbandonAsync(string sessionId, long userId)
    {
        var session = await db.GameSessions.FindAsync(sessionId)
            ?? throw SessionException.NotFound();

        if (session.UserId != userId)
            throw SessionException.Forbidden();

        if (session.Status != "IN_PROGRESS")
            throw SessionException.AlreadyEnded();

        session.Status  = "FAILED";
        session.EndedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new EndSessionResponseDto(session.SessionId, session.Status, session.EndedAt.Value);
    }

    public Task CompleteAsync(string sessionId, long questId, QuestResult questResult) =>
        FinalizeAsync(sessionId, questId, questResult, "COMPLETED");

    public Task FailedAsync(string sessionId, long questId, QuestResult questResult) =>
        FinalizeAsync(sessionId, questId, questResult, "FAILED");

    private async Task FinalizeAsync(string sessionId, long questId, QuestResult questResult, string status)
    {
        var session = await db.GameSessions.FindAsync(sessionId)
            ?? throw SessionException.NotFound();

        if (session.Status != "IN_PROGRESS")
            throw SessionException.AlreadyEnded();

        session.Status           = status;
        session.EndedAt          = DateTime.UtcNow;
        session.SemanticScore    = (int)Math.Round(questResult.AverageContextRelevance  * 100);
        session.GrammarScore     = (int)Math.Round(questResult.AverageGrammarAccuracy   * 100);
        session.NaturalnessScore = (int)Math.Round(questResult.AverageExpressionQuality * 100);

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
