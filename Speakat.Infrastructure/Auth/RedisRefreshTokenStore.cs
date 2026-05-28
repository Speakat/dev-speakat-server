using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Speakat.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Speakat.Infrastructure.Auth;

public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private const string KeyPrefix = "refreshToken:";
    private const string BlacklistPrefix = "blacklist:refreshToken:";

    private readonly IDistributedCache _cache;
    private readonly IDatabase _db;

    public RedisRefreshTokenStore(IDistributedCache cache, IConnectionMultiplexer multiplexer)
    {
        _cache = cache;
        _db = multiplexer.GetDatabase();
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

    public async Task BlacklistAsync(string refreshToken)
    {
        var remaining = await _db.KeyTimeToLiveAsync(Key(refreshToken));
        if (remaining is null || remaining.Value <= TimeSpan.Zero) return;
        
        await _cache.SetStringAsync(BlacklistKey(refreshToken), "logout", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = remaining
        });
    }

    public async Task<bool> IsBlacklistedAsync(string refreshToken) =>
        await _cache.GetStringAsync(BlacklistKey(refreshToken)) is not null;

    private static string Key(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return KeyPrefix + Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string BlacklistKey(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return BlacklistPrefix + Convert.ToHexString(hash).ToLowerInvariant();
    }
}
