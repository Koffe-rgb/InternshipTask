using Microsoft.AspNetCore.Mvc;
using UserWebApi.Models;
using UserWebApi.Models.Dtos;
using UserWebApi.Repositories.Interfaces;

namespace UserWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _groupRepository;
    private readonly IUserStateRepository _stateRepository;

    public UserController(IUserRepository userRepository, 
        IUserGroupRepository groupRepository, 
        IUserStateRepository stateRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _stateRepository = stateRepository;
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers(
        [FromQuery(Name = "offset")] int offset = 0, 
        [FromQuery(Name = "pageSize")] int pageSize = 10)
    {
        if (offset < 0)
            ModelState.AddModelError(nameof(offset), "Offset can not be negative");
        
        if (pageSize <= 0) 
            ModelState.AddModelError(nameof(pageSize), "Page size can not be negative or zero");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var users = await _userRepository.GetUsersAsync(offset, pageSize);
        
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Unexpected error while getting data from database");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
        
        return Ok(users);
    }

    [HttpGet("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(int userId)
    {
        if (! await _userRepository.IsUserExistsAsync(userId))
            return NotFound();
        
        var user = await _userRepository.GetUserAsync(userId);
        
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Unexpected error while getting data from database");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
        
        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        if (userDto == null)
            return UnprocessableEntity(ModelState);
        
        // проверка на добавление пользователя с тем же логином в течении последних 5 сек
        if (await _userRepository.IsRecentUserExists(userDto.Login))
        {
            ModelState.AddModelError(nameof(userDto.Login),
                "User with the same login was created less than 5 seconds ago");
            return BadRequest(ModelState);
        }
        
        if (! await _groupRepository.IsUserGroupExistsAsync(userDto.GroupCode))
        {
            ModelState.AddModelError(nameof(userDto.GroupCode), 
                "User group associated with the GroupCode doesn't exists");
            return BadRequest(ModelState);
        }
        
        // проверка на повторное добавление администратора
        if (userDto.GroupCode == UserGroup.CodeStringValue(GroupCode.Admin))
        {
            if (await _userRepository.IsActiveAdminExistsAsync())
            {
                ModelState.AddModelError("", "Admin user already exists");
                return BadRequest(ModelState);
            }
        }
        
        var userGroup = await _groupRepository.GetUserGroupAsync(userDto.GroupCode);
        var activeState = await _stateRepository.GetUserStateAsync(UserState.CodeStringValue(StateCode.Active));
        var user = new User
        {
            Login = userDto.Login,
            Password = userDto.Password,
            CreatedDate = DateTime.UtcNow,
            UserState = activeState,
            UserGroup = userGroup
        };

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (! await _userRepository.CreateUserAsync(user))
        {
            ModelState.AddModelError("", "Unexpected error while saving user to database");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
    
        return Ok();
    }
    
    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        if (! await _userRepository.IsUserExistsAsync(userId))
            return NotFound();
    
        var blockedState = await _stateRepository.GetUserStateAsync(UserState.CodeStringValue(StateCode.Blocked));
        var userToBlock = await _userRepository.GetUserAsync(userId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (userToBlock.UserState.Code == UserState.CodeStringValue(StateCode.Blocked))
        {
            ModelState.AddModelError("", "User already blocked");
            return BadRequest(ModelState);
        }
        userToBlock.UserState = blockedState;
        
        if (! await _userRepository.UpdateUserAsync(userToBlock))
        {
            ModelState.AddModelError("", "Unexpected error while deleting user from database");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
    
        return NoContent();
    }
}