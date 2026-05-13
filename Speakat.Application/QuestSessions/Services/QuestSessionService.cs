using Speakat.Application.Common.Interfaces;

public class QuestSessionService(ISessionStore store, IQuestDataService questData) : IQuestSessionService
{
    public async Task<CreateSessionResponseDto> CreateSessionAsync(long userId, long questId)
    {
        var sessionId = Guid.NewGuid().ToString();
        await store.SaveAsync(sessionId, userId, questId);
        var openingLine = await questData.GetOpeningLineAsync((int)questId);
        return new CreateSessionResponseDto(sessionId, openingLine);
    }
}
