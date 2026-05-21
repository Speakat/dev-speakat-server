using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Application.Evaluate.Services;

public class EvaluateService(IAiPipelineClient ai, IQuestDataService questData, ISessionStore sessionStore, IGameSessionRepository gameSessionRepo, IFlashcardRepository flashcardRepo, IUserStageRepository userStageRepo) : IEvaluateService
{
    private const float PassThreshold = 0.7f;

    public async Task<EvaluateResponseDto> EvaluateAsync(EvaluateRequestDto request, string sessionId, long userId)
    {
        var (sessionUserId, sessionQuestId) = await sessionStore.GetAsync(sessionId)
            ?? throw SessionException.NotFound();

        if (sessionUserId != userId || sessionQuestId != request.QuestId)
            throw SessionException.Forbidden();

        var questPrompt = await questData.GetQuestPromptDtoAsync((int)request.QuestId);

        var aiResult = await ai.EvaluateAsync(request.Audio, request.QuestId, sessionId, request.Turn, questPrompt);
        bool isTurnPassed = aiResult.SimilarityPassed && aiResult.TurnEvaluation.ContextRelevance >= PassThreshold;

        await flashcardRepo.SaveAsync(userId, request.QuestId, aiResult.TurnEvaluation);

        if (aiResult.QuestResult is not null)
            if(aiResult.QuestResult.IsQuestSuccess)
            {
                await gameSessionRepo.CompleteAsync(sessionId, request.QuestId, aiResult.QuestResult);
                await userStageRepo.TryCompleteAsync(userId, request.QuestId);
            }
            else
                await gameSessionRepo.FailedAsync(sessionId, request.QuestId, aiResult.QuestResult);

        return new EvaluateResponseDto(
            NpcDialogue:      aiResult.NpcDialogue,
            NpcDialogueAudio: aiResult.NpcDialogueAudio,
            IsTurnPassed:     isTurnPassed,
            TurnEvaluation:   aiResult.TurnEvaluation,
            QuestResult:      aiResult.QuestResult
        );
    }
}
