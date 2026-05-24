namespace Speakat.Application.Common.Exceptions;

public class SessionException : BusinessException
{
    private SessionException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static SessionException NotFound() =>
        new("SESSION_NOT_FOUND", "세션을 찾을 수 없습니다.", 404);

    public static SessionException Forbidden() =>
        new("SESSION_FORBIDDEN", "해당 세션에 대한 권한이 없습니다.", 403);

    public static SessionException AlreadyEnded() =>
        new("SESSION_ALREADY_ENDED", "이미 종료된 세션입니다.", 409);
}
