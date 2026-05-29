using Speakat.Domain.Enums;

namespace Speakat.Application.Users.Dtos;

public class UserProfileDto
{
    public string UserId { get; init; } = null!;
    public string Nickname { get; init; } = null!;
    public string? ProfileImageUrl { get; init; }
    public EnglishLevel EnglishLevel { get; init; }
}
