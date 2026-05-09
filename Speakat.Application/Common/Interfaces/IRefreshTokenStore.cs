namespace Speakat.Application.Common.Interfaces;

public interface IRefreshTokenStore
{
    Task SaveAsync(string refreshToken, string userUuid, TimeSpan ttl);
    Task<string?> GetUserUuidAsync(string refreshToken);
    Task DeleteAsync(string refreshToken);
    Task BlacklistAsync(string refreshToken, TimeSpan ttl);
    Task<bool> IsBlacklistedAsync(string refreshToken);
}
