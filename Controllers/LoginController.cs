using Microsoft.AspNetCore.Mvc;
using TicTacToe.User.DTOs;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("login")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public IActionResult LoginAsync([FromBody] UserCredentials credentials)
    {
        return StatusCode(501, "Not implemented.");
    }
}