using UserWebApi.Models;

namespace UserWebApi.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ICollection<User>> GetUsersAsync(int offset, int pageSize, int maxPageSize = 10);
    Task<User> GetUserAsync(int id);
    Task<bool> IsActiveAdminExistsAsync();
    Task<bool> IsUserExistsAsync(int id);
    Task<bool> IsRecentUserExists(string login);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> SaveAsync();
}