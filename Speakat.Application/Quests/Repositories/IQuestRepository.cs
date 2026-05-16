using Speakat.Application.Quests.Dtos;

namespace Speakat.Application.Quests.Repositories;

public record QuestDetailData(
    long QuestId,
    long StageId,
    string Title,
    string Description,
    string ThumbnailUrl,
    IReadOnlyList<string> Objectives,
    bool IsCompleted,
    bool PreviousQuestCompleted,
    int? BestScore,
    int AttemptCount
);

public interface IQuestRepository
{
    Task<QuestDetailData?> GetQuestDetailAsync(long questId, string userUuid);
}