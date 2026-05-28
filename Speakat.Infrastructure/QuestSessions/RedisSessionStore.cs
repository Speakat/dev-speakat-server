using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.QuestSessions;

public class RedisSessionStore : ISessionStore
{
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(1);
    private readonly IDistributedCache _cache;

    public RedisSessionStore(IDistributedCache cache) => _cache = cache;

    public async Task SaveAsync(string sessionId, long userId, long questId)
    {
        var json = JsonSerializer.Serialize(new { userId, questId });
        await _cache.SetStringAsync($"session:{sessionId}", json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Ttl
        });
    }

    public async Task<(long UserId, long QuestId)?> GetAsync(string sessionId)
    {
        var raw = await _cache.GetStringAsync($"session:{sessionId}");
        if (raw is null) return null;

        using var doc = JsonDocument.Parse(raw);
        var userId = doc.RootElement.GetProperty("userId").GetInt64();
        var questId = doc.RootElement.GetProperty("questId").GetInt64();
        return (userId, questId);
    }

    public Task DeleteAsync(string sessionId)
        => _cache.RemoveAsync($"session:{sessionId}");
}
