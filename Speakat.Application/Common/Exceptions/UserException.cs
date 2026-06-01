namespace Speakat.Application.Common.Exceptions;

public class UserException : BusinessException
{
    private UserException(string code, string message, int statusCode) : base(code, message, statusCode) { }

    public static UserException DuplicateNickname() =>
        new("DUPLICATE_NICKNAME", "이미 존재하는 닉네임", 409);

    public static UserException InvalidRequest() =>
        new("INVALID_REQUEST", "닉네임 형식 오류", 400);

    public static UserException InvalidImageFormat() =>
        new("INVALID_IMAGE_FORMAT", "지원하지 않는 이미지 형식입니다. (jpg, png만 허용)", 400);
}
