namespace Speakat.Infrastructure.Evaluate.Ai;

public class StubAiPipelineClient : IAiPipelineClient
{
    public Task<(string BestDefinition, IReadOnlyList<float> Scores)> FindBestDefinitionAsync(string query, IReadOnlyList<string> definitions)
        => Task.FromResult((definitions.FirstOrDefault() ?? string.Empty, (IReadOnlyList<float>)definitions.Select(_ => 0f).ToList()));

    public Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, string sessionId, int turn, QuestPromptDto questPrompt)
    {
        var result = new AiPipelineResult(
            UserText:         "I'm so sorry I'm late.",
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
                BetterSuggestions:    [
                    new BetterSuggestionResult("apologize", "사과하다", "verb"),
                    new BetterSuggestionResult("sorry",     "미안한",   "adjective"),
                    new BetterSuggestionResult("regret",    "후회하다", "verb"),
                ],
                RecommendationReason: "스터빙 고정 추천 이유입니다."
            ),
            QuestResult: new QuestResult(
                AverageContextRelevance:  0.85f,
                AverageGrammarAccuracy:   0.90f,
                AverageExpressionQuality: 0.80f,
                AchievedObjectives:       ["apologize"],
                IsQuestSuccess:           true
            )
            // QuestResult: null
        );
        return Task.FromResult(result);
    }
}
