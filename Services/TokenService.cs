using System;
using System.Text;
using System.Text.Json;
using TicTacToe.User.Models;

namespace TicTacToe.User.Services;

public class TokenService
{
    private readonly EncryptionService _encryptionService;
    
    public TokenService(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public string IssueEncryptedToken(UserAccount user)
    {
        var tokenId = user.Id + DateTime.UtcNow;
        var tokenIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenId));
        var token = new LoginToken
        {
            Id = tokenIdBase64,
            UserId = user.Id,
            IssueTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddDays(2)
        };
        var tokenJson = JsonSerializer.Serialize(token);
        var encryptedToken = _encryptionService.EncryptString(tokenJson);
        return encryptedToken;
    }

    public LoginToken DecryptToken(string encryptedToken)
    {
        try
        {
            var tokenJson = _encryptionService.DecryptString(encryptedToken);
            var token = JsonSerializer.Deserialize<LoginToken>(tokenJson);

            if (DateTime.UtcNow >= token.ExpiryTime)
                throw new ExpiredTokenException();

            return token;
        }
        catch (DecryptionFailedException)
        {
            throw new InvalidTokenException();
        }
        catch (JsonException)
        {
            throw new InvalidTokenException();
        }
    }
}

public class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("The token is invalid.")
    {
    }
}

public class ExpiredTokenException : Exception
{
    public ExpiredTokenException() : base("The token has expired.")
    {
    }
}