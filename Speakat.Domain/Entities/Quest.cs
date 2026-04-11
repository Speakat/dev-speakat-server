public class Quest
{
    public int QuestId { get; init; }
    public int StageId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public DateTime CreatedAt { get; init; }
}