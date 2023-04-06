using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.User.Options;
using TicTacToe.User.Services;

namespace TicTacToe.User;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
        builder.Services.Configure<AesOptions>(builder.Configuration.GetSection("Aes"));
        builder.Services.Configure<HashOptions>(builder.Configuration.GetSection("Hash"));
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<TokenService>();
        builder.Services.AddSingleton<EncryptionService>();
        builder.Services.AddSingleton<HashService>();
        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}