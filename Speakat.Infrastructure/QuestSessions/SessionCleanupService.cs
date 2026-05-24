using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.QuestSessions;

public class SessionCleanupService(IServiceScopeFactory scopeFactory, ILogger<SessionCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan SessionTtl = TimeSpan.FromHours(1);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(CheckInterval, stoppingToken);
            await CleanupExpiredSessionsAsync();
        }
    }

    private async Task CleanupExpiredSessionsAsync()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredBefore = DateTime.UtcNow - SessionTtl;

            var expiredSessions = await db.GameSessions
                .Where(gs => gs.Status == "IN_PROGRESS" && gs.StartedAt < expiredBefore)
                .ToListAsync();

            if (expiredSessions.Count == 0) return;

            foreach (var session in expiredSessions)
            {
                session.Status  = "ABANDONED";
                session.EndedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            logger.LogInformation("만료된 게임 세션 {Count}개를 ABANDONED 처리했습니다.", expiredSessions.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "세션 정리 중 오류가 발생했습니다.");
        }
    }
}
