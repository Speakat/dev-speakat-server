namespace Speakat.Domain.Entities;

public class Stage
{
    public long StageId { get; set; }
    public long LanguageId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Language? Language { get; set; }
}