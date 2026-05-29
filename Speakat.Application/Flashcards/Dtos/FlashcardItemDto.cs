namespace Speakat.Application.Flashcards.Dtos;

public class FlashcardItemDto
{
    public long FlashcardId { get; init; }
    public string Word { get; init; } = null!;
    public string Meaning { get; init; } = null!;
    public string Phonetic { get; init; } = null!;
    public bool IsMastered { get; init; }
    public DateTime SavedAt { get; init; }
    public long QuestId { get; init; }
    public string QuestTitle { get; init; } = null!;
}