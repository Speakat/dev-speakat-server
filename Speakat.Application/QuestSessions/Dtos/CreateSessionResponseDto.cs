using System.Text.Json.Serialization;

public record CreateSessionResponseDto(
    [property: JsonPropertyName("session_id")]   string SessionId,
    [property: JsonPropertyName("npc_dialogue")] string NpcDialogue
);
