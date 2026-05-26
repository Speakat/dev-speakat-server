namespace Speakat.Application.Users.Repositories;

public record UserProfileData(
    string UserUuid,
    string Nickname,
    string? ProfileImageKey,
    double? AvgSemanticScore,
    double? AvgGrammarScore,
    double? AvgNaturalnessScore
);

public interface IUserProfileRepository
{
    Task<UserProfileData?> GetProfileDataAsync(long userId);
}
