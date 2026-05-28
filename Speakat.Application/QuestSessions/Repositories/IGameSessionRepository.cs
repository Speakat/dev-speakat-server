public interface IGameSessionRepository
{
    Task<IReadOnlyList<string>> CreateAsync(string sessionId, long userId, long questId);
    Task CompleteAsync(string sessionId, long questId, QuestResult questResult);
    Task FailedAsync(string sessionId, long questId, QuestResult questResult);
    Task<EndSessionResponseDto> AbandonAsync(string sessionId, long userId);
}
