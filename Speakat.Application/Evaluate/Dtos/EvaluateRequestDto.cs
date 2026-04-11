public record EvaluateRequestDto(
    [property: JsonPropertyName("session_id")]   string SessionId,
    [property: JsonPropertyName("quest_id")]     int QuestId,
    [property: JsonPropertyName("turn")]         int Turn,
    [property: JsonPropertyName("is_last_turn")] bool IsLastTurn,
    [property: JsonPropertyName("audio")]        string Audio
);