using UserWebApi.Models;

namespace UserWebApi.Repositories.Interfaces;

public interface IUserGroupRepository
{
    Task<ICollection<UserGroup>> GetUserGroupsAsync();
    Task<UserGroup> GetUserGroupAsync(int id);
    Task<UserGroup> GetUserGroupAsync(string code);
    Task<bool> IsUserGroupExistsAsync(int id);
    Task<bool> IsUserGroupExistsAsync(string code);
    Task<bool> SaveAsync();
}