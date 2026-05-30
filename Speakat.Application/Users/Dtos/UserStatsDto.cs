namespace Speakat.Application.Users.Dtos;

public class UserStatsDto
{
    public int TotalQuestsCompleted { get; init; }
    public int TotalSessionsPlayed { get; init; }
    public int TotalStagesCompleted { get; init; }
    public double? AvgSemanticScore { get; init; }
    public double? AvgGrammarScore { get; init; }
    public double? AvgNaturalnessScore { get; init; }
}
