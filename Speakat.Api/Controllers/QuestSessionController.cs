using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("sessions")]
[Authorize]
public class QuestSessionController(IQuestSessionService sessionService, IUserRepository userRepo) : ControllerBase
{
    private string UserUuid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateSessionResponseDto>>> CreateSessionAsync(
        [FromBody] CreateSessionRequestDto request)
    {
        var user = await userRepo.FindByUuidAsync(UserUuid)
            ?? throw new NotFoundException("유저를 찾을 수 없습니다.");
        var result = await sessionService.CreateSessionAsync(user.UserId, request.QuestId);
        return Ok(ApiResponse<CreateSessionResponseDto>.Success(result));
    }

    [HttpPost("{session_id}/end")]
    public async Task<ActionResult<ApiResponse<EndSessionResponseDto>>> EndSessionAsync(
        [FromRoute(Name = "session_id")] string sessionId)
    {
        var user = await userRepo.FindByUuidAsync(UserUuid)
            ?? throw new NotFoundException("유저를 찾을 수 없습니다.");
        var result = await sessionService.EndSessionAsync(sessionId, user.UserId);
        return Ok(ApiResponse<EndSessionResponseDto>.Success(result));
    }
}
