using Speakat.Application.Auth.Dtos;
using Speakat.Application.Auth.Providers;
using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;
using Speakat.Domain.Entities;
using Speakat.Domain.Enums;

namespace Speakat.Application.Auth.Services;

public class AuthService : IAuthService
{
    private static readonly TimeSpan RefreshTokenTtl = TimeSpan.FromDays(14);

    private readonly IEnumerable<IOAuthProvider> _oauthProviders;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public AuthService(
        IEnumerable<IOAuthProvider> oauthProviders,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenStore refreshTokenStore)
    {
        _oauthProviders = oauthProviders;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<OAuthLoginResponseDto> OAuthLoginAsync(SocialType provider, string authorizationCode)
    {
        var oauthProvider = _oauthProviders.FirstOrDefault(p => p.ProviderType == provider)
            ?? throw AuthException.UnsupportedProvider();

        // 사용자 정보 조회
        var userInfo = await oauthProvider.GetUserInfoAsync(authorizationCode);

        // 기존 회원 여부 확인
        var user = await _userRepository.FindBySocialTypeAndSocialIdAsync(provider, userInfo.SocialId);

        // 신규 회원이면 User 엔티티 생성 후 저장 
        var isNewUser = user is null;
        if (isNewUser)
        {
            user = new User
            {
                SocialType = provider,
                SocialId = userInfo.SocialId,
                Nickname = userInfo.Nickname,
                Email = userInfo.Email,
                Gender = userInfo.Gender,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user = await _userRepository.SaveAsync(user);
        }

        // JWT 발급
        var accessToken = _jwtTokenService.GenerateAccessToken(user!.UserUuid);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Redis에 저장
        await _refreshTokenStore.SaveAsync(refreshToken, user.UserUuid, RefreshTokenTtl);

        return new OAuthLoginResponseDto
        {
            UserUuid = user.UserUuid,
            Email = user.Email,
            Nickname = user.Nickname,
            // TODO: 이미지 업로드 구현 후 ProfileImageKey 변환
            ProfileImageUrl = null,
            Provider = user.SocialType,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IsNewUser = isNewUser
        };
    }
}
