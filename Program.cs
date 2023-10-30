using Discord;
using Discord.WebSocket;

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

        await Task.Delay(-1);
    }
}