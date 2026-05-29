using Speakat.Application.Users.Dtos;

namespace Speakat.Application.Users.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userUuid);

    Task<PatchUserResultDto> UpdateProfileAsync(string userUuid, string? nickname, string? profileImageKey);

    Task DeleteAccountAsync(string userUuid);

    Task<UserSettingsDto> GetSettingsAsync(string userUuid);

    Task<UserStatsDto> GetStatsAsync(string userUuid);

    Task<UserStreakDto> GetStreakAsync(string userUuid);
}
