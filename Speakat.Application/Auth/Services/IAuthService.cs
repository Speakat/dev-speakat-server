using Speakat.Application.Auth.Commands;
using Speakat.Domain.Enums;

namespace Speakat.Application.Auth.Services;

public interface IAuthService
{
    // OAuth 로그인/회원가입 통합 처리
    Task<OAuthLoginResponseDto> OAuthLoginAsync(SocialType provider, string authorizationCode);
}
