public class QuestRepository : IQuestRepository
{
    private readonly AppDbContext _db;

    public QuestRepository(AppDbContext db) => _db = db;

    public async Task<Quest> GetByIdAsync(int questId)
    {
        return await _db.Quests.FindAsync(questId)
               ?? throw new NotFoundException($"Quest {questId} 없음");
    }
}