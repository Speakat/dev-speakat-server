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
}
