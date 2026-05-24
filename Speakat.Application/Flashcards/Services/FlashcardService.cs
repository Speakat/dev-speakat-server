using System.Text;
using Speakat.Application.Auth.Repositories;
using Speakat.Application.Flashcards.Dtos;
using Speakat.Application.Flashcards.Repositories;

namespace Speakat.Application.Flashcards.Services;

public class FlashcardService : IFlashcardService
{
    private readonly IFlashcardRepository _flashcardRepository;
    private readonly IUserRepository _userRepository;

    public FlashcardService(
        IFlashcardRepository flashcardRepository,
        IUserRepository userRepository)
    {
        _flashcardRepository = flashcardRepository;
        _userRepository = userRepository;
    }

    public async Task<FlashcardListDto> GetFlashcardsAsync(
        string userUuid,
        string? cursor,
        int size,
        long? questId)
    {
        var userId = await _userRepository.FindUserIdByUuidAsync(userUuid) ?? throw new UnauthorizedAccessException();

        long? cursorId = DecodeCursor(cursor);

        // size + 1개를 가져와서 다음 페이지가 있는지 확인
        var rows = await _flashcardRepository.GetFlashcardsAsync(userId, cursorId, size + 1, questId);

        var hasMore = rows.Count > size;
        var items = rows.Take(size).ToList();

        var nextCursor = hasMore ? EncodeCursor(items.Last().UserFlashcardId) : null;

        return new FlashcardListDto
        {
            Items = items.Select(r => new FlashcardItemDto
            {
                FlashcardId = r.FlashcardId,
                Word = r.Word,
                Meaning = r.Meaning,
                Phonetic = r.Phonetic,
                IsMastered = r.IsMastered,
                SavedAt = r.SavedAt,
                QuestId = r.QuestId,
                QuestTitle = r.QuestTitle
            }).ToList(),
            NextCursor = nextCursor,
            HasMore = hasMore
        };
    }

    // 커서 디코딩
    private static long? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor))
            return null;
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return long.TryParse(decoded, out var id) ? id : null;
    }

    // 커서 인코딩
    private static string EncodeCursor(long id) 
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(id.ToString()));
}