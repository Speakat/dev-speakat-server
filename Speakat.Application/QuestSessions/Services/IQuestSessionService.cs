public interface IQuestSessionService
{
    Task<CreateSessionResponseDto> CreateSessionAsync(long userId, long questId);
}
