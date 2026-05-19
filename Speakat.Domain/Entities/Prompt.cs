namespace Speakat.Domain.Entities;

public class Prompt
{
    public long PromptId { get; set; }
    public string Scenario { get; set; } = null!;
    public string SuccessCriteria { get; set; } = null!;
    public string OpeningLine { get; set; } = null!;
}
