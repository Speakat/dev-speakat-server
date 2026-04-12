using Speakat.Domain.Entities;
using Speakat.Domain.Enums;

namespace Speakat.Application.Users.Repositories;

public interface IUserRepository
{
    // 특정 유저 찾기
    Task<User?> FindBySocialTypeAndSocialIdAsync(SocialType socialType, string socialId);
    
    // 신규 유저는 등록, 기존 유저는 업데이트
    Task<User> SaveAsync(User user);
}