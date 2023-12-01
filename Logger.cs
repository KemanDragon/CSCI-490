using System;
using System.IO;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket; //Since Discord bots use Sockets in order to be able to connect to different servers, I decided to add it here
using MySql.Data.MySqlClient;


namespace _490Bot.Handlers.LogHandler
{
    public class Logs
    {
        public ulong UserID { get; set; }
        public ulong LogID { get; set; }
        public string LogLevel { get; set; }
        public string LogMessage { get; set; }
        public string Reason { get; set; }

        public Logs(ulong userId, ulong logId, string logLevel, string logMessage, string reason)
        {
            UserID = userId;
            LogID = logId;
            LogLevel = logLevel;
            LogMessage = logMessage;
            Reason = reason;
        }
    }


    public sealed class DatabaseConnector
    {
        IGuild server;
        MySqlConnection _connection;
        // String connectionstring = "server=localhost;uid=root;pwd=root;database=LoggerClass";
        public DatabaseConnector()
        {
            _connection = new MySqlConnection("server=localhost;uid=root;pwd=root;database=LoggerClass");
        }
        public async void OpenConnection()
        {
            try
            {
                await _connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void CloseConnection()
        {
            try
            {
                await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int Insert(Logs logs)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                String queryText = $"INSERT INTO logs VALUES(@UserID, @LogID, @LogLevel, @LogMessage, @Reason, 0)";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@UserID", logs.UserID);
                query.Parameters.AddWithValue("@LogID", logs.LogID);
                query.Parameters.AddWithValue("@LogLevel", logs.LogLevel);
                query.Parameters.AddWithValue("@LogMessage", logs.LogMessage);
                query.Parameters.AddWithValue("@Reason", logs.Reason);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            CloseConnection();
            return result;
        }
    }

    public class Logger
    {
        private readonly DiscordSocketClient _client;
        private Logger _logger;
        
        public Logger(DiscordSocketClient client, Logger logger) // Pass the DiscordSocketClient as a parameter
        {
            _client = client;
            _logger = logger;
            _client.Log += LogAsync;


            //Add event asyncs
            _client.UserBanned += UserBannedAsync;
            //_client.MessageReceived += MessageReceivedAsync;
            _client.MessageUpdated += MessageUpdatedAsync;
            _client.MessageDeleted += MessageDeletedAsync;
        }

        public ulong UserID { get; set; }

        private Task LogAsync(LogMessage log)
        {

            Console.WriteLine(log);

            //Logic to be added
            return Task.CompletedTask;
        }

        //OffensiveLanguageHandler
        private void LogOffensiveLanguage(ulong authorId, string content) // Implement LogOffensiveLanguage
        {
            // logic to be added
        }

        //DeletionEditHandler
        private Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // Log message edits here using _logger.LogDeletionEdit
            return Task.CompletedTask;
        }

        private Task MessageDeletedAsync(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            // Log message deletions here using _logger.LogDeletionEdit
            return Task.CompletedTask;
        }
    



        private Task UserBannedAsync(SocketUser user, SocketGuild guild)
        {
            LogBannedUser(user.Id, guild.Id, "Reason not available");
            return Task.CompletedTask;
        }

        //BannedUserHandler
        private void LogBannedUser(ulong UserId, ulong guildId, string reason) // _client will use this handler since
                                                                               // this is essentially the BannedUserHandler
        {
            //_logger.LogBannedUser(user.Id, guild.Id, reason); //Log user that got banned, the guild they were bannded from,
            // and the reason for which the user was banned.
            // return Task.CompletedTask; //Task has been completed.
        }
    
    }

}

