namespace Speakat.Application.Common.Exceptions;

public class StageException : BusinessException
{
    private StageException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static StageException NotFound() =>
        new("STAGE_NOT_FOUND", "존재하지 않는 스테이지", 404);
}
