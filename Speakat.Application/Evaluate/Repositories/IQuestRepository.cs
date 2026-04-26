using Speakat.Domain.Entities;

public interface IQuestRepository
{
    Task<Quest> GetByIdAsync(long questId);
}