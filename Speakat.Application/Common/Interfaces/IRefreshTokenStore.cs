namespace Speakat.Application.Common.Interfaces;

public interface IRefreshTokenStore
{
    Task SaveAsync(string refreshToken, string userUuid, TimeSpan ttl);
    Task<string?> GetUserUuidAsync(string refreshToken);
    Task DeleteAsync(string refreshToken);
}
