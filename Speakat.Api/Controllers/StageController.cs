using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Stages.Dtos;
using Speakat.Application.Stages.Services;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("stages")]
[Authorize]
public class StageController : ControllerBase
{
    private readonly IStageService _stageService;

    public StageController(IStageService stageService)
    {
        _stageService = stageService;
    }

    private long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Stage 목록 조회
    [HttpGet]
    public async Task<ActionResult<ApiResponse<StageListDto>>> GetStagesAsync()
    {
        var result = await _stageService.GetStagesAsync(UserId);
        return Ok(ApiResponse<StageListDto>.Success(result));
    }

    [HttpGet("{stageId}")]
    public async Task<ActionResult<ApiResponse<StageDetailDto>>> GetStageAsync([FromRoute] long stageId)
    {
        try
        {
            var result = await _stageService.GetStageAsync(stageId, UserId);
            return Ok(ApiResponse<StageDetailDto>.Success(result));
        }
        catch (StageException ex)
        {
            return StatusCode(ex.StatusCode, ApiResponse<StageDetailDto>.Fail(ex.Code, ex.Message!));
        }
    }
}
