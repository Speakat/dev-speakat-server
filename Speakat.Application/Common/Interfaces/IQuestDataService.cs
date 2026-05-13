public interface IQuestDataService
{
    Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId);
    Task<string> GetOpeningLineAsync(int questId);
}