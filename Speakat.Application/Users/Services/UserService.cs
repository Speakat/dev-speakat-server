using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Common.Interfaces;
using Speakat.Application.Users.Dtos;
using Speakat.Application.Users.Repositories;
using Speakat.Domain.Enums;

namespace Speakat.Application.Users.Services;

public class UserService(
    IUserRepository userRepository,
    IUserProfileRepository userProfileRepository,
    IImageStorageService imageStorageService,
    IUserSettingsRepository userSettingsRepository,
    IUserStatsRepository userStatsRepository,
    IUserStreakRepository userStreakRepository,
    IUserCalendarRepository userCalendarRepository) : IUserService
{
    public async Task<UserProfileDto> GetProfileAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var data = await userProfileRepository.GetProfileDataAsync(userId)
            ?? throw AuthException.UserNotFound();

        return new UserProfileDto
        {
            UserId = data.UserUuid,
            Nickname = data.Nickname,
            ProfileImageUrl = data.ProfileImageKey is not null ? 
                imageStorageService.GetPublicUrl(data.ProfileImageKey) : null,
            EnglishLevel = CalculateEnglishLevel(data.AvgSemanticScore, data.AvgGrammarScore, data.AvgNaturalnessScore)
        };
    }

    public async Task<PatchUserResultDto> UpdateProfileAsync(string userUuid, string? nickname, string? profileImageKey)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        if (nickname is not null && await userRepository.ExistsNicknameAsync(nickname))
            throw UserException.DuplicateNickname();

        var (uuid, updatedNickname, updatedImageKey) = await userProfileRepository.UpdateProfileAsync(userId, nickname, profileImageKey);

        return new PatchUserResultDto
        {
            UserId = uuid,
            Nickname = updatedNickname,
            ProfileImageUrl = updatedImageKey is not null ? 
                imageStorageService.GetPublicUrl(updatedImageKey) : null,
        };
    }

    private static readonly Dictionary<string, string> AllowedContentTypes = new()
    {
        ["image/jpeg"] = "jpg",
        ["image/png"]  = "png",
    };

    public async Task<UploadUrlDto> GetImageUploadUrlAsync(string userUuid, ImageType type, string contentType)
    {
        if (!AllowedContentTypes.TryGetValue(contentType, out var ext))
            throw UserException.InvalidImageFormat();

        _ = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var key = ImageKey(userUuid, type, ext);
        var uploadUrl = await imageStorageService.GenerateUploadUrlAsync(key, contentType);

        return new UploadUrlDto
        {
            UploadUrl = uploadUrl,
            Key = key
        };
    }

    private static string ImageKey(string userUuid, ImageType type, string ext) => type switch
    {
        ImageType.Profile => $"profile-images/{userUuid}.{ext}",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    public async Task DeleteAccountAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();
        await userRepository.SoftDeleteAsync(userId);
    }

    public async Task<UserSettingsDto> GetSettingsAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var settings = await userSettingsRepository.GetByUserIdAsync(userId)
            ?? throw AuthException.UserNotFound();

        return new UserSettingsDto
        {
            ShowNpcScript = settings.ShowNpcScript,
            StreakGoal = settings.StreakGoal
        };
    }

    public async Task<UserSettingsDto> UpdateSettingsAsync(string userUuid, bool? showNpcScript, int? streakGoal)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var settings = await userSettingsRepository.UpdateAsync(userId, showNpcScript, streakGoal);

        return new UserSettingsDto
        {
            ShowNpcScript = settings.ShowNpcScript,
            StreakGoal = settings.StreakGoal
        };
    }

    public async Task<UserStatsDto> GetStatsAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var data = await userStatsRepository.GetStatsAsync(userId);

        return new UserStatsDto
        {
            TotalQuestsCompleted = data.TotalQuestsCompleted,
            TotalSessionsPlayed = data.TotalSessionsPlayed,
            TotalStagesCompleted = data.TotalStagesCompleted,
            AvgSemanticScore = data.AvgSemanticScore,
            AvgGrammarScore = data.AvgGrammarScore,
            AvgNaturalnessScore = data.AvgNaturalnessScore
        };
    }

    public async Task<UserStreakDto> GetStreakAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var dates = await userStreakRepository.GetCompletedSessionDatesAsync(userId);
        var streakGoal = await userStreakRepository.GetStreakGoalAsync(userId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var (currentStreak, longestStreak) = CalculateStreaks(dates, today);

        return new UserStreakDto
        {
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            TodayCompleted = dates.Contains(today),
            StreakGoal = streakGoal
        };
    }

    public async Task<UserCalendarDto> GetCalendarAsync(string userUuid, int year, int month)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw AuthException.UserNotFound();

        var days = await userCalendarRepository.GetMonthlyActivityAsync(userId, year, month);

        return new UserCalendarDto
        {
            Year = year,
            Month = month,
            Days = days.Select(d => new CalendarDayDto { Date = d.Date, SessionCount = d.SessionCount }).ToList()
        };
    }

    private static (int Current, int Longest) CalculateStreaks(IReadOnlyList<DateOnly> dates, DateOnly today)
    {
        if (dates.Count == 0) return (0, 0);

        var sorted = dates.OrderByDescending(d => d).ToList();

        // Current streak: consecutive days ending today or yesterday
        var currentStreak = 0;
        var mostRecent = sorted[0];
        if (mostRecent == today || mostRecent == today.AddDays(-1))
        {
            var expected = mostRecent;
            foreach (var date in sorted)
            {
                if (date != expected) break;
                currentStreak++;
                expected = expected.AddDays(-1);
            }
        }

        // Longest streak: max consecutive run across all history
        var longestStreak = 0;
        var runLength = 0;
        DateOnly? prev = null;
        foreach (var date in sorted.OrderBy(d => d))
        {
            runLength = prev is not null && date == prev.Value.AddDays(1) ? runLength + 1 : 1;
            if (runLength > longestStreak) longestStreak = runLength;
            prev = date;
        }

        return (currentStreak, longestStreak);
    }

    private static EnglishLevel CalculateEnglishLevel(double? semantic, double? grammar, double? naturalness)
    {
        var values = new[] { semantic, grammar, naturalness }
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

        if (values.Count == 0)
            return EnglishLevel.BEGINNER;

        var avg = values.Average();

        return avg switch
        {
            < 40 => EnglishLevel.BEGINNER,
            < 60 => EnglishLevel.ELEMENTARY,
            < 75 => EnglishLevel.INTERMEDIATE,
            < 88 => EnglishLevel.UPPER_INTERMEDIATE,
            _ => EnglishLevel.ADVANCED
        };
    }
}
