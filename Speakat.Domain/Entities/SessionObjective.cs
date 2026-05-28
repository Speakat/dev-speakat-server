namespace Speakat.Domain.Entities;

public class SessionObjective
{
    public long SessionObjectiveId { get; set; }
    public string SessionId { get; set; } = null!;
    public long QuestObjectiveId { get; set; }
    public bool IsAchieved { get; set; }

    public GameSession? GameSession { get; set; }
    public QuestObjective? QuestObjective { get; set; }
}
