using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Quests.Dtos;
using Speakat.Application.Quests.Services;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("quests")]
[Authorize]
public class QuestController : ControllerBase
{
    private readonly IQuestService _questService;

    public QuestController(IQuestService questService)
        => _questService = questService;

    private string UserUuid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{questId}")]
    public async Task<ActionResult<ApiResponse<QuestDetailDto>>> GetQuestDetailAsync(
        [FromRoute] long questId)
    {
        var result = await _questService.GetQuestDetailAsync(questId, UserUuid);
        return Ok(ApiResponse<QuestDetailDto>.Success(result));
    }
}
