namespace Speakat.Infrastructure.Evaluate.Ai;

public class StubAiPipelineClient : IAiPipelineClient
{
    public Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, int turn)
    {
        var result = new AiPipelineResult(
            Roleplay:             "Oh hey, you're finally here! What happened?",
            RoleplayAudio:        "",
            Score:                85,
            Grade:                "good",
            BetterSuggestions:    ["apologize", "sorry", "regret"],
            RecommendationReason: "스터빙 고정 응답입니다.",
            SimilarityScore:      0.8f,
            SimilarityPassed:     true
        );
        return Task.FromResult(result);
    }
}
