public interface IEvaluateService
{
    Task<EvaluateResponseDto> EvaluateAsync(EvaluateRequestDto request);
}