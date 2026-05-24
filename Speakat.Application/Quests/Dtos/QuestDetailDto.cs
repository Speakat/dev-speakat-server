using Speakat.Domain.Enums;

namespace Speakat.Application.Quests.Dtos;

public class QuestDetailDto
{
    public long QuestId { get; init; }
    public long StageId { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ThumbnailUrl { get; init; } = null!;
    public IReadOnlyList<string> Objectives { get; init; } = [];
    public QuestStatus Status { get; init; }
    public int? BestScore { get; init; }
    public int AttemptCount { get; init; }
}