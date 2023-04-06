using System.ComponentModel.DataAnnotations;

namespace TicTacToe.User.DTOs;

public class UserCredentials
{
    [Required]
    [MinLength(6)]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric.")]
    public string Username;
    [Required]
    [MinLength(6)]
    public string Password;
}