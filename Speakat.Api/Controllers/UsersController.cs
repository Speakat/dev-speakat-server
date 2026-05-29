using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Users.Dtos;
using Speakat.Application.Users.Services;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    private string UserUuid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfileAsync()
    {
        var result = await userService.GetProfileAsync(UserUuid);
        return Ok(ApiResponse<UserProfileDto>.Success(result));
    }

    [HttpPatch("me")]
    public async Task<ActionResult<ApiResponse<PatchUserResultDto>>> PatchProfileAsync(
        [FromBody] PatchUserRequestDto request)
    {
        var result = await userService.UpdateProfileAsync(UserUuid, request.Nickname, request.ProfileImageKey);
        return Ok(ApiResponse<PatchUserResultDto>.Success(result));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        await userService.DeleteAccountAsync(UserUuid);
        return NoContent();
    }

    [HttpGet("me/settings")]
    public async Task<ActionResult<ApiResponse<UserSettingsDto>>> GetSettingsAsync()
    {
        var result = await userService.GetSettingsAsync(UserUuid);
        return Ok(ApiResponse<UserSettingsDto>.Success(result));
    }

    [HttpGet("me/stats")]
    public async Task<ActionResult<ApiResponse<UserStatsDto>>> GetStatsAsync()
    {
        var result = await userService.GetStatsAsync(UserUuid);
        return Ok(ApiResponse<UserStatsDto>.Success(result));
    }

    [HttpGet("me/streak")]
    public async Task<ActionResult<ApiResponse<UserStreakDto>>> GetStreakAsync()
    {
        var result = await userService.GetStreakAsync(UserUuid);
        return Ok(ApiResponse<UserStreakDto>.Success(result));
    }

    [HttpGet("me/calendar")]
    public async Task<ActionResult<ApiResponse<UserCalendarDto>>> GetCalendarAsync(
        [FromQuery] int? year,
        [FromQuery] int? month)
    {
        var today = DateTime.UtcNow;
        var y = year ?? today.Year;
        var m = month ?? today.Month;

        if (m < 1 || m > 12 || y < 1)
            return BadRequest(ApiResponse<UserCalendarDto>.Fail("INVALID_DATE", "유효하지 않은 연도/월"));

        var result = await userService.GetCalendarAsync(UserUuid, y, m);
        return Ok(ApiResponse<UserCalendarDto>.Success(result));
    }
}
