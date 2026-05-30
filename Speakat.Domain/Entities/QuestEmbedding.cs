namespace Speakat.Domain.Entities;

public class QuestEmbedding
{
    public long Id { get; set; }
    public long QuestId { get; set; }
    public string ReferenceSentence { get; set; } = null!;

    public Quest? Quest { get; set; }
}
