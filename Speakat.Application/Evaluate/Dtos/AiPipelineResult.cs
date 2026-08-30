using System.Text.Json.Serialization;

public record BetterSuggestionResult(
    [property: JsonPropertyName("word")]           string Word,
    [property: JsonPropertyName("meaning")]        string Meaning,
    [property: JsonPropertyName("part_of_speech")] string PartOfSpeech
);

public record TurnEvaluationResult(
    [property: JsonPropertyName("context_relevance")]    float ContextRelevance,
    [property: JsonPropertyName("grammar_accuracy")]     float GrammarAccuracy,
    [property: JsonPropertyName("expression_quality")]   float ExpressionQuality,
    [property: JsonPropertyName("objective_progress")]   List<string> ObjectiveProgress,
    [property: JsonPropertyName("is_quest_complete")]    bool IsQuestComplete,
    [property: JsonPropertyName("reason")]               string Reason,
    [property: JsonPropertyName("better_suggestions")]   List<BetterSuggestionResult> BetterSuggestions,
    [property: JsonPropertyName("recommendation_reason")] string RecommendationReason
);

public record QuestResult(
    [property: JsonPropertyName("average_context_relevance")]  float AverageContextRelevance,
    [property: JsonPropertyName("average_grammar_accuracy")]   float AverageGrammarAccuracy,
    [property: JsonPropertyName("average_expression_quality")] float AverageExpressionQuality,
    [property: JsonPropertyName("achieved_objectives")]        List<string> AchievedObjectives,
    [property: JsonPropertyName("is_quest_success")]           bool IsQuestSuccess
);

public record AiPipelineResult(
    [property: JsonPropertyName("user_text")]          string UserText,
    [property: JsonPropertyName("npc_dialogue")]       string NpcDialogue,
    [property: JsonPropertyName("npc_dialogue_audio")] string NpcDialogueAudio,
    [property: JsonPropertyName("similarity_score")]   float SimilarityScore,
    [property: JsonPropertyName("similarity_passed")]  bool SimilarityPassed,
    [property: JsonPropertyName("turn_evaluation")]    TurnEvaluationResult TurnEvaluation,
    [property: JsonPropertyName("quest_result")]       QuestResult? QuestResult
);
