using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.User.DTOs;
using TicTacToe.User.Services;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("create")]
public class CreateController : ControllerBase
{
    private readonly UserService _userService;

    public CreateController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserInfo>> CreateAccountAsync([FromBody] UserCredentials credentials)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.CreateUserAsync(credentials.Username, credentials.Password);

            var info = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                CreationTime = user.CreationTime
            };
            return Created("/info/" + info.Id, info);
        }
        catch (UserAlreadyExistsException)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Username already in use.",
                Status = 409
            };
            return Conflict(problemDetails);
        }
    }
}