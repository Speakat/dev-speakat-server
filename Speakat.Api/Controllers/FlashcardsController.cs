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

    [HttpGet]
    public async Task<ActionResult<ApiResponse<FlashcardListDto>>> GetFlashcardsAsync(
        [FromQuery] string? cursor,
        [FromQuery] int size = 20,
        [FromQuery] long? questId = null)
    {
        var result = await _flashcardService.GetFlashcardsAsync(UserUuid, cursor, size, questId);
        return Ok(ApiResponse<FlashcardListDto>.Success(result));
    }
}
