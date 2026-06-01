using System.Net.Http.Json;
using Speakat.Application.Common.Exceptions;

namespace Speakat.Infrastructure.Evaluate.Ai;

public class AiPipelineClient : IAiPipelineClient
{
    private readonly HttpClient _http;
    public AiPipelineClient(HttpClient http) => _http = http;

    public async Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, string sessionId, int turn, QuestPromptDto questPrompt)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("/evaluate", new
            {
                audio_base64 = audioBase64,
                quest_id     = questId,
                session_id   = sessionId,
                turn,
                quest_prompt = questPrompt,
            });
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadFromJsonAsync<AiPipelineResult>()
                ?? throw new InvalidOperationException("AI 서비스 응답 파싱 실패");
        }
        catch (Exception ex) when (ex is not AiServiceException)
        {
            throw AiServiceException.Unavailable();
        }
    }
}
