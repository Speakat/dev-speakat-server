using Speakat.Application.Quests.Dtos;

namespace Speakat.Application.Quests.Services;

public interface IQuestService
{
    Task<QuestDetailDto> GetQuestDetailAsync(long questId, string userUuid);
}