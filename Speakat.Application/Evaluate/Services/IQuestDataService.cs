public interface IQuestDataService
{
    Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId);
}