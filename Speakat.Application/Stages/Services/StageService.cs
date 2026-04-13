using Speakat.Application.Common.Exceptions;
using Speakat.Application.Stages.Dtos;
using Speakat.Application.Stages.Repositories;

namespace Speakat.Application.Stages.Services;

public class StageService : IStageService
{
    private readonly IStageRepository _stageRepository;

    public StageService(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository;
    }

    private static string ResolveStatus(bool isCompleted, bool previousCompleted) =>
        isCompleted ? "COMPLETED" : previousCompleted ? "UNLOCKED" : "LOCKED";

    // 유저id로 스테이지 목록 조회
    public async Task<StageListDto> GetStagesAsync(long userId)
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
}
