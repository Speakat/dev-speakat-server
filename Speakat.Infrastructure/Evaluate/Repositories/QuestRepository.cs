using Speakat.Application.Common.Exceptions;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence;

public class QuestRepository : IQuestRepository
{
    private readonly AppDbContext _db;

    public QuestRepository(AppDbContext db) => _db = db;

    public async Task<Quest> GetByIdAsync(long questId)
    {
        return await _db.Quests.FindAsync(questId)
               ?? throw new NotFoundException($"Quest {questId} 없음");
    }
}