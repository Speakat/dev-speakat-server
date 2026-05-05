using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;

namespace Speakat.Application.Evaluate.Services;

public class EvaluateService(IAiPipelineClient ai, IQuestDataService questData, ISessionStore sessionStore) : IEvaluateService
{
    private const float PassThreshold = 0.7f;

    public async Task<EvaluateResponseDto> EvaluateAsync(EvaluateRequestDto request, long userId)
    {
        var (sessionUserId, sessionQuestId) = await sessionStore.GetAsync(request.SessionId)
            ?? throw SessionException.NotFound();

        if (sessionUserId != userId || sessionQuestId != request.QuestId)
            throw SessionException.Forbidden();

        var questPrompt = await questData.GetQuestPromptDtoAsync((int)request.QuestId);

        var aiResult = await ai.EvaluateAsync(request.Audio, request.QuestId, request.SessionId, request.Turn, questPrompt);
        bool isTurnPassed = aiResult.SimilarityPassed && aiResult.TurnEvaluation.ContextRelevance >= PassThreshold;

        return new EvaluateResponseDto(
            NpcDialogue:      aiResult.NpcDialogue,
            NpcDialogueAudio: aiResult.NpcDialogueAudio,
            IsTurnPassed:     isTurnPassed,
            IsQuestComplete:  aiResult.TurnEvaluation.IsQuestComplete,
            TurnEvaluation:   aiResult.TurnEvaluation,
            QuestResult:      aiResult.QuestResult
        );
    }
}
