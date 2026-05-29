using System.ComponentModel.DataAnnotations;

namespace Speakat.Application.Flashcards.Dtos;

public class PatchFlashcardRequestDto
{
    [Required]
    public required bool IsMastered { get; set; }
}
