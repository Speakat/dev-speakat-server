namespace Speakat.Application.Stages.Repositories;

public record StageProgressData(
    long StageId,
    string Title,
    string Description,
    int QuestCount,
    int CompletedQuestCount
);

public interface IStageRepository
{
    Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(long userId);
}
