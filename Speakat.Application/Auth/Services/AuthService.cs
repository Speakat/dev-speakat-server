using Microsoft.Extensions.Logging;
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
    private const string DefaultProfileImageKey = "profile-images/default.jpg";

    private readonly IEnumerable<IOAuthProvider> _oauthProviders;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IEnumerable<IOAuthProvider> oauthProviders,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenStore refreshTokenStore,
        IImageStorageService imageStorageService,
        ILogger<AuthService> logger)
    {
        _oauthProviders = oauthProviders;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenStore = refreshTokenStore;
        _imageStorageService = imageStorageService;
        _logger = logger;
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
            var profileKey = DefaultProfileImageKey;
            if (!string.IsNullOrEmpty(userInfo.ProfileImageUrl))
            {
                try
                {
                    var key = ProfileImgKey(user.UserUuid);
                    await _imageStorageService.UploadFromUrlAsync(userInfo.ProfileImageUrl, key);
                    profileKey = key;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "{Provider} 프로필 이미지 S3 업로드 실패, 기본 이미지 적용 (userUuid: {UserUuid})", provider, user.UserUuid);
                }
            }
            await _userRepository.UpdateProfileImageKeyAsync(user.UserId, profileKey);
            user.ProfileImageKey = profileKey;
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
            ProfileImageUrl = user!.ProfileImageKey is not null 
                ? _imageStorageService.GetPublicUrl(user.ProfileImageKey) : null,
            Provider = user.SocialType,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IsNewUser = isNewUser
        };
    }

    public async Task<RefreshTokenResponseDto> RefreshAsync(string refreshToken)
    {
        if (await _refreshTokenStore.IsBlacklistedAsync(refreshToken))
            throw AuthException.BlacklistedToken();
        
        // Redis에서 userUuid 조회
        var userUuid = await _refreshTokenStore.GetUserUuidAsync(refreshToken) ?? throw AuthException.RefreshTokenExpired();
        
        // 새 토큰 발급
        var newAccessToken = _jwtTokenService.GenerateAccessToken(userUuid);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        
        // 기존 토큰 삭제 후 새 토큰 저장
        await _refreshTokenStore.DeleteAsync(refreshToken);
        await _refreshTokenStore.SaveAsync(newRefreshToken,
            userUuid, RefreshTokenTtl);
        
        return new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        await _refreshTokenStore.BlacklistAsync(refreshToken);
        await _refreshTokenStore.DeleteAsync(refreshToken);
    }

    private static string ProfileImgKey(string userUuid) => $"profile-images/{userUuid}.jpg";

    public async Task<CheckNicknameResponseDto> CheckNicknameAsync(string nickname)
    {
        var exists = await _userRepository.ExistsNicknameAsync(nickname);
        
        if (!exists)
            return new CheckNicknameResponseDto { Available = true };

        var suggestion = $"{nickname}_{Random.Shared.Next(10, 10000)}";
        while(await _userRepository.ExistsNicknameAsync(suggestion))
            suggestion = $"{nickname}_{Random.Shared.Next(10, 10000)}";
        return new CheckNicknameResponseDto { Available = false, Suggestion = suggestion };
    }
}