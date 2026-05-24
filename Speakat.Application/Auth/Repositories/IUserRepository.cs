using Speakat.Domain.Entities;
using Speakat.Domain.Enums;

namespace Speakat.Application.Auth.Repositories;

public interface IUserRepository
{
    Task<User?> FindBySocialTypeAndSocialIdAsync(SocialType socialType, string socialId);
    Task<User?> FindByUuidAsync(string userUuid);
    
    // 신규 유저는 등록, 기존 유저는 업데이트
    Task<User> SaveAsync(User user);
    
    // 존재하는 닉네임인지 찾기
    Task<bool> ExistsNicknameAsync(string nickname);
}