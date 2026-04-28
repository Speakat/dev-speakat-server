namespace Speakat.Application.Common.Exceptions;

public abstract class BusinessException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    protected BusinessException(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}