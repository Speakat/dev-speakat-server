using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("quests/{questId}/sessions")]
[Authorize]
public class QuestSessionController : ControllerBase
{
    private readonly IQuestSessionService _sessionService;

    public QuestSessionController(IQuestSessionService sessionService)
        => _sessionService = sessionService;

    private long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateSessionResponseDto>>> CreateSessionAsync(
        [FromRoute] long questId)
    {
        var sessionId = await _sessionService.CreateSessionAsync(UserId, questId);
        return Ok(ApiResponse<CreateSessionResponseDto>.Success(new CreateSessionResponseDto(sessionId)));
    }
}
