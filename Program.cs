using System.Data;
using Discord;
using Discord.WebSocket;
using MySql.Data.MySqlClient;
using _490Bot.Handlers.ProfileHandler;

public class Program {
    public static Task Main(string[] args) => new Program().MainAsync();
    private DiscordSocketClient _client;

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task MainAsync() {
        _client = new DiscordSocketClient();

        _client.Log += Log;

        var token = "MTE2ODQxOTkyMzc0NDI2MDIxOA.GSgNiK.R8-UmMyBc48oH1iy5HUlS3PXkviKnUSf9REJHA";

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        

        await _connection.OpenAsync();
        if (_connection != null && _connection.State == System.Data.ConnectionState.Open) {
            Console.WriteLine("Connection to database successful.");
            Badge test = new();
            MySqlCommand testQuery = new MySqlCommand();
            String testQueryText = $"INSERT INTO badge VALUES(@BadgeName, @BadgeDesc, @BadgeIcon, 0)";
            testQuery.CommandText = testQueryText;
            testQuery.Connection = _connection;
            testQuery.Parameters.AddWithValue("@BadgeName", test.BadgeName);
            testQuery.Parameters.AddWithValue("@BadgeDesc", test.BadgeDesc);
            testQuery.Parameters.AddWithValue("@BadgeIcon", test.BadgeIcon);
            int result = testQuery.ExecuteNonQuery();
            if (result < 0) { Console.WriteLine("Error inserting"); }
            else Console.WriteLine("Successfully inserted.");
        } else {
            Console.WriteLine("Connection to database failed.");
        }

        await _connection.CloseAsync();
        await Task.Delay(-1);
    }
}