namespace Speakat.Domain.Entities;

public class Flashcard
{
    public long FlashcardId { get; set; }
    public long WordId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsMastered { get; set; }

    public Word? Word { get; set; }
}
