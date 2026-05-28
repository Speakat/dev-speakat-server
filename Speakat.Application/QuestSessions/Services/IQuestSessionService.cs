public interface IQuestSessionService
{
    Task<CreateSessionResponseDto> CreateSessionAsync(long userId, long questId);
    Task<EndSessionResponseDto> EndSessionAsync(string sessionId, long userId);
}
