public interface IFlashcardRepository
{
    Task SaveAsync(long userId, long questId, TurnEvaluationResult evaluationResult);
}