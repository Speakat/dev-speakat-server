using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.Translation;

public class GoogleTranslationService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<GoogleTranslationService> logger) : ITranslationService
{
    private const string TranslateUrl = "https://translation.googleapis.com/language/translate/v2";

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        var apiKey = configuration["Google:TranslationApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("Google TranslationApiKey가 설정되지 않아 번역을 건너뜁니다.");
            return text;
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            var body = JsonSerializer.Serialize(new { q = text, target = targetLanguage, format = "text" });
            var response = await client.PostAsync(
                $"{TranslateUrl}?key={apiKey}",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("번역 API 호출 실패: {StatusCode}", response.StatusCode);
                return text;
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TranslationResponse>(json);
            return result?.Data?.Translations?.FirstOrDefault()?.TranslatedText ?? text;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "번역 실패, 원문을 사용합니다: {Text}", text);
            return text;
        }
    }

    private sealed class TranslationResponse
    {
        [JsonPropertyName("data")]
        public TranslationData? Data { get; init; }
    }

    private sealed class TranslationData
    {
        [JsonPropertyName("translations")]
        public List<TranslationItem>? Translations { get; init; }
    }

    private sealed class TranslationItem
    {
        [JsonPropertyName("translatedText")]
        public string TranslatedText { get; init; } = string.Empty;
    }
}
