namespace Speakat.Application.Common.Exceptions;

public class StageException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    private StageException(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public static StageException NotFound() =>
        new("STAGE_NOT_FOUND", "존재하지 않는 스테이지", 404);
}
