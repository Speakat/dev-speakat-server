using Speakat.Application.Auth.Repositories;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Users.Dtos;
using Speakat.Application.Users.Repositories;
using Speakat.Domain.Enums;

namespace Speakat.Application.Users.Services;

public class UserService(
    IUserRepository userRepository,
    IUserProfileRepository userProfileRepository,
    IUserSettingsRepository userSettingsRepository) : IUserService
{
    public async Task<UserProfileDto> GetProfileAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw new UnauthorizedAccessException();

        var data = await userProfileRepository.GetProfileDataAsync(userId)
            ?? throw new UnauthorizedAccessException();

        return new UserProfileDto
        {
            UserId = data.UserUuid,
            Nickname = data.Nickname,
            ProfileImageUrl = null, // TODO: S3 이미지 업로드 구현 후 ProfileImageKey → URL 변환
            EnglishLevel = CalculateEnglishLevel(data.AvgSemanticScore, data.AvgGrammarScore, data.AvgNaturalnessScore)
        };
    }

    public async Task<PatchUserResultDto> UpdateProfileAsync(string userUuid, string? nickname, string? profileImageKey)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw new UnauthorizedAccessException();

        if (nickname is not null && await userRepository.ExistsNicknameAsync(nickname))
            throw UserException.DuplicateNickname();

        var (uuid, updatedNickname, _) = await userProfileRepository.UpdateProfileAsync(userId, nickname, profileImageKey);

        return new PatchUserResultDto
        {
            UserId = uuid,
            Nickname = updatedNickname,
            ProfileImageUrl = null // TODO: S3 이미지 업로드 구현 후 ProfileImageKey → URL 변환
        };
    }

    public async Task DeleteAccountAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw new UnauthorizedAccessException();
        await userRepository.SoftDeleteAsync(userId);
    }

    public async Task<UserSettingsDto> GetSettingsAsync(string userUuid)
    {
        var userId = await userRepository.FindUserIdByUuidAsync(userUuid) ?? throw new UnauthorizedAccessException();

        var settings = await userSettingsRepository.GetByUserIdAsync(userId)
            ?? throw new UnauthorizedAccessException();

        return new UserSettingsDto
        {
            ShowNpcScript = settings.ShowNpcScript,
            StreakGoal = settings.StreakGoal
        };
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
