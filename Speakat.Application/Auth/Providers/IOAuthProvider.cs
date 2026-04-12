using Speakat.Domain.Enums;

namespace Speakat.Application.Auth.Providers;

public interface IOAuthProvider
{
    SocialType ProviderType { get; }
 
    // 인가 코드 -> 사용자 정보
    public Task<OAuthUserInfo> GetUserInfoAsync(string authorizationCode);
}