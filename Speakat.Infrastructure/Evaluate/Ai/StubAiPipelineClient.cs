namespace Speakat.Infrastructure.Evaluate.Ai;

public class StubAiPipelineClient : IAiPipelineClient
{
    public Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, string sessionId, int turn, QuestPromptDto questPrompt)
    {
        var result = new AiPipelineResult(
            NpcDialogue:      "Oh hey, you're finally here! What happened?",
            NpcDialogueAudio: "",
            SimilarityScore:  0.8f,
            SimilarityPassed: true,
            TurnEvaluation:   new TurnEvaluationResult(
                ContextRelevance:     0.85f,
                GrammarAccuracy:      0.90f,
                ExpressionQuality:    0.80f,
                ObjectiveProgress:    ["apologize"],
                IsQuestComplete:      false,
                Reason:               "스터빙 고정 응답입니다.",
                BetterSuggestions:    ["apologize", "sorry", "regret"],
                RecommendationReason: "스터빙 고정 추천 이유입니다."
            ),
            QuestResult: null
        );
        return Task.FromResult(result);
    }
}
