namespace Speakat.Domain.Entities;

public class GameSession
{
    public string SessionId { get; set; } = null!;
    public long UserId { get; set; }
    public long QuestId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? SemanticScore { get; set; }
    public int? GrammarScore { get; set; }
    public int? NaturalnessScore { get; set; }

    public User? User { get; set; }
    public Quest? Quest { get; set; }
}
