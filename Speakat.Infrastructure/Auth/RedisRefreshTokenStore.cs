using Microsoft.Extensions.Caching.Distributed;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.Auth;

public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private const string KeyPrefix = "refresh:";

    private readonly IDistributedCache _cache;

    public RedisRefreshTokenStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task SaveAsync(string refreshToken, string userUuid, TimeSpan ttl) =>
        _cache.SetStringAsync(Key(refreshToken), userUuid, new DistributedCacheEntryOptions
        {
            // ttl일 뒤 자동 삭제
            AbsoluteExpirationRelativeToNow = ttl
        });

    public Task<string?> GetUserUuidAsync(string refreshToken) =>
        _cache.GetStringAsync(Key(refreshToken));

    public Task DeleteAsync(string refreshToken) =>
        _cache.RemoveAsync(Key(refreshToken));

    private static string Key(string refreshToken) => KeyPrefix + refreshToken;
}
