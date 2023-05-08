using Microsoft.EntityFrameworkCore;
using UserWebApi.Data;
using UserWebApi.Models;
using UserWebApi.Repositories.Interfaces;

namespace UserWebApi.Repositories;

public class UserStateRepository : IUserStateRepository
{
    private readonly DataContext _context;

    public UserStateRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ICollection<UserState>> GetUserStatesAsync()
    {
        return await _context.UserStates.OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<UserState> GetUserStateAsync(int id)
    {
        return await _context.UserStates.FirstAsync(s => s.Id == id);
    }

    public async Task<UserState> GetUserStateAsync(string code)
    {
        return await _context.UserStates.FirstAsync(s => s.Code == code);
    }

    public async Task<bool> IsUserStateExistsAsync(int id)
    {
        return await _context.UserStates.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> IsUserStateExistsAsync(string code)
    {
        return await _context.UserStates.AnyAsync(s => s.Code == code);
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}