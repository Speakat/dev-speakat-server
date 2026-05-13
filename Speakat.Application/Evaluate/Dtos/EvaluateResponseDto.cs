public record EvaluateResponseDto(
    string NpcDialogue,
    string NpcDialogueAudio,
    bool IsTurnPassed,
    TurnEvaluationResult TurnEvaluation,
    QuestResult? QuestResult
);
