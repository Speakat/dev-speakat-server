public class EvaluateService : IEvaluateService
{
    private const int PassScore = 70; //고정값 사용

    private readonly IAiPipelineClient _ai; //ai 평가 파이프라인 호출
    private readonly IQuestRepository _questRepo; //퀘스트 데이터 조회

    public EvaluateService(IAiPipelineClient ai, IQuestRepository questRepo)
    {
        _ai = ai;
        _questRepo = questRepo;
    }

    public async Task<EvaluateRequestDto> EvaluateAsync(EvaluateRequestDto request)
    {
        var aiResult = await _ai.EvaluateAsync(request.QuestId); //ai 평가 요청

        await _questRepo.GetByIdAsync(request.QuestId); //존재 검증용
        bool isTurnPassed = aiResult.SimilarityPassed && aiResult.Score >= PassScore;
        // bool isQuestComplete = isTurnPassed && request.IsLastTurn;
        bool isQuestComplete = isTurnPassed; //각 턴 통과 시 결과 반환

        return new EvaluateResponseDto(
            Roleplay:             aiResult.Roleplay,
            Score:                aiResult.Score,
            Grade:                aiResult.Grade,
            BetterSuggestions:    aiResult.BetterSuggestions,
            RecommendationReason: aiResult.RecommendationReason,
            IsTurnPassed:         isTurnPassed,
            IsQuestComplete:      isQuestComplete
        );
    }
}