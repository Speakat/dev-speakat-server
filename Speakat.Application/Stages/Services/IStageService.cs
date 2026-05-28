using Speakat.Application.Stages.Dtos;

namespace Speakat.Application.Stages.Services;

public interface IStageService
{
    Task<StageListDto> GetStagesAsync(string userUuid);
    Task<StageDetailDto> GetStageAsync(long stageId, string userUuid);
}
