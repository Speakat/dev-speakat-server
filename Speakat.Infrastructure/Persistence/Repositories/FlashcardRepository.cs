using Microsoft.EntityFrameworkCore;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class FlashcardRepository(AppDbContext db) : IFlashcardRepository
{
    public async Task SaveAsync(long userId, long questId, TurnEvaluationResult evaluationResult)
    {
        foreach (var suggestion in evaluationResult.BetterSuggestions)
        {
            var word = await db.Words.FirstOrDefaultAsync(w => w.Text == suggestion);
            if (word is null) continue;

            var flashcard = await db.Flashcards.FirstOrDefaultAsync(f => f.WordId == word.WordId)
                ?? await CreateFlashcardAsync(word.WordId);

            db.UserFlashcards.Add(new UserFlashcard
            {
                UserId = userId,
                FlashcardId = flashcard.FlashcardId,
                QuestId = questId,
                RecommendationReason = evaluationResult.RecommendationReason,
            });
        }

        await db.SaveChangesAsync();
    }

    private async Task<Flashcard> CreateFlashcardAsync(long wordId)
    {
        var flashcard = new Flashcard { WordId = wordId };
        db.Flashcards.Add(flashcard);
        await db.SaveChangesAsync();
        return flashcard;
    }
}
