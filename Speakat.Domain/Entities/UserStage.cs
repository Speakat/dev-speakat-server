namespace Speakat.Domain.Entities;

public class UserStage
{
    public long UserStageId { get; set; }
    public string UserId { get; set; } = null!;
    public long StageId { get; set; }
    public string? Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User? User { get; set; }
    public Stage? Stage { get; set; }
}
