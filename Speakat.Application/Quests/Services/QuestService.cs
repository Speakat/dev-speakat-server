using Speakat.Application.Common.Exceptions;
using Speakat.Application.Quests.Dtos;
using Speakat.Application.Quests.Repositories;
using Speakat.Domain.Enums;

namespace Speakat.Application.Quests.Services;

public class QuestService : IQuestService
{
    private readonly IQuestRepository _questRepository;

    public QuestService(IQuestRepository questRepository)
    {
        _questRepository = questRepository;
    }
    
    private static QuestStatus ResolveStatus(bool isCompleted, bool previousCompleted) =>
        isCompleted ? QuestStatus.Completed : previousCompleted ? QuestStatus.InProgress : QuestStatus.Locked;
    
    public async Task<QuestDetailDto> GetQuestDetailAsync(long questId, string userUuid)
    {
        var data = await _questRepository.GetQuestDetailAsync(questId, userUuid) ?? throw QuestException.NotFound();

        var status = ResolveStatus(data.IsCompleted, data.PreviousQuestCompleted);

        return new QuestDetailDto
        {
            QuestId = data.QuestId,
            StageId = data.StageId,
            Title = data.Title,
            Description = data.Description,
            ThumbnailUrl = data.ThumbnailUrl,
            Objectives = data.Objectives,
            Status = status,
            BestScore = data.BestScore,
            AttemptCount = data.AttemptCount
        };
    }
}