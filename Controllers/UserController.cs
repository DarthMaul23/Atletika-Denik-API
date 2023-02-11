using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Atletika_Denik_API.Controllers;

public class UserController : ControllerBase
{
    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("user-Login")]
    public IActionResult UserLogin([FromBody] Atletika_Denik_API.Data.Models.UserLogin userLogin)
    {
        return Ok(_userService.LoginUser(userLogin.userName, userLogin.userPassword));
    }
    
    [HttpGet("get-User-By-ID")]
    public IActionResult GetUserByID(int id = 0)
    {
        return Ok(_userService.GetUserByID(id));
    }
    
    [HttpPost("create-User")]
    public IActionResult CreateUser(Atletika_Denik_API.Data.Models.UsersCreate user, int trenerId)
    {
        _userService.CreateUser(user, trenerId);
        return Ok();
    }
    
    [HttpPut("update-User")]
    public IActionResult UpdateUser(Atletika_Denik_API.Data.Models.Users user, int trenerId)
    {
        _userService.UpdateUser(user, trenerId);
        return Ok();
    }
    
    [HttpDelete("delete-User")]
    public IActionResult DelteUser(int userId, int trenerId)
    {
        _userService.DeleteUser(userId, trenerId);
        return Ok();
    }
    
    [HttpGet("get-Users-For-Trainer")]
    public IActionResult GetUsersForTrainer(int trener_id = 1)
    {
        return Ok(_userService.GetUsersForTrainer(trener_id));
    }
    
    [HttpGet("get-User-Info-By-Id")]
    public IActionResult GetUserInfoById(int userId = 1)
    {
        return Ok(_userService.GetUserInfoByID(userId));
    }
    
    [HttpPost("create-User-Info")]
    public IActionResult CreateUserInfo(int userId, [FromBody] Info userInfo)
    {
        _userService.CreateUserInfo(userId, userInfo);
        return Ok();
    }
    
    [HttpPut("update-User-Info")]
    public IActionResult UpdateUserInfo(int userId, [FromBody] Info userInfo)
    {
        _userService.UpdateUserInfo(userId, userInfo);
        return Ok();
    }
    
    [HttpDelete("delete-User-Info")]
    public IActionResult DeleteUserInfo(int userId)
    {
        _userService.DeleteUserInfo(userId);
        return Ok();
    }
    
}