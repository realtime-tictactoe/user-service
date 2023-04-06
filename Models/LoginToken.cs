using System;

namespace TicTacToe.User.Models;

public class LoginToken
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public DateTime IssueTime { get; set; }
    public DateTime ExpiryTime { get; set; }
}