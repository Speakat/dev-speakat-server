namespace Speakat.Application.Stages.Dtos;

public class QuestItemDto
{
    public long QuestId { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int SortOrder { get; init; }
    public bool IsCompleted { get; init; }
    public int AttemptCount { get; init; }
}
