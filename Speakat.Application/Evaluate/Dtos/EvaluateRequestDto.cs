using System.Text.Json.Serialization;

public record EvaluateRequestDto(
    [property: JsonPropertyName("quest_id")] long QuestId,
    [property: JsonPropertyName("turn")]     int Turn,
    [property: JsonPropertyName("audio")]    string Audio
);