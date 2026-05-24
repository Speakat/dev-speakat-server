using Microsoft.EntityFrameworkCore;
using Speakat.Application.Auth.Repositories;
using Speakat.Domain.Entities;
using Speakat.Domain.Enums;

namespace Speakat.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> FindByUuidAsync(string userUuid)
        => await _context.Users.FirstOrDefaultAsync(u => u.UserUuid == userUuid);

    public async Task<User?> FindBySocialTypeAndSocialIdAsync(SocialType socialType, string socialId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.SocialType == socialType && u.SocialId == socialId);
    }
    
    // 신규 유저는 등록, 기존 유저는 업데이트
    public async Task<User> SaveAsync(User user)
    {
        var existing = await FindBySocialTypeAndSocialIdAsync(user.SocialType, user.SocialId);
        
        if (existing != null)
        {
            // 기존 유저: Update
            _context.Users.Update(user);
        }
        else
        {
            // 신규 유저: Add
            user.UserUuid = Guid.NewGuid().ToString();
            await _context.Users.AddAsync(user);
        }
        
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<long?> FindUserIdByUuidAsync(string userUuid)
    {
        return await _context.Users
            .Where(u => u.UserUuid == userUuid)
            .Select(u => (long?)u.UserId)
            .FirstOrDefaultAsync();
    }
    
    // 존재하는 닉네임인지 찾기
    public Task<bool> ExistsNicknameAsync(string nickname) =>
        _context.Users.AnyAsync(u => u.Nickname == nickname);
}