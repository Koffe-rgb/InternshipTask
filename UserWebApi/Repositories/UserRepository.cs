using Microsoft.EntityFrameworkCore;
using UserWebApi.Data;
using UserWebApi.Models;
using UserWebApi.Repositories.Interfaces;

namespace UserWebApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }
    
    // используется Offset pagination
    public async Task<ICollection<User>> GetUsersAsync(int offset, int pageSize, int maxPageSize = 10)
    {
        if (pageSize > maxPageSize) 
            pageSize = maxPageSize;
        
        return await _context.Users
            .Skip(offset)
            .Take(pageSize)
            .OrderBy(u => u.Id)
            .Include(u => u.UserState)
            .Include(u => u.UserGroup)
            .ToListAsync();
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _context.Users
            .Include(u => u.UserState)
            .Include(u => u.UserGroup)
            .FirstAsync(u => id == u.Id);
    }

    public async Task<bool> IsActiveAdminExistsAsync()
    {
        return await _context.Users
            .Where(u => u.UserState.Code == UserState.CodeStringValue(StateCode.Active))
            .AnyAsync(u => u.UserGroup.Code == UserGroup.CodeStringValue(GroupCode.Admin));
    }

    public async Task<bool> IsUserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => id == u.Id);
    }

    public async Task<bool> IsRecentUserExists(string login)
    {
        return await _context.Users
            .AnyAsync(u => 
                u.Login == login && 
                u.CreatedDate >= DateTime.UtcNow.AddSeconds(-5));
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        await _context.AddAsync(user);
        return await SaveAsync();
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        _context.Update(user);
        return await SaveAsync();
    }


    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}