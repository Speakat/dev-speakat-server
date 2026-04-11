[ApiController]
[Route("evaluate")]
public class EvaluateController : ControllerBase
{
    private readonly IEvaluateService _evaluateService;
    public EvaluateController(IEvaluateService evaluateService)
        => _evaluateService = evaluateService;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EvaluateResponseDto>>> EvaluateAsync(
        [FromBody] EvaluateRequestDto request)
    {
        try
        {
            var result = await _evaluateService.EvaluateAsync(request);
            return Ok(ApiResponse<EvaluateResponseDto>.Success(result));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<EvaluateResponseDto>.Fail("NOT_FOUND", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<EvaluateResponseDto>.Fail("EVALUATE_ERROR", ex.Message));
        }
    }
}