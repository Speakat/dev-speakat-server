public interface IUserStageRepository
{
    Task EnsureStartedAsync(long userId, long questId);
    Task TryCompleteAsync(long userId, long questId);
}
