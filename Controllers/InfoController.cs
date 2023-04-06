using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.User.DTOs;
using TicTacToe.User.Services;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("info")]
public class InfoController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public InfoController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<ActionResult<UserInfo>> GetUserInfoAsync()
    {
        try
        {
            var encryptedToken = Request.Cookies["Token"];
            if (encryptedToken is null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Not logged in.",
                    Status = 401
                };
                return Unauthorized(problemDetails);
            }

            var token = _tokenService.DecryptToken(encryptedToken);

            var user = await _userService.GetUserAsync(token.UserId);

            var info = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                CreationTime = user.CreationTime
            };
            return Ok(info);
        }
        catch (InvalidTokenException)
        {
            Response.Cookies.Delete("Token");

            var problemDetails = new ProblemDetails
            {
                Title = "Invalid login.",
                Status = 401
            };
            return Unauthorized(problemDetails);
        }
        catch (ExpiredTokenException)
        {
            Response.Cookies.Delete("Token");

            var problemDetails = new ProblemDetails
            {
                Title = "Login expired.",
                Status = 401
            };
            return Unauthorized(problemDetails);
        }
        catch (UserDoesNotExistException)
        {
            Response.Cookies.Delete("Token");

            var problemDetails = new ProblemDetails
            {
                Title = "User does not exist.",
                Status = 401
            };
            return Unauthorized(problemDetails);
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUserInfoAsync([FromRoute] int id)
    {
        return StatusCode(501, "Not implemented.");
    }
}