using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Infrastructure.Dictionary;

public class DictionaryApiService(IHttpClientFactory httpClientFactory, ILogger<DictionaryApiService> logger) : IDictionaryService
{
    private const string BaseUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";

    public async Task<DictionaryWordData?> LookupAsync(string word)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{BaseUrl}{Uri.EscapeDataString(word)}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var entries = JsonSerializer.Deserialize<List<DictionaryEntry>>(json);
            if (entries is null || entries.Count == 0) return null;

            var definitions = entries
                .SelectMany(e => e.Meanings)
                .SelectMany(m => m.Definitions)
                .Select(d => d.Text)
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .ToList();

            if (definitions.Count == 0) return null;

            var first = entries[0];
            var phonetic = entries
                .SelectMany(e => e.Phonetics)
                .Select(p => p.Text)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t)) ?? string.Empty;

            var audioUrl = entries
                .SelectMany(e => e.Phonetics)
                .Select(p => p.Audio)
                .FirstOrDefault(a => !string.IsNullOrEmpty(a));

            return new DictionaryWordData(first.Word, definitions, phonetic, audioUrl);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "단어 사전 조회 실패: {Word}", word);
            return null;
        }
    }

    private sealed class DictionaryEntry
    {
        [JsonPropertyName("word")]
        public string Word { get; init; } = string.Empty;

        [JsonPropertyName("phonetics")]
        public List<Phonetic> Phonetics { get; init; } = [];

        [JsonPropertyName("meanings")]
        public List<Meaning> Meanings { get; init; } = [];
    }

    private sealed class Phonetic
    {
        [JsonPropertyName("text")]
        public string? Text { get; init; }

        [JsonPropertyName("audio")]
        public string? Audio { get; init; }
    }

    private sealed class Meaning
    {
        [JsonPropertyName("definitions")]
        public List<Definition> Definitions { get; init; } = [];
    }

    private sealed class Definition
    {
        [JsonPropertyName("definition")]
        public string Text { get; init; } = string.Empty;
    }
}
