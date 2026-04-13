using Microsoft.EntityFrameworkCore;
using Speakat.Application.Stages.Repositories;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class StageRepository : IStageRepository
{
    private readonly AppDbContext _context;

    public StageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<StageProgressData>> GetStagesWithProgressAsync(long userId)
    {
        return await _context.Stages
            .OrderBy(s => s.SortOrder)
            .Select(s => new StageProgressData(
                s.StageId,
                s.Title,
                s.Description,
                _context.Quests.Count(q => q.StageId == s.StageId),
                _context.Quests.Count(q => q.StageId == s.StageId &&
                    _context.UserQuests.Any(uq => uq.QuestId == q.QuestId
                        && uq.UserId == userId
                        && uq.CompletedAt != null))
            ))
            .ToListAsync();
    }
}
