using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("sessions")]
[Authorize]
public class QuestSessionController : ControllerBase
{
    private readonly IQuestSessionService _sessionService;

    public QuestSessionController(IQuestSessionService sessionService)
        => _sessionService = sessionService;

    private long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateSessionResponseDto>>> CreateSessionAsync(
        [FromBody] CreateSessionRequestDto request)
    {
        var result = await _sessionService.CreateSessionAsync(UserId, request.QuestId);
        return Ok(ApiResponse<CreateSessionResponseDto>.Success(result));
    }

    [HttpPost("{session_id}/end")]
    public async Task<ActionResult<ApiResponse<EndSessionResponseDto>>> EndSessionAsync(
        [FromRoute(Name = "session_id")] string sessionId)
    {
        var result = await _sessionService.EndSessionAsync(sessionId, UserId);
        return Ok(ApiResponse<EndSessionResponseDto>.Success(result));
    }
}
