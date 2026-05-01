using Microsoft.AspNetCore.Diagnostics;
using Speakat.Application.Common.Exceptions;
using Speakat.Api.Common.Response;

namespace Speakat.Api.Common.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, code, message) = exception switch
        {
            BusinessException ex => (ex.StatusCode, ex.Code, ex.Message),
            _ => (500, "INTERNAL_ERROR", "서버 내부 오류가 발생했습니다.")
        };
        
        if (statusCode == 500)
            logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(code, message!), cancellationToken);

        return true;
    }
}