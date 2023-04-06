using Microsoft.AspNetCore.Mvc;
using TicTacToe.User.DTOs;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("create")]
public class CreateController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateAccountAsync([FromBody] UserCredentials credentials)
    {
        return StatusCode(501, "Not implemented.");
    }
}