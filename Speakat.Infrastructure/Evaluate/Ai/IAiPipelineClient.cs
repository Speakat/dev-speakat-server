public interface IAiPipelineClient
{
    Task<AiPipelineResult> EvaluateAsync(string audioBase64);
}