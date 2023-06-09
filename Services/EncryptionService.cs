using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TicTacToe.User.Options;

namespace TicTacToe.User.Services;

public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IOptions<AesOptions> options)
    {
        if (string.IsNullOrEmpty(options.Value.Key))
            throw new ArgumentException("AES key is not configured or is an empty string.");
        _key = Convert.FromHexString(options.Value.Key);
    }

    public static string GenerateKey()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            return Convert.ToHexString(aes.Key);
        }
    }

    public string EncryptString(string payload)
    {
        try
        {
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var ivBytes = GenerateRandomVector();

            using (var memoryStream = new MemoryStream())
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = ivBytes;

                using (var cryptoStream = new CryptoStream(
                           memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(payloadBytes);
                }

                var iv = Convert.ToHexString(ivBytes);
                var cipherText = Convert.ToHexString(memoryStream.ToArray());

                return iv + "!" + cipherText;
            }
        }
        catch
        {
            throw new EncryptionFailedException();
        }
    }

    public string DecryptString(string cipherText)
    {
        try
        {
            var splitStrings = cipherText.Split("!");
            var ivBytes = Convert.FromHexString(splitStrings[0]);
            var cipherBytes = Convert.FromHexString(splitStrings[1]);

            string decryptedPayload;

            using (var memoryStream = new MemoryStream(cipherBytes))
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = ivBytes;

                using (var cryptoStream = new CryptoStream(
                           memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var reader = new StreamReader(cryptoStream))
                {
                    decryptedPayload = reader.ReadToEnd();
                }
            }

            return decryptedPayload;
        }
        catch
        {
            throw new DecryptionFailedException();
        }
    }

    private static byte[] GenerateRandomVector()
    {
        return RandomNumberGenerator.GetBytes(16);
    }
}

public class EncryptionFailedException : Exception
{
    public EncryptionFailedException() : base("The encryption failed.")
    {
    }
}

public class DecryptionFailedException : Exception
{
    public DecryptionFailedException() : base("The decryption failed.")
    {
    }
}