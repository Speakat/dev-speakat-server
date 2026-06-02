using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speakat.Api.Common.Response;
using Speakat.Application.Flashcards.Dtos;
using Speakat.Application.Flashcards.Services;

namespace Speakat.Api.Controllers;

[ApiController]
[Route("flashcards")]
[Authorize]
public class FlashcardsController : ControllerBase
{
    private readonly IFlashcardService _flashcardService;

    public FlashcardsController(IFlashcardService flashcardService) => _flashcardService = flashcardService;

    private string UserUuid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FlashcardDetailDto>>> SaveFlashcardAsync(
        [FromBody] SaveFlashcardRequestDto request)
    {
        var result = await _flashcardService.SaveFlashcardAsync(UserUuid, request);
        return Ok(ApiResponse<FlashcardDetailDto>.Success(result));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<FlashcardListDto>>> GetFlashcardsAsync(
        [FromQuery] string? cursor,
        [FromQuery] int size = 20,
        [FromQuery] long? questId = null)
    {
        var result = await _flashcardService.GetFlashcardsAsync(UserUuid, cursor, size, questId);
        return Ok(ApiResponse<FlashcardListDto>.Success(result));
    }

    [HttpGet("{flashcardId}")]
    public async Task<ActionResult<ApiResponse<FlashcardDetailDto>>> GetFlashcardDetailAsync(long flashcardId)
    {
        var result = await _flashcardService.GetFlashcardDetailAsync(UserUuid, flashcardId);
        return Ok(ApiResponse<FlashcardDetailDto>.Success(result));
    }

    [HttpPatch("{flashcardId}")]
    public async Task<ActionResult<ApiResponse<PatchFlashcardResultDto>>> PatchFlashcardAsync(
        long flashcardId,
        [FromBody] PatchFlashcardRequestDto request)
    {
        var result = await _flashcardService.UpdateIsMasteredAsync(UserUuid, flashcardId, request.IsMastered);
        return Ok(ApiResponse<PatchFlashcardResultDto>.Success(result));
    }
}
