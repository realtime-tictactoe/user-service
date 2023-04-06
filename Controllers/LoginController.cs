using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.User.DTOs;
using TicTacToe.User.Services;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("login")]
public class LoginController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public LoginController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<ActionResult<UserInfo>> LoginAsync([FromBody] UserCredentials credentials)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.LoginUser(credentials.Username, credentials.Password);
            
            var token = _tokenService.IssueEncryptedToken(user);
            Response.Cookies.Append("Token", token);
            
            var info = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                CreationTime = user.CreationTime
            };
            return Ok(info);
        }
        catch (InvalidCredentialsException)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid credentials.",
                Status = 401
            };
            return Unauthorized(problemDetails);
        }
    }
}