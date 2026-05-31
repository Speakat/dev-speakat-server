namespace Speakat.Application.Common.Exceptions;

public class AiServiceException : BusinessException
{
    private AiServiceException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static AiServiceException Unavailable() =>
        new("AI_SERVICE_ERROR", "AI 서비스가 일시적으로 이용 불가능합니다.", 503);
}
