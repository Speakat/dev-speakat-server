public class MockQuestDataService : IQuestDataService
{
    public Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId)
    {
        //엔티티 없이 하드코딩 반환(테스트용)
        var mock = new QuestPromptDto(
            Scenario: "A customer walks into a coffee shop on a sunny morning.",
            SuccessCriteria: "Order a drink and complete the payment interaction.",
            Npc: new NpcDto(
                Name: "Emma",
                Role: "barista at a busy downtown coffee shop",
                Tone: "friendly and upbeat"
            ),
            Objectives: new List<ObjectiveDto>
            {
                new("order_drink", "Order an iced latte"),
                new("order_food",  "Order a blueberry muffin"),
                new("pay",         "Complete the payment")
            }
        );

        return Task.FromResult(mock);
    }
}