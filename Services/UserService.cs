using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TicTacToe.User.Models;
using TicTacToe.User.Options;

namespace TicTacToe.User.Services;

/// <summary>
/// Provides user management functionality.
/// </summary>
public class UserService
{
    private readonly IMongoCollection<UserAccount> _collection;
    private readonly HashService _hashService;

    public UserService(HashService hashService, IOptions<DatabaseOptions> dbOptions)
    {
        _hashService = hashService;

        var client = new MongoClient(dbOptions.Value.ConnectionString);
        var database = client.GetDatabase(dbOptions.Value.Name);
        _collection = database.GetCollection<UserAccount>(dbOptions.Value.UsersCollectionName);
    }
    
    public async Task<UserAccount> LoginUser(string username, string password)
    {
        var usernameEqualityFilter = Builders<UserAccount>.Filter
            .Eq(user => user.Username, username);
        var user = await _collection.Find(usernameEqualityFilter).FirstOrDefaultAsync();
        if (user is null)
            throw new InvalidCredentialsException();

        try
        {
            _hashService.VerifyString(password, user.PasswordHash);
        }
        catch (HashVerificationFailedException)
        {
            throw new InvalidCredentialsException();
        }

        // Remove sensitive information before returning the user object
        user.PasswordHash = null;
        return user;
    }

    /// <summary>
    /// Create and save a new user account with the provided credentials.
    /// </summary>
    /// <param name="username">The username to create the user with.</param>
    /// <param name="password">The password to create the user with.</param>
    /// <returns>The created user account.</returns>
    /// <exception cref="UserAlreadyExistsException">The username is already in use.</exception>
    public async Task<UserAccount> CreateUserAsync(string username, string password)
    {
        var usernameEqualityFilter = Builders<UserAccount>.Filter
            .Eq(user => user.Username, username);
        var existingUser = await _collection.Find(usernameEqualityFilter).FirstOrDefaultAsync();
        if (existingUser is not null)
            throw new UserAlreadyExistsException();

        var passwordHash = _hashService.HashString(password);

        var user = new UserAccount
        {
            Username = username,
            PasswordHash = passwordHash,
            CreationTime = DateTime.UtcNow,
            LastLoginTime = DateTime.UtcNow
        };
        await _collection.InsertOneAsync(user);
        
        // Remove sensitive information before returning the user object
        user.PasswordHash = null;
        return user;
    }
}

public class UserServiceException : Exception
{
    protected UserServiceException(string message) : base(message)
    {
    }
}

public class InvalidCredentialsException : UserServiceException
{
    public InvalidCredentialsException() : base("Invalid credentials.")
    {
    }
}

public class UserAlreadyExistsException : UserServiceException
{
    public UserAlreadyExistsException() : base("A user with the provided username already exists.")
    {
    }
}