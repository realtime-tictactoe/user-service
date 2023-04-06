using System;

namespace TicTacToe.User.DTOs;

public class UserInfo
{
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreationTime { get; set; }
}