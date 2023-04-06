namespace TicTacToe.User.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public string Name { get; set; }
    public string UsersCollectionName { get; set; }
}