using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Auth.Dtos;
using Speakat.Application.Auth.Services;
using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Enums;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // OAuth 로그인 / 회원가입
    [HttpPost("oauth/{provider}")]
    public async Task<ActionResult<ApiResponse<OAuthLoginResponseDto>>> OAuthLoginAsync(
        [FromRoute] string provider,
        [FromBody] OAuthLoginRequestDto request)
    {
        // SocialType enum으로 변환
        if (!Enum.TryParse<SocialType>(provider, ignoreCase: true, out var socialType))
            return BadRequest(ApiResponse<OAuthLoginResponseDto>.Fail(
                "UNSUPPORTED_OAUTH_PROVIDER",
                $"지원하지 않는 OAuth Provider: {provider}"));

        try
        {
            var result = await _authService.OAuthLoginAsync(socialType, request.AuthorizationCode);
            return Ok(ApiResponse<OAuthLoginResponseDto>.Success(result));
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, ApiResponse<OAuthLoginResponseDto>.Fail(ex.Code, ex.Message!));
        }
    }
}
