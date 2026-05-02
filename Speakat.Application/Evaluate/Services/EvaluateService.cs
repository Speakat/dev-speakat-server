public class EvaluateService : IEvaluateService
{
    private const float PassThreshold = 0.7f;

    private readonly IAiPipelineClient _ai;
    private readonly IQuestDataService _questData;

    public EvaluateService(IAiPipelineClient ai, IQuestDataService questData)
    {
        _ai = ai;
        _questData = questData;
    }

    public async Task<EvaluateResponseDto> EvaluateAsync(EvaluateRequestDto request)
    {
        var questPrompt = await _questData.GetQuestPromptDtoAsync((int)request.QuestId);

        var aiResult = await _ai.EvaluateAsync(request.Audio, request.QuestId, request.SessionId, request.Turn, questPrompt);
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
