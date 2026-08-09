using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Application.Evaluate.Services;

public class EvaluateService(IAiPipelineClient ai, IQuestDataService questData, ISessionStore sessionStore, IGameSessionRepository gameSessionRepo, IUserStageRepository userStageRepo) : IEvaluateService
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
        bool isTurnPassed = aiResult.SimilarityPassed
            && aiResult.TurnEvaluation.ContextRelevance >= PassThreshold
            && aiResult.TurnEvaluation.GrammarAccuracy >= PassThreshold
            && aiResult.TurnEvaluation.ExpressionQuality >= PassThreshold;

        if (aiResult.QuestResult is not null)
            if(aiResult.QuestResult.IsQuestSuccess)
            {
                await gameSessionRepo.CompleteAsync(sessionId, request.QuestId, aiResult.QuestResult);
                await userStageRepo.TryCompleteAsync(userId, request.QuestId);
            }
            else
                await gameSessionRepo.FailedAsync(sessionId, request.QuestId, aiResult.QuestResult);

        var turnEvaluation = new TurnEvaluationDto(
            ContextRelevance:     aiResult.TurnEvaluation.ContextRelevance,
            GrammarAccuracy:      aiResult.TurnEvaluation.GrammarAccuracy,
            ExpressionQuality:    aiResult.TurnEvaluation.ExpressionQuality,
            ObjectiveProgress:    aiResult.TurnEvaluation.ObjectiveProgress,
            IsQuestComplete:      aiResult.TurnEvaluation.IsQuestComplete,
            Reason:               aiResult.TurnEvaluation.Reason,
            BetterSuggestions:    [..aiResult.TurnEvaluation.BetterSuggestions.Select(s => new BetterSuggestionDto(s.Word, s.Meaning))],
            RecommendationReason: aiResult.TurnEvaluation.RecommendationReason
        );

        var questResult = aiResult.QuestResult is { } qr ? new QuestResultDto(
            AverageContextRelevance:  qr.AverageContextRelevance,
            AverageGrammarAccuracy:   qr.AverageGrammarAccuracy,
            AverageExpressionQuality: qr.AverageExpressionQuality,
            AchievedObjectives:       qr.AchievedObjectives,
            IsQuestSuccess:           qr.IsQuestSuccess
        ) : null;

        return new EvaluateResponseDto(
            UserText:         aiResult.UserText,
            NpcDialogue:      aiResult.NpcDialogue,
            NpcDialogueAudio: aiResult.NpcDialogueAudio,
            IsTurnPassed:     isTurnPassed,
            TurnEvaluation:   turnEvaluation,
            QuestResult:      questResult
        );
    }
}
