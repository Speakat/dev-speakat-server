namespace Speakat.Application.Auth.Dtos;

public class CheckNicknameResponseDto
{
    public bool Available { get; set; }
    public string? Suggestion { get; set; }
}