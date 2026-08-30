public record BetterSuggestionDto(
    string Word,
    string Meaning,
    string PartOfSpeech
);

public record TurnEvaluationDto(
    float ContextRelevance,
    float GrammarAccuracy,
    float ExpressionQuality,
    List<string> ObjectiveProgress,
    bool IsQuestComplete,
    string Reason,
    List<BetterSuggestionDto> BetterSuggestions,
    string RecommendationReason
);

public record QuestResultDto(
    float AverageContextRelevance,
    float AverageGrammarAccuracy,
    float AverageExpressionQuality,
    List<string> AchievedObjectives,
    bool IsQuestSuccess
);

public record EvaluateResponseDto(
    string UserText,
    string NpcDialogue,
    string NpcDialogueAudio,
    bool IsTurnPassed,
    TurnEvaluationDto TurnEvaluation,
    QuestResultDto? QuestResult
);
