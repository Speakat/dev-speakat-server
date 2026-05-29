namespace Speakat.Application.Users.Dtos;

public class PatchUserResultDto
{
    public string UserId { get; init; } = null!;
    public string Nickname { get; init; } = null!;
    public string? ProfileImageUrl { get; init; }
}
