namespace Speakat.Application.Common.Exceptions;

public class AuthException : BusinessException
{

    private AuthException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static AuthException AccessTokenExpired() =>
        new("ACCESS_TOKEN_EXPIRED", "Access Token 만료", 401);
    
    public static AuthException InvalidToken() =>
        new("INVALID_TOKEN", "유효하지 않은 토큰", 401);
    
    public static AuthException BlacklistedToken() =>
        new("BLACKLIST_TOKEN", "블랙리스트 토큰", 401);
    
    public static AuthException RefreshTokenExpired() =>
        new("REFRESH_TOKEN_EXPIRED", "Refresh Token 만료", 401);
    
    public static AuthException OAuthFailed() =>
        new("OAUTH_AUTH_FAILED", "OAuth 인증 실패 (Provider 토큰 검증 실패)", 401);

    public static AuthException DisabledAccount() =>
        new("ACCOUNT_DISABLED", "비활성화된 계정", 403);
    
    public static AuthException DuplicateNickname() =>
        new("DUPLICATE_NICKNAME", "이미 존재하는 닉네임", 409);

    public static AuthException UnsupportedProvider() =>
        new("UNSUPPORTED_OAUTH_PROVIDER", "지원하지 않는 OAuth Provider", 400);

    public static AuthException UserNotFound() =>
        new("USER_NOT_FOUND", "존재하지 않는 사용자", 401);
}