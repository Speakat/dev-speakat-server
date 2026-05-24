namespace Speakat.Application.Flashcards.Repositories;

public record FlashcardData(
    long UserFlashcardId,
    long FlashcardId,
    string Word,
    string Meaning,
    string Phonetic,
    bool IsMastered,
    DateTime SavedAt,
    long QuestId,
    string QuestTitle
);

public interface IFlashcardRepository
{
    Task<IReadOnlyList<FlashcardData>> GetFlashcardsAsync(
        long userId,
        long? cursorId,
        int size,
        long? questId);
}