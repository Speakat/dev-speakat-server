using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Speakat.Application.Auth.Providers;
using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Enums;

namespace Speakat.Infrastructure.OAuth;

public class GoogleOAuthProvider : IOAuthProvider
{
    // Google OAuth 2.0
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

    // 클라이언트
    private readonly HttpClient _httpClient;

    // 클라이언트 자격증명
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public SocialType ProviderType => SocialType.Google;

    public GoogleOAuthProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _clientId = configuration["OAuth:Google:ClientId"]
            ?? throw new InvalidOperationException("OAuth:Google:ClientId is not configured.");
        _clientSecret = configuration["OAuth:Google:ClientSecret"]
            ?? throw new InvalidOperationException("OAuth:Google:ClientSecret is not configured.");
        _redirectUri = configuration["OAuth:Google:RedirectUri"]
            ?? throw new InvalidOperationException("OAuth:Google:RedirectUri is not configured.");
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
            throw AuthException.OAuthFailed();
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(json)
            ?? throw AuthException.OAuthFailed();

        return tokenResponse.AccessToken;
    }

    // GET 사용자 프로필
    private async Task<OAuthUserInfo> FetchUserInfoAsync(string accessToken)
    {
        // Authorization 헤더를 추가
        using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw AuthException.OAuthFailed();

        var json = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json)
            ?? throw AuthException.OAuthFailed();

        return new OAuthUserInfo
        {
            SocialId = userInfo.Id,
            Email = userInfo.Email,
            // Nickname 값으로 name -> email -> id 설정 시도
            Nickname = userInfo.Name ?? userInfo.Email ?? userInfo.Id,
            ProfileImageUrl = userInfo.Picture ?? string.Empty
        };
    }

    // JSON의 snake_case 키를 C# PascalCase 프로퍼티 로 매핑
    private sealed class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = null!;
    }

    private sealed class GoogleUserInfoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        [JsonPropertyName("email")]
        public string? Email { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; } 

        [JsonPropertyName("picture")]
        public string? Picture { get; init; } 
    }
}
