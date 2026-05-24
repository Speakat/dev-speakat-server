namespace Speakat.Application.Common.Exceptions;

public class FlashcardException : BusinessException
{
    private FlashcardException(string code, string message, int statusCode) : base(code, message, statusCode) { }
    
    public static FlashcardException NotFound() =>
        new("FLASHCARD_NOT_FOUND", "존재하지 않는 플래시카드", 404);
}