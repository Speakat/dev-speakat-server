public record AiPipelineResult(
    string Roleplay,
    int Score,
    string Grade,
    List<string> BetterSuggestions,
    string RecommendationReason,
    float SimilarityScore,
    bool SimilarityPassed
);