using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;
using UserWebApi.Models.Dtos;
using UserWebApi.Test.Common;

namespace UserWebApi.Test.Controller;

public class UserControllerTests : ControllerTestBase
{
    [Fact]
    public async void GetUsers_ReturnsUsersOfPageSize()
    {
        // Arrange
        var offset = 0;
        var pageSize = 2;

        // Act
        var actionResult = await UserController.GetUsers(offset, pageSize);
        
        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(result.Value);
        var value = Assert.IsAssignableFrom<ICollection<User>>(result.Value);
        Assert.Equal(pageSize, value.Count);
    }

    [Fact]
    public async void GetUsers_ReturnsBadRequest_OnWrongOffset()
    {
        // Arrange
        var offset = -1;
        var pageSize = 2;

        // Act
        var actionResult = await UserController.GetUsers(offset, pageSize);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
    
    [Fact]
    public async void GetUsers_ReturnsBadRequest_OnWrongPageSize()
    {
        // Arrange
        var offset = 0;
        var pageSize = 0;

        // Act
        var actionResult = await UserController.GetUsers(offset, pageSize);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async void GetUser_ReturnsUser()
    {
        // Arrange
        var userId = 1;

        // Act
        var expectedUser = await Context.Users.FirstAsync(u => u.Id == userId);
        var actionResult = await UserController.GetUser(userId);
        
        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(result.Value);
        var user = Assert.IsType<User>(result.Value);
        Assert.Equal(expectedUser, user);
    }

    [Fact]
    public async void GetUser_ReturnsNotFound_OnWrongId()
    {
        // Arrange
        var userId = -1; // в БД хранятся только пользователи с Id: 1, 2, 3

        // Act
        var actionResult = await UserController.GetUser(userId);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async void CreateUser_ReturnsOk_OnCreationOfNotAdmin()
    {
        // Arrange
        var userDto = new UserDto
        {
            Login = "completly new user",
            GroupCode = UserGroup.CodeStringValue(GroupCode.User),
            Password = "123"
        };

        // Act
        var actionResult = await UserController.CreateUser(userDto);

        // Assert
        Assert.IsType<OkResult>(actionResult);
        var expectedUserToAdd = await Context.Users.FirstAsync(u =>
            u.Login == userDto.Login &&
            u.UserGroup.Code == UserGroup.CodeStringValue(GroupCode.User) &&
            u.Password == userDto.Password &&
            u.UserState.Code == UserState.CodeStringValue(StateCode.Active)
        );
        Assert.NotNull(expectedUserToAdd);
    }
    
    [Fact]
    public async void CreateUser_ReturnsStatusCodeUnprocessableEntity_OnNull()
    {
        // Arrange
        UserDto userDto = null;

        // Act
        var actionResult = await UserController.CreateUser(userDto);

        // Assert
        Assert.IsType<UnprocessableEntityObjectResult>(actionResult);
    }
    
    [Fact]
    public async void CreateUser_ReturnsBadRequest_OnWrongGroupCode()
    {
        // Arrange
        var userDto = new UserDto
        {
            Login = "completly new user",
            GroupCode = "new group code",
            Password = "123"
        };

        // Act
        var actionResult = await UserController.CreateUser(userDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async void CreateUser_ReturnsBadRequest_OnCreationOfAdminWhileAdminExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Login = "completly new user",
            GroupCode = UserGroup.CodeStringValue(GroupCode.Admin),
            Password = "123"
        };

        // Act
        var actionResult = await UserController.CreateUser(userDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
    
    [Fact]
    public async void CreateUser_ReturnsOk_OnCreationOfAdminWhileAdminNotExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Login = "completly new user",
            GroupCode = UserGroup.CodeStringValue(GroupCode.Admin),
            Password = "123"
        };

        // Act
        var blockedState = await Context.UserStates.FirstAsync(s => 
            s.Code == UserState.CodeStringValue(StateCode.Blocked));
        var adminToBlock = await Context.Users.FirstAsync(u =>
            u.UserGroup.Code == UserGroup.CodeStringValue(GroupCode.Admin) &&
            u.UserState.Code == UserState.CodeStringValue(StateCode.Active));
        adminToBlock.UserState = blockedState;
        await Context.SaveChangesAsync();

        var actionResult = await UserController.CreateUser(userDto);

        // Assert
        Assert.IsType<OkResult>(actionResult);
    }

    [Fact]
    public async void CreateUser_ReturnsBadRequest_OnCreationInLast5Seconds()
    {
        // Arrange
        var sameUser = new UserDto
        {
            Login = "same login",
            GroupCode = UserGroup.CodeStringValue(GroupCode.User),
            Password = "pass"
        };

        // Act
        var actionResultA = await UserController.CreateUser(sameUser);
        Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(0, 5)));
        var actionResultB = await UserController.CreateUser(sameUser);

        // Assert
        Assert.IsType<OkResult>(actionResultA);
        Assert.IsType<BadRequestObjectResult>(actionResultB);
    }
    
    [Fact]
    public async void CreateUser_ReturnsOk_OnCreationAfter5Seconds()
    {
        // Arrange
        var sameUser = new UserDto
        {
            Login = "same login",
            GroupCode = UserGroup.CodeStringValue(GroupCode.User),
            Password = "pass"
        };

        // Act
        var actionResultA = await UserController.CreateUser(sameUser);
        Thread.Sleep(TimeSpan.FromSeconds(5));
        var actionResultB = await UserController.CreateUser(sameUser);

        // Assert
        Assert.IsType<OkResult>(actionResultA);
        Assert.IsType<OkResult>(actionResultB);
    }

    [Fact]
    public async void DeleteUser_ReturnsNoContent()
    {
        // Arrange
        var userId = 2;

        // Act
        var actionResult = await UserController.DeleteUser(userId);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
        var userToBlock = await Context.Users.FirstAsync(u => u.Id == userId);
        Assert.NotNull(userToBlock);
        Assert.Equal(UserState.CodeStringValue(StateCode.Blocked), userToBlock.UserState.Code);
    }
    
    [Fact]
    public async void DeleteUser_ReturnsNotFound_OnWrongId()
    {
        // Arrange
        var userId = -1;

        // Act
        var actionResult = await UserController.DeleteUser(userId);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }
    
    [Fact]
    public async void DeleteUser_ReturnsBadRequest_WhenUserAlreadyBlocked()
    {
        // Arrange
        var userId = 3; // Пользователь с Id = 3 уже заблокирован

        // Act
        var actionResult = await UserController.DeleteUser(userId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);
    }
}