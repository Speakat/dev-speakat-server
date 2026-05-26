namespace Speakat.Application.Flashcards.Dtos;

public class FlashcardDetailDto
{
    public long FlashcardId { get; init; }
    public string Word { get; init; } = null!;
    public string Meaning { get; init; } = null!;
    public string Phonetic { get; init; } = null!;
    public string? AudioUrl { get; init; }
    public bool IsMastered { get; init; }
}
