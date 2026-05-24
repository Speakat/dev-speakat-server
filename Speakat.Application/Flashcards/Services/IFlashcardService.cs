using Speakat.Application.Flashcards.Dtos;

namespace Speakat.Application.Flashcards.Services;

public interface IFlashcardService
{
    Task<FlashcardListDto> GetFlashcardsAsync(
        string userUuid,
        string? cursor,
        int size,
        long? questId);
}