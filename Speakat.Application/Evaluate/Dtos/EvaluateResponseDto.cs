public record EvaluateResponseDto(
    string Roleplay,
    int Score,
    string Grade,
    List<string> BetterSuggestions,
    string RecommendationReason,
    bool IsTurnPassed,
    bool IsQuestComplete
);