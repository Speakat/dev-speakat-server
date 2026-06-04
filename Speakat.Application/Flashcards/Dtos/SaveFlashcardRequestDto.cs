namespace Speakat.Application.Flashcards.Dtos;

public class SaveFlashcardRequestDto
{
    public long QuestId { get; init; }
    public string Word { get; init; } = string.Empty;
    public string RecommendationReason { get; init; } = string.Empty;
}
