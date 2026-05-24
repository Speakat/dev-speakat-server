
namespace Speakat.Domain.Entities;

public class Quest
{
    public long QuestId { get; set; }
    public long StageId { get; set; }
    public long PromptId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Thumbnail { get; set; } = null!;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Stage? Stage { get; set; }
    public Prompt? Prompt { get; set; }
}
