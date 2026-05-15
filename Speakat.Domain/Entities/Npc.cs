namespace Speakat.Domain.Entities;

public class Npc
{
    public long NpcId { get; set; }
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string Tone { get; set; } = null!;
}
