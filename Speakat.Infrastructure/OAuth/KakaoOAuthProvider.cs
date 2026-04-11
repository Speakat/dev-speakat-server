using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Speakat.Application.Auth.Interfaces;
using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Enums;

namespace Speakat.Infrastructure.OAuth;

public class KakaoOAuthProvider : IOAuthProvider
{
    // Kakao OAuth 2.0
    private const string TokenEndpoint = "https://kauth.kakao.com/oauth/token";
    private const string UserInfoEndpoint = "https://kapi.kakao.com/v2/user/me";

    // 클라이언트
    private readonly HttpClient _httpClient;
    private readonly ILogger<KakaoOAuthProvider> _logger;

    // 클라이언트 자격증명
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public SocialType ProviderType => SocialType.Kakao;

    public KakaoOAuthProvider(HttpClient httpClient, IConfiguration configuration, ILogger<KakaoOAuthProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _clientId = configuration["OAuth:Kakao:ClientId"]
            ?? throw new InvalidOperationException("OAuth:Kakao:ClientId is not configured.");
        _clientSecret = configuration["OAuth:Kakao:ClientSecret"]
            ?? throw new InvalidOperationException("OAuth:Kakao:ClientSecret is not configured.");
        _redirectUri = configuration["OAuth:Kakao:RedirectUri"]
            ?? throw new InvalidOperationException("OAuth:Kakao:RedirectUri is not configured.");
    }

    // authorizationCode를 받아 OAuthUserInfo를 반환
    public async Task<OAuthUserInfo> GetUserInfoAsync(string authorizationCode)
    {
        // authorizationCode -> accessToken
        var accessToken = await FetchAccessTokenAsync(authorizationCode);

        // accessToken -> 사용자 정보
        return await FetchUserInfoAsync(accessToken);
    }

    // access_token 획득
    private async Task<string> FetchAccessTokenAsync(string authorizationCode)
    {
        var parameters = new Dictionary<string, string>
        {
            ["code"] = authorizationCode,
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["redirect_uri"] = _redirectUri,
            ["grant_type"] = "authorization_code"
        };

        using var response = await _httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Kakao Token 요청 실패 | StatusCode: {StatusCode} | Body: {Body}", response.StatusCode, errorBody);
            throw AuthException.OAuthFailed();
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<KakaoTokenResponse>(json)
            ?? throw AuthException.OAuthFailed();

        return tokenResponse.AccessToken;
    }

    // GET 사용자 프로필
    private async Task<OAuthUserInfo> FetchUserInfoAsync(string accessToken)
    {
        var propertyKeys = JsonSerializer.Serialize(new[]
        {
            "kakao_account.profile",
            "kakao_account.email",
            "kakao_account.gender"
        });

        var url = $"{UserInfoEndpoint}?property_keys={Uri.EscapeDataString(propertyKeys)}";

        // Authorization 헤더를 추가
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Kakao UserInfo 요청 실패 | StatusCode: {StatusCode} | Body: {Body}", response.StatusCode, errorBody);
            throw AuthException.OAuthFailed();
        }

        var json = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<KakaoUserInfoResponse>(json)
            ?? throw AuthException.OAuthFailed();

        return new OAuthUserInfo
        {
            SocialId = userInfo.Id.ToString(),
            Email = userInfo.KakaoAccount?.Email,
            // Nickname 값으로 Nickname -> email -> id 설정 시도
            Nickname = userInfo.KakaoAccount?.Profile?.Nickname
                       ?? userInfo.KakaoAccount?.Email
                       ?? userInfo.Id.ToString(),
            ProfileImageUrl = userInfo.KakaoAccount?.Profile?.ProfileImageUrl ?? string.Empty,
            Gender = userInfo.KakaoAccount?.Gender switch
            {
                "male" => Gender.Male,
                "female" => Gender.Female,
                _ => Gender.Other
            }
        };
    }

    // JSON의 snake_case 키를 C# PascalCase 프로퍼티 로 매핑
    private sealed class KakaoTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = null!;
    }

    private sealed class KakaoUserInfoResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("kakao_account")]
        public KakaoAccount? KakaoAccount { get; init; }
    }

    private sealed class KakaoAccount
    {
        [JsonPropertyName("email")]
        public string? Email { get; init; }

        [JsonPropertyName("gender")]
        public string? Gender { get; init; }

        [JsonPropertyName("profile")]
        public KakaoProfile? Profile { get; init; }
    }

    private sealed class KakaoProfile
    {
        [JsonPropertyName("nickname")]
        public string? Nickname { get; init; }

        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; init; }
    }
}
