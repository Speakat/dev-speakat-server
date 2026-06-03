public interface IAiPipelineClient
{
    Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, string sessionId, int turn, QuestPromptDto questPrompt);
    Task<(string BestDefinition, IReadOnlyList<float> Scores)> FindBestDefinitionAsync(string query, IReadOnlyList<string> definitions);
}
