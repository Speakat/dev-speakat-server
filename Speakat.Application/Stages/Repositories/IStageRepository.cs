namespace Speakat.Application.Stages.Repositories;

public record StageProgressData(
    long StageId,
    string Title,
    string Description,
    int QuestCount,
    int CompletedQuestCount
);

public record QuestProgressData(
    long QuestId,
    string Title,
    string Description,
    int SortOrder,
    int AttemptCount,
    bool IsCompleted
);

public record StageDetailData(
    long StageId,
    string Title,
    string Description,
    bool PreviousStageCompleted,
    IReadOnlyList<QuestProgressData> Quests
);

public interface IStageRepository
{
    Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(long userId);
    Task<StageDetailData?> GetStageDetailAsync(long stageId, long userId);
}
