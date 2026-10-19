using Microsoft.EntityFrameworkCore;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;
using Speakat.Application.Flashcards.Repositories;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly AppDbContext _context;
    private readonly IAiPipelineClient _aiPipelineClient;
    private readonly ITranslationService _translationService;
    private const float DuplicateThreshold = 0.85f;
    private const long EnglishLanguageId = 1;
    private const long KoreanLanguageId = 2;

    public FlashcardRepository(
        AppDbContext context,
        IAiPipelineClient aiPipelineClient,
        ITranslationService translationService)
    {
        _context = context;
        _aiPipelineClient = aiPipelineClient;
        _translationService = translationService;
    }

    public async Task<FlashcardDetailData> SaveWordAsync(long userId, long questId, string word, string meaning, string partOfSpeech, string recommendationReason)
    {
        var questExists = await _context.Quests.AnyAsync(q => q.QuestId == questId);
        if (!questExists) throw QuestException.NotFound();

        var wordEntity = await FindOrCreateWordAsync(word, meaning, partOfSpeech);

        var flashcard = await _context.Flashcards.FirstOrDefaultAsync(f => f.WordId == wordEntity.WordId)
                        ?? await CreateFlashcardAsync(wordEntity.WordId, meaning);

        var userFlashcard = new UserFlashcard
        {
            UserId               = userId,
            FlashcardId          = flashcard.FlashcardId,
            QuestId              = questId,
            RecommendationReason = recommendationReason,
        };
        _context.UserFlashcards.Add(userFlashcard);
        await _context.SaveChangesAsync();

        return new FlashcardDetailData(
            flashcard.FlashcardId,
            wordEntity.Text,
            flashcard.Definition,
            wordEntity.Phonetic,
            wordEntity.AudioUrl,
            userFlashcard.IsMastered
        );
    }

    private async Task<Word> FindOrCreateWordAsync(string text, string meaning, string partOfSpeech)
    {
        var candidates = await _context.Words
            .Where(w => w.Text == text && w.PartOfSpeech == partOfSpeech)
            .ToListAsync();

        if (candidates.Count > 0)
        {
            var existingMeanings = candidates.Select(w => w.Definition).ToList();
            var (_, scores) = await _aiPipelineClient.FindBestDefinitionAsync(meaning, existingMeanings);
            var maxScore = scores.Max();

            if (maxScore >= DuplicateThreshold)
                return candidates[scores.ToList().IndexOf(maxScore)];
        }

        var newWord = new Word
        {
            LanguageId   = EnglishLanguageId,
            Text         = text,
            Definition   = meaning,
            PartOfSpeech = partOfSpeech,
            Phonetic     = string.Empty,
            AudioUrl     = null,
        };
        _context.Words.Add(newWord);
        await _context.SaveChangesAsync();
        return newWord;
    }

    private async Task<Flashcard> CreateFlashcardAsync(long wordId, string meaning)
    {
        var flashcard = new Flashcard
        {
            WordId     = wordId,
            LanguageId = KoreanLanguageId,
            Definition = meaning,
        };
        _context.Flashcards.Add(flashcard);
        await _context.SaveChangesAsync();
        return flashcard;
    }

    public async Task<FlashcardDetailData?> GetFlashcardDetailAsync(long userId, long flashcardId)
    {
        return await _context.UserFlashcards
            .Where(uf => uf.UserId == userId && uf.FlashcardId == flashcardId)
            .Join(_context.Flashcards,
                uf => uf.FlashcardId,
                f => f.FlashcardId,
                (uf, f) => new { uf, f })
            .Join(_context.Words,
                x => x.f.WordId,
                w => w.WordId,
                (x, w) => new FlashcardDetailData(
                    x.f.FlashcardId,
                    w.Text,
                    x.f.Definition,
                    w.Phonetic,
                    w.AudioUrl,
                    x.uf.IsMastered
                ))
            .FirstOrDefaultAsync();
    }

    public async Task<UserFlashcard?> UpdateIsMasteredAsync(long userId, long flashcardId, bool isMastered)
    {
        var userFlashcards = await _context.UserFlashcards
            .Where(uf => uf.UserId == userId && uf.FlashcardId == flashcardId)
            .ToListAsync();

        if (userFlashcards.Count == 0) return null;

        foreach (var uf in userFlashcards)
            uf.IsMastered = isMastered;

        await _context.SaveChangesAsync();
        return userFlashcards[0];
    }

    public async Task<IReadOnlyList<FlashcardData>> GetFlashcardsAsync(
        long userId,
        long? cursorId,
        int size,
        long? questId)
    {
        var query = _context.UserFlashcards
            .Where(uf => uf.UserId == userId)
            .Where(uf => questId == null || uf.QuestId == questId)
            .Join(_context.Flashcards,
                uf => uf.FlashcardId,
                f => f.FlashcardId,
                (uf, f) => new { uf, f })
            .Join(_context.Words,
                x => x.f.WordId,
                w => w.WordId,
                (x, w) => new { x.uf, x.f, w })
            .Join(_context.Quests,
                x => x.uf.QuestId,
                q => q.QuestId,
                (x, q) => new { x.uf, x.f, x.w, q });

        query = query.OrderByDescending(x => x.uf.UserFlashcardId);
        if (cursorId.HasValue)
            query = query.Where(x => x.uf.UserFlashcardId < cursorId.Value);

        return await query
            .Take(size)
            .Select(x => new FlashcardData(
                x.uf.UserFlashcardId,
                x.f.FlashcardId,
                x.w.Text,
                x.f.Definition,
                x.w.Phonetic,
                x.uf.IsMastered,
                x.uf.SavedAt,
                x.uf.QuestId,
                x.q.Title
            )).ToListAsync();
    }
}
