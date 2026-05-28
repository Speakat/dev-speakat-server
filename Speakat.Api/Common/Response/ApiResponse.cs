using System.Text.Json.Serialization;

namespace Speakat.Api.Common.Response;

// API 공통 응답 래퍼
public class ApiResponse<T>
{
    public bool IsSuccess { get; init; }

    // 성공 시 응답 데이터
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    // 실패 시 에러 코드
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }

    // 실패 시 에러 메시지
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    private ApiResponse() { }

    // 성공 응답 생성
    public static ApiResponse<T> Success(T data) =>
        new() { IsSuccess = true, Data = data };

    // 실패 응답 생성
    public static ApiResponse<T> Fail(string code, string message) =>
        new() { IsSuccess = false, Code = code, Message = message };
}
