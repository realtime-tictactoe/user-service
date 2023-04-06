using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TicTacToe.User.Options;

namespace TicTacToe.User.Services;

/// <summary>
/// Provides SHA256 hashing functionality.
/// </summary>
public class HashService
{
    private readonly IOptions<HashOptions> _options;

    public HashService(IOptions<HashOptions> options)
    {
        _options = options;
        if (_options.Value.Salt is null || _options.Value.Salt == string.Empty)
            throw new ArgumentException("Salt is not configured or is an empty string.");
    }

    /// <summary>
    /// Hashes a string using the SHA256 algorithm.
    /// </summary>
    /// <param name="payload">The payload to hash.</param>
    /// <returns>A hexadecimal representation of the resulting hash.</returns>
    public string HashString(string payload)
    {
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var saltBytes = Encoding.UTF8.GetBytes(_options.Value.Salt);

        // Concatenate both byte arrays
        var saltedPayloadBytes = new byte[payloadBytes.Length + saltBytes.Length];
        Array.Copy(payloadBytes, 0, saltedPayloadBytes, 0, payloadBytes.Length);
        Array.Copy(saltBytes, 0, saltedPayloadBytes, payloadBytes.Length, saltBytes.Length);
        
        var hashedBytes = SHA256.HashData(saltedPayloadBytes);

        return Convert.ToHexString(hashedBytes);
    }

    /// <summary>
    /// Takes a payload, and a valid hash and tests whether the payload's hash matches the valid hash.
    /// </summary>
    /// <param name="payload">The payload to verify.</param>
    /// <param name="correctHash">The hash to verify against.</param>
    /// <exception cref="HashVerificationFailedException">Hash verification failed.</exception>
    public void VerifyString(string payload, string correctHash)
    {
        var hashToVerify = HashString(payload);
        if (hashToVerify != correctHash)
            throw new HashVerificationFailedException();
    }
}

/// <summary>
/// Thrown when the verification of a string fails.
/// </summary>
public class HashVerificationFailedException : Exception
{
    public HashVerificationFailedException() : base("Hash verification failed.")
    {
    }
}