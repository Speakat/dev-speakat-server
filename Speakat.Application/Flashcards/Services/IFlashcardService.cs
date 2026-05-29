using Speakat.Application.Flashcards.Dtos;

namespace Speakat.Application.Flashcards.Services;

public interface IFlashcardService
{
    Task<FlashcardListDto> GetFlashcardsAsync(
        string userUuid,
        string? cursor,
        int size,
        long? questId);

    Task<FlashcardDetailDto> GetFlashcardDetailAsync(string userUuid, long flashcardId);

    Task<PatchFlashcardResultDto> UpdateIsMasteredAsync(string userUuid, long flashcardId, bool isMastered);
}