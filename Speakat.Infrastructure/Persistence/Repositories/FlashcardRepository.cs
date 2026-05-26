using Microsoft.EntityFrameworkCore;
using Speakat.Application.Flashcards.Repositories;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly AppDbContext _context;

    public FlashcardRepository(AppDbContext context) => _context = context;
    
    public async Task SaveAsync(long userId, long questId, TurnEvaluationResult evaluationResult)
    {
        foreach (var suggestion in evaluationResult.BetterSuggestions)
        {
            var word = await _context.Words.FirstOrDefaultAsync(w => w.Text == suggestion);
            if (word is null) continue;

            var flashcard = await _context.Flashcards.FirstOrDefaultAsync(f => f.WordId == word.WordId)
                ?? await CreateFlashcardAsync(word.WordId);

            _context.UserFlashcards.Add(new UserFlashcard
            {
                UserId = userId,
                FlashcardId = flashcard.FlashcardId,
                QuestId = questId,
                RecommendationReason = evaluationResult.RecommendationReason,
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Flashcard> CreateFlashcardAsync(long wordId)
    {
        var flashcard = new Flashcard { WordId = wordId };
        _context.Flashcards.Add(flashcard);
        await _context.SaveChangesAsync();
        return flashcard;
    }

    public async Task<Flashcard?> UpdateIsMasteredAsync(long userId, long flashcardId, bool isMastered)
    {
        var hasFlashcard = await _context.UserFlashcards
            .AnyAsync(uf => uf.UserId == userId && uf.FlashcardId == flashcardId);

        if (!hasFlashcard) return null;

        var flashcard = await _context.Flashcards.FindAsync(flashcardId);
        if (flashcard is null) return null;

        flashcard.IsMastered = isMastered;
        await _context.SaveChangesAsync();
        return flashcard;
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
                x.w.Definition,
                x.w.Phonetic,
                x.f.IsMastered,
                x.uf.SavedAt,
                x.uf.QuestId,
                x.q.Title
            )).ToListAsync();
    }
}
