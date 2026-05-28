using Speakat.Application.Common.Interfaces;

public class QuestSessionService(ISessionStore store, IQuestDataService questData, IGameSessionRepository gameSessionRepo, IUserStageRepository userStageRepo, IUnitOfWork unitOfWork) : IQuestSessionService
{
    public async Task<CreateSessionResponseDto> CreateSessionAsync(long userId, long questId)
    {
        var openingLine = await questData.GetOpeningLineAsync((int)questId);
        var sessionId = Guid.NewGuid().ToString();

        IReadOnlyList<string> abandonedSessionIds;
        await unitOfWork.BeginTransactionAsync();
        try
        {
            abandonedSessionIds = await gameSessionRepo.CreateAsync(sessionId, userId, questId);
            await userStageRepo.EnsureStartedAsync(userId, questId);
            await unitOfWork.CommitAsync();
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }

        foreach (var abandonedId in abandonedSessionIds)
            await store.DeleteAsync(abandonedId);

        await store.SaveAsync(sessionId, userId, questId);
        return new CreateSessionResponseDto(sessionId, openingLine);
    }

    public Task<EndSessionResponseDto> EndSessionAsync(string sessionId, long userId)
        => gameSessionRepo.AbandonAsync(sessionId, userId);
}
