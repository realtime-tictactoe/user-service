using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicTacToe.User.Models;

public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreationTime { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastLoginTime { get; set; }
}