using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;

namespace UserWebApi.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        _ = Model;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserState> UserStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserGroup
        modelBuilder.Entity<UserGroup>()
            .HasIndex(g => g.Code)
            .IsUnique();
        modelBuilder.Entity<UserGroup>()
            .HasData(Enum.GetValues(typeof(GroupCode))
                .Cast<GroupCode>()
                .Select(groupCode => new UserGroup
                {
                    Id = (int)groupCode,
                    Code = UserGroup.CodeStringValue(groupCode),
                    Description = UserGroup.GroupDescription(groupCode)
                }));
        
        // UserState
        modelBuilder.Entity<UserState>()
            .HasIndex(s => s.Code)
            .IsUnique();
        modelBuilder.Entity<UserState>()
            .HasData(Enum.GetValues(typeof(StateCode))
                .Cast<StateCode>()
                .Select(stateCode => new UserState
                {
                    Id = (int)stateCode,
                    Code = UserState.CodeStringValue(stateCode),
                    Description = UserState.StateDescription(stateCode)
                }));
    }
}