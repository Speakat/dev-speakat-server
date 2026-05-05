public interface IQuestSessionService
{
    Task<string> CreateSessionAsync(long userId, long questId);
}
