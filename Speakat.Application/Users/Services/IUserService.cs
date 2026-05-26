using Speakat.Application.Users.Dtos;

namespace Speakat.Application.Users.Services;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userUuid);
}
