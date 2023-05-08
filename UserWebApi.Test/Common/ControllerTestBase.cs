using UserWebApi.Controllers;
using UserWebApi.Data;
using UserWebApi.Repositories;

namespace UserWebApi.Test.Common;

public class ControllerTestBase : IDisposable
{
    protected readonly DataContext Context;
    protected readonly UserController UserController;

    protected ControllerTestBase()
    {
        Context = DataContextFactory.Create();
        var userRepository = new UserRepository(Context);
        var stateRepository = new UserStateRepository(Context);
        var groupRepository = new UserGroupRepository(Context);
        
        UserController = new UserController(userRepository, groupRepository, stateRepository);
    }

    public void Dispose()
    {
        DataContextFactory.Destroy(Context);
        UserController.Dispose();
    }
}