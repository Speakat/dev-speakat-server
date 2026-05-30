namespace Speakat.Application.Evaluate.Services;

public class MockQuestDataService : IQuestDataService
{
    public Task<QuestPromptDto> GetQuestPromptDtoAsync(int questId)
    {
        var mock = new QuestPromptDto(
            Scenario: "A customer walks into a coffee shop on a sunny morning.",
            SuccessCriteria: "Order a drink and complete the payment interaction.",
            Npc: new NpcDto(
                Name: "Emma",
                Role: "barista at a busy downtown coffee shop",
                Tone: "friendly and upbeat",
                Voice: "alloy"
            ),
            Objectives:
            [
                new("order_drink", "Order an iced latte"),
                new("order_food",  "Order a blueberry muffin"),
                new("pay",         "Complete the payment")
            ],
            ReferenceSentences: ["I'd like an iced latte please", "Can I get a blueberry muffin"]
        );

        return Task.FromResult(mock);
    }

    public Task<string> GetOpeningLineAsync(int questId)
    {
        return Task.FromResult("Hi there! Welcome to the coffee shop. What can I get for you today?");
    }
}