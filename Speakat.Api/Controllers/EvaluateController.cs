using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("/sessions/{session_id}/speech")]
[Authorize]
public class EvaluateController(IEvaluateService evaluateService) : ControllerBase
{
    private long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EvaluateResponseDto>>> EvaluateAsync(
        [FromRoute(Name = "session_id")] string sessionId,
        [FromBody] EvaluateRequestDto request)
    {
        var result = await evaluateService.EvaluateAsync(request, sessionId, UserId);
        return Ok(ApiResponse<EvaluateResponseDto>.Success(result));
    }
}
