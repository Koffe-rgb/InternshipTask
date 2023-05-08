using UserWebApi.Models;

namespace UserWebApi.Repositories.Interfaces;

public interface IUserStateRepository
{
    Task<ICollection<UserState>> GetUserStatesAsync();
    Task<UserState> GetUserStateAsync(int id);
    Task<UserState> GetUserStateAsync(string code);
    Task<bool> IsUserStateExistsAsync(int id);
    Task<bool> IsUserStateExistsAsync(string code);
    Task<bool> SaveAsync();
}