namespace Speakat.Domain.Entities;

public class Objective
{
    public long ObjectiveId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
