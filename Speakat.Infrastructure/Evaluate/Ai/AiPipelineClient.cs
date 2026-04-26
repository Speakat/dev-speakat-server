using System.Net.Http.Json;

namespace Speakat.Infrastructure.Evaluate.Ai;

public class AiPipelineClient : IAiPipelineClient
{
    private readonly HttpClient _http;
    public AiPipelineClient(HttpClient http) => _http = http;

    public async Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, int turn)
    {
        var res = await _http.PostAsJsonAsync("/evaluate", new { 
            audio_base64 = audioBase64,
            quest_id = questId,
            turn = turn
        });
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<AiPipelineResult>()
            ?? throw new InvalidOperationException("AI 서비스 응답 파싱 실패");
    }
}