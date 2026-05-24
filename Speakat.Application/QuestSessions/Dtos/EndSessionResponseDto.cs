public record EndSessionResponseDto(
    string SessionId,
    string Status,
    DateTime EndedAt
);
