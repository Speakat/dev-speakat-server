using Speakat.Domain.Enums;

namespace Speakat.Application.Stages.Dtos;

public class StageItemDto
{
    public long StageId { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public StageStatus Status { get; init; }
    public int QuestCount { get; init; }
    public int CompletedQuestCount { get; init; }
}
