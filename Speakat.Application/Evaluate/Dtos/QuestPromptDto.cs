// QuestPromptDto.cs
public record QuestPromptDto(
    string         Scenario,
    string         SuccessCriteria,
    NpcDto         Npc,
    List<ObjectiveDto> Objectives
);

// NpcDto.cs
public record NpcDto(
    string Name,   // Npcs.name
    string Role,   // QuestNpcs.role
    string Tone    // Npcs.tone
);

// ObjectiveDto.cs
public record ObjectiveDto(
    string Name,        // Objectives.name
    string Description  // Objectives.description
);