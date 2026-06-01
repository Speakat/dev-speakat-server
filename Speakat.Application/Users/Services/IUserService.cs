using Speakat.Application.Users.Dtos;
using Speakat.Domain.Enums;

namespace Speakat.Application.Users.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userUuid);
    Task<PatchUserResultDto> UpdateProfileAsync(string userUuid, string? nickname, string? profileImageKey);
    Task<UploadUrlDto> GetImageUploadUrlAsync(string userUuid, ImageType type);
    
    Task DeleteAccountAsync(string userUuid);
    
    Task<UserSettingsDto> GetSettingsAsync(string userUuid);
    Task<UserSettingsDto> UpdateSettingsAsync(string userUuid, bool? showNpcScript, int? streakGoal);

    Task<UserStatsDto> GetStatsAsync(string userUuid);
    Task<UserStreakDto> GetStreakAsync(string userUuid);
    Task<UserCalendarDto> GetCalendarAsync(string userUuid, int year, int month);
}
