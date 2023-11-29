using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dataBasee {
   
   
    public class Program
    {
        private DiscordSocketClient _client;
        private Database _database;

        public static Task Main(string[] args) => new Program().MainAsync();

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

              
        private async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;

        // connection string for database
        string connectionString = "Server=127.0.0.1;Database=muteLogs;User=root;Password=jjhindsj";
        _database = new Database(connectionString);

        // calls database connection from database class
        string userID = "Example";
        string userName = "Name_Username";
        string message = " a whole bunch of bad words that shoudlnt be said";
        DateTime timeStamp = DateTime.Now;


        if (_database.InsertLog(userID,userName,message,timeStamp))
        {
            Console.WriteLine("Database connection successful.");
        }
        else
        {
            Console.WriteLine("Database connection failed.");
            return;
        }
        
        
        
        //private async Task MainAsync()
        
        _client = new DiscordSocketClient();
        _client.Log += Log;
            //_client.MessageReceived += MessageReceivedAsync;

        var token = "MTE2ODQxOTkyMzc0NDI2MDIxOA.GSgNiK.R8-UmMyBc48oH1iy5HUlS3PXkviKnUSf9REJHA";

        _database = new Database("Server=127.0.0.1;Database=muteLogs;User=root;Password=jjhindsj");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
        
        
        
        
     }
    }
}