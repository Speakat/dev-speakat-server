namespace Speakat.Domain.Entities;

public class UserQuest
{
    public long UserQuestId { get; set; }
    public long UserId { get; set; }
    public long QuestId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User? User { get; set; }
    public Quest? Quest { get; set; }
}
