namespace Speakat.Application.Common.Interfaces;

public interface ISessionStore
{
    Task SaveAsync(string sessionId, long userId, long questId);
    Task<(long UserId, long QuestId)?> GetAsync(string sessionId);
    Task DeleteAsync(string sessionId);
}
