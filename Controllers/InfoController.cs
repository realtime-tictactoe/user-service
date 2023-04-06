using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.User.Controllers;

[ApiController]
[Route("info")]
public class InfoController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUserInfoAsync()
    {
        return StatusCode(501, "Not implemented.");
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUserInfoAsync([FromRoute] int id)
    {
        return StatusCode(501, "Not implemented.");
    }
}