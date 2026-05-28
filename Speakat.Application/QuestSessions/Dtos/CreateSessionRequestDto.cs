using System.Text.Json.Serialization;

public record CreateSessionRequestDto(
    [property: JsonPropertyName("quest_id")] long QuestId
);
