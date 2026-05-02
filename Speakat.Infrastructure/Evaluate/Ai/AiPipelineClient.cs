using System.Net.Http.Json;

namespace Speakat.Infrastructure.Evaluate.Ai;

public class AiPipelineClient : IAiPipelineClient
{
    private readonly HttpClient _http;
    public AiPipelineClient(HttpClient http) => _http = http; // 의존성 주입 패턴

    public async Task<AiPipelineResult> EvaluateAsync(string audioBase64, long questId, string sessionId, int turn, QuestPromptDto questPrompt)
    {
        var res = await _http.PostAsJsonAsync("/evaluate", new // HttpClient에 설정된 baseAddress에 /evaluate를 붙여 요청
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
}
