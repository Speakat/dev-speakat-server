public record EvaluateResponseDto(
    string Roleplay,
    string RoleplayAudio,
    int Score,
    string Grade,
    List<string> BetterSuggestions,
    string RecommendationReason,
    bool IsTurnPassed,
    bool IsQuestComplete
);
