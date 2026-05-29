namespace Speakat.Application.Flashcards.Dtos;

public class FlashcardListDto
{
    public IReadOnlyList<FlashcardItemDto> Items { get; init; } = [];
    public string? NextCursor { get; init; }
    public bool HasMore { get; init; }
}