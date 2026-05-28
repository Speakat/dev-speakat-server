using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("sessions/{session_id}/speech")]
[Authorize]
public class EvaluateController(IEvaluateService evaluateService, IUserRepository userRepo) : ControllerBase
{
    private string UserUuid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EvaluateResponseDto>>> EvaluateAsync(
        [FromRoute(Name = "session_id")] string sessionId,
        [FromBody] EvaluateRequestDto request)
    {
        var user = await userRepo.FindByUuidAsync(UserUuid)
            ?? throw new NotFoundException("유저를 찾을 수 없습니다.");
        var result = await evaluateService.EvaluateAsync(request, sessionId, user.UserId);
        return Ok(ApiResponse<EvaluateResponseDto>.Success(result));
    }
}
