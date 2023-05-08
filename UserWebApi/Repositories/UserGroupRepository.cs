using Microsoft.EntityFrameworkCore;
using UserWebApi.Data;
using UserWebApi.Models;
using UserWebApi.Repositories.Interfaces;

namespace UserWebApi.Repositories;

public class UserGroupRepository : IUserGroupRepository
{
    private readonly DataContext _context;

    public UserGroupRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ICollection<UserGroup>> GetUserGroupsAsync()
    {
        return await _context.UserGroups.OrderBy(g => g.Id).ToListAsync();
    }

    public async Task<UserGroup> GetUserGroupAsync(int id)
    {
        return await _context.UserGroups.FirstAsync(g => g.Id == id);
    }

    public async Task<UserGroup> GetUserGroupAsync(string code)
    {
        return await _context.UserGroups.FirstAsync(g => g.Code == code);
    }

    public async Task<bool> IsUserGroupExistsAsync(int id)
    {
        return await _context.UserGroups.AnyAsync(g => g.Id == id);
    }

    public async Task<bool> IsUserGroupExistsAsync(string code)
    {
        return await _context.UserGroups.AnyAsync(g => g.Code == code);
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}