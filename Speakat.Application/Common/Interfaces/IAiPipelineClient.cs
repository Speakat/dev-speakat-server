public interface IAiPipelineClient
{
    Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, int turn);
}
