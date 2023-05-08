using Microsoft.EntityFrameworkCore;
using UserWebApi.Data;
using UserWebApi.Models;

namespace UserWebApi.Test.Common;

public static class DataContextFactory
{
    public static DataContext Create()
    {
        // создание in-memory БД
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        // наполнение тестовыми данными
        PopulateDatabase(context);

        return context;
    }

    public static void Destroy(DataContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    private static void PopulateDatabase(DataContext context)
    {
        // объекты UserState и UserGroup вставляются сами при создании БД 
        
        var states = context.UserStates.ToList();
        var groups = context.UserGroups.ToList();

        var activeState = states.First(u => u.Code == UserState.CodeStringValue(StateCode.Active));
        var blockedState = states.First(u => u.Code == UserState.CodeStringValue(StateCode.Blocked));

        var adminGroup = groups.First(g => g.Code == UserGroup.CodeStringValue(GroupCode.Admin));
        var userGroup = groups.First(g => g.Code == UserGroup.CodeStringValue(GroupCode.User));

        context.Users.AddRange(
            new User
            {
                Id = 1,
                Login = "Active Admin",
                Password = "pass",
                UserState = activeState,
                UserGroup = adminGroup,
                CreatedDate = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Login = "Active User 1",
                Password = "не скажу",
                UserState = activeState,
                UserGroup = userGroup,
                CreatedDate = DateTime.UtcNow.AddDays(5)
            },
            new User
            {
                Id = 3,
                Login = "Blocked User 1",
                Password = "aaaa",
                UserState = blockedState,
                UserGroup = userGroup,
                CreatedDate = DateTime.UtcNow.AddMonths(-3)
            }
        );
        context.SaveChanges();
    }
}