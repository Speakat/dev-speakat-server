using Speakat.Application.Common.Interfaces;

public class QuestSessionService : IQuestSessionService
{
    private readonly ISessionStore _store;

    public QuestSessionService(ISessionStore store) => _store = store;

    public async Task<string> CreateSessionAsync(long userId, long questId)
    {
        var sessionId = Guid.NewGuid().ToString();
        await _store.SaveAsync(sessionId, userId, questId);
        return sessionId;
    }
}
