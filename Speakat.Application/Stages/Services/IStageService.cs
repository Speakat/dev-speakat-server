using Speakat.Application.Stages.Dtos;

namespace Speakat.Application.Stages.Services;

public interface IStageService
{
    Task<StageListDto> GetStagesAsync(long userId);
    Task<StageDetailDto> GetStageAsync(long stageId, long userId);
}
