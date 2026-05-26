using Speakat.Application.Users.Dtos;

namespace Speakat.Application.Users.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userUuid);

    Task<PatchUserResultDto> UpdateProfileAsync(string userUuid, string? nickname, string? profileImageKey);
}
