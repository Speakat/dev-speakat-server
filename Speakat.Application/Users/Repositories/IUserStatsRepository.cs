namespace Speakat.Application.Users.Repositories;

public record UserStatsData(
    int TotalQuestsCompleted,
    int TotalSessionsPlayed,
    int TotalStagesCompleted,
    double? AvgSemanticScore,
    double? AvgGrammarScore,
    double? AvgNaturalnessScore
);

public interface IUserStatsRepository
{
    Task<UserStatsData> GetStatsAsync(long userId);
}
