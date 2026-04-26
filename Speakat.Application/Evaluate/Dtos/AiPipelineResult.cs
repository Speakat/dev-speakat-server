using System.Text.Json.Serialization;

public record AiPipelineResult(
    string Roleplay,
    int Score,
    string Grade,
    [property: JsonPropertyName("better_suggestions")]    List<string> BetterSuggestions,
    [property: JsonPropertyName("recommendation_reason")] string RecommendationReason,
    [property: JsonPropertyName("similarity_score")]      float SimilarityScore,
    [property: JsonPropertyName("similarity_passed")]     bool SimilarityPassed
);
