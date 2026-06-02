namespace Speakat.Domain.Entities;

public class Flashcard
{
    public long FlashcardId { get; set; }
    public long WordId { get; set; }
    public long LanguageId { get; set; }
    public string Definition { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Word? Word { get; set; }
    public Language? Language { get; set; }
}
