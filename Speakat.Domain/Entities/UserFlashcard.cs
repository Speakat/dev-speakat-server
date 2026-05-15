namespace Speakat.Domain.Entities;

public class UserFlashcard
{
    public long UserFlashcardId { get; set; }
    public long UserId { get; set; }
    public long FlashcardId { get; set; }
    public long QuestId { get; set; }
    public string? RecommendationReason { get; set; }
    public DateTime SavedAt { get; set; }

    public User? User { get; set; }
    public Flashcard? Flashcard { get; set; }
    public Quest? Quest { get; set; }
}
