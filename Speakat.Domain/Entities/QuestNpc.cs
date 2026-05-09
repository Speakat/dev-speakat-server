namespace Speakat.Domain.Entities;

public class QuestNpc
{
    public long QuestNpcId { get; set; }
    public long QuestId { get; set; }
    public long NpcId { get; set; }
    public string Role { get; set; } = null!;

    public Quest? Quest { get; set; }
    public Npc? Npc { get; set; }
}
