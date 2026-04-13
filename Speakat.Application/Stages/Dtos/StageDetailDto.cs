namespace Speakat.Application.Stages.Dtos;

public class StageDetailDto
{
    public long StageId { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Status { get; init; } = null!;
    public IReadOnlyList<QuestItemDto> Quests { get; init; } = [];
}
