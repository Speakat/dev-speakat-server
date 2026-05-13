public interface IEvaluateService
{
    Task<EvaluateResponseDto> EvaluateAsync(EvaluateRequestDto request, string sessionId, long userId);
}