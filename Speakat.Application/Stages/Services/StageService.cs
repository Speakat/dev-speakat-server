using Speakat.Application.Common.Exceptions;
using Speakat.Application.Stages.Dtos;
using Speakat.Application.Stages.Repositories;
using Speakat.Domain.Enums;

namespace Speakat.Application.Stages.Services;

public class StageService : IStageService
{
    private readonly IStageRepository _stageRepository;

    public StageService(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository;
    }

    private static StageStatus ResolveStatus(bool isCompleted, bool previousCompleted) =>
        isCompleted ? StageStatus.Completed : previousCompleted ? StageStatus.Unlocked : StageStatus.Locked;

    // 유저id로 스테이지 목록 조회
    public async Task<StageListDto> GetStagesAsync(string userId)
    {
        var stages = await _stageRepository.GetStagesWithProgressAsync(userId);

        var items = new List<StageItemDto>();
        var previousCompleted = true; // 첫 스테이지 이전은 완료 처리

        foreach (var stage in stages)
        {
            var isCompleted = stage.QuestCount > 0 && stage.CompletedQuestCount == stage.QuestCount;

            var status = ResolveStatus(isCompleted, previousCompleted);

            previousCompleted = isCompleted;

            items.Add(new StageItemDto
            {
                StageId = stage.StageId,
                Title = stage.Title,
                Description = stage.Description,
                Status = status,
                QuestCount = stage.QuestCount,
                CompletedQuestCount = stage.CompletedQuestCount
            });
        }

        return new StageListDto { Items = items };
    }

    public async Task<StageDetailDto> GetStageAsync(long stageId, string userId)
    {
        var data = await _stageRepository.GetStageDetailAsync(stageId, userId)
            ?? throw StageException.NotFound();

        var isCompleted = data.Quests.Count > 0 && data.Quests.All(q => q.IsCompleted);

        var status = ResolveStatus(isCompleted, data.PreviousStageCompleted);

        return new StageDetailDto
        {
            StageId = data.StageId,
            Title = data.Title,
            Description = data.Description,
            Status = status,
            Quests = data.Quests.Select(q => new QuestItemDto
            {
                QuestId = q.QuestId,
                Title = q.Title,
                Description = q.Description,
                SortOrder = q.SortOrder,
                IsCompleted = q.IsCompleted,
                AttemptCount = q.AttemptCount
            }).ToList()
        };
    }
}
