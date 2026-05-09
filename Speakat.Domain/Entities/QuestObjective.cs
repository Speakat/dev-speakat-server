namespace Speakat.Domain.Entities;

public class QuestObjective
{
    public long QuestObjectiveId { get; set; }
    public long QuestId { get; set; }
    public long ObjectiveId { get; set; }
    public int? SortOrder { get; set; }

    public Quest? Quest { get; set; }
    public Objective? Objective { get; set; }
}
