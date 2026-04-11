public interface IQuestRepository
{
    Task<Quest> GetByIdAsync(int questId);
}