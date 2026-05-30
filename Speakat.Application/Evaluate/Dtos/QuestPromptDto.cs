using System.Text.Json.Serialization;

public record QuestPromptDto(
    [property: JsonPropertyName("scenario")]            string Scenario,
    [property: JsonPropertyName("success_criteria")]    string SuccessCriteria,
    [property: JsonPropertyName("npc")]                 NpcDto Npc,
    [property: JsonPropertyName("objectives")]          List<ObjectiveDto> Objectives,
    [property: JsonPropertyName("reference_sentences")] List<string> ReferenceSentences
);

public record NpcDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("tone")] string Tone,
    [property: JsonPropertyName("voice")] string Voice
);

public record ObjectiveDto(
    [property: JsonPropertyName("name")]        string Name,
    [property: JsonPropertyName("description")] string Description
);