using System.Text.Json.Serialization;

public record EndSessionResponseDto(
    [property: JsonPropertyName("session_id")] string SessionId,
    [property: JsonPropertyName("status")]     string Status,
    [property: JsonPropertyName("ended_at")]   DateTime EndedAt
);
