public record EvaluateResponseDto(
    string NpcDialogue,
    string NpcDialogueAudio,
    bool IsTurnPassed,
    bool IsQuestComplete,
    TurnEvaluationResult TurnEvaluation,
    QuestResult? QuestResult
);
