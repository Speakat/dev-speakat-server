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

public record FlashcardDetailData(
    long FlashcardId,
    string Word,
    string Meaning,
    string Phonetic,
    string? AudioUrl,
    bool IsMastered
);

public interface IFlashcardRepository
{
    Task SaveAsync(long userId, long questId, TurnEvaluationResult evaluationResult);

    Task<IReadOnlyList<FlashcardData>> GetFlashcardsAsync(
        long userId,
        long? cursorId,
        int size,
        long? questId);

    Task<FlashcardDetailData?> GetFlashcardDetailAsync(long userId, long flashcardId);

    Task<Flashcard?> UpdateIsMasteredAsync(long userId, long flashcardId, bool isMastered);
}