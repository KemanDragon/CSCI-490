using System;
using System.Data.Common;
using System.IO;
using System.Text;
using _490Bot.Handlers.OffensiveLanguageHandler;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket; //Since Discord bots use Sockets in order to be able to connect to different servers, I decided to add it here
using MySql.Data.MySqlClient;


namespace _490Bot.Utilities
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

    /*
    public class DatabaseConnector
    {
        IGuild server;
        MySqlConnection _connection;
        
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

        
    }
    */
    
    public class Logger
    {
        //private readonly DiscordSocketClient _client;
        //private Logger _logger;
        private readonly Database _dbConnector;
        private ulong _logChannelId;
        public Logger()
        {
            _dbConnector = new Database();
            
        }

        public void SetLogChannelId(ulong logChannelId)
        {
            _logChannelId = logChannelId;
        }

        private async Task LogToChannelAsync(ITextChannel logChannel, string message)
        {
            try
            {
                if (logChannel != null)
                {
                    await logChannel.SendMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogToChannelAsync: {ex}");
            }
        }

        private async Task LogMessageToChannelAsync(ITextChannel logChannel, string logMessage)
        {
            await LogToChannelAsync(logChannel, logMessage);
        }

        public async Task LogOffensiveLanguageAsync(ulong authorId, string content)
        {
            try
            {
                // Create an instance of OffensiveLanguageDetector
                var offensiveLanguageDetector = new OffensiveLanguageDetector();

                // Check if the content contains offensive language
                if (offensiveLanguageDetector.ContainsOffensiveLanguage(content))
                {
                    // Create a log for offensive language
                    var log = new Logs(authorId, 0, "OffensiveLanguage", $"Offensive language detected from user {authorId}", content);

                    // Insert the log into the database
                    await _dbConnector.Insert(log);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogOffensiveLanguageAsync: {ex}");
            }
        }

        public async Task LogBannedUserAsync(SocketUser user, SocketGuild guild)
        {
            try
            {
                var userId = user.Id;

                // Fetch the ban entry to get the reason (if available)
                var banEntry = await guild.GetBanAsync(userId);
                var reason = banEntry?.Reason ?? "No reason provided";

                // Log user ban
                var log = new Logs(userId, 0, "UserBanned", $"User {userId} banned. Reason: {reason}", reason);

                // Insert the log into the database
                await _dbConnector.Insert(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BannedUserAsync: {ex}");
            }
        }

        public async Task MessageDeletedAsync(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            try
            {
                if (message.HasValue && message.Value is SocketUserMessage userMessage)
                {
                    // Send deletion log to the specified channel
                    var logChannel = userMessage.Channel as ITextChannel;
                    await LogMessageToChannelAsync(logChannel, $"Message deleted: {userMessage.Content}");

                    // Log deletion or edit as needed
                    await LogDeletionOrEditAsync("Message Deleted", userMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MessageDeletedAsync: {ex}");
            }
        }

        public async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            try
            {
                if (after is SocketUserMessage userMessage)
                {
                    // Send edit log to the specified channel
                    var logChannel = channel as ITextChannel;
                    var beforeContent = (before.HasValue && before.Value is SocketUserMessage beforeMessage) ? beforeMessage.Content : "N/A";
                    var afterContent = userMessage.Content;

                    await LogMessageToChannelAsync(logChannel, $"Message edited: Before: {beforeContent}, After: {afterContent}");

                    // Log deletion or edit as needed
                    await LogDeletionOrEditAsync("Message Updated", userMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MessageUpdatedAsync: {ex}");
            }
        }



        private async Task LogDeletionOrEditAsync(string action, SocketUserMessage message)
        {
            // Extract necessary information from the message
            var messageId = message.Id;
            var content = message.Content;

            // Create a log for message deletion or update
            var log = new Logs(0, messageId, "DeletionEdit", $"{action} - Message ID: {messageId}", content);

            // Insert the log into the database
            await _dbConnector.Insert(log);
        }

        /*
        public Logger(DiscordSocketClient client, Logger logger) // Pass the DiscordSocketClient as a parameter
        {
            _client = client;
            _logger = logger;
            _client.Log += LogAsync;
            _dbConnector = new Database();

            //Add event asyncs
            //_client.UserBanned += UserBannedAsync;
            //_client.MessageReceived += MessageReceivedAsync;
            //_client.MessageUpdated += MessageUpdatedAsync;
            //_client.MessageDeleted += MessageDeletedAsync;

        }
        */
        //public ulong UserID { get; set; }

        /*
        private Task LogAsync(LogMessage log)
        {

            Console.WriteLine(log);

            return Task.CompletedTask;
        }
        */
        /*
        //OffensiveLanguageHandler
        private void LogOffensiveLanguage(ulong authorId, string content)
        {
            // Create an instance of OffensiveLanguageDetector
            var offensiveLanguageDetector = new OffensiveLanguageDetector();

            // Check if the content contains offensive language
            if (offensiveLanguageDetector.ContainsOffensiveLanguage(content))
            {
                // Create a log for offensive language
                Logs log = new Logs(authorId, 0, "OffensiveLanguage", $"Offensive language detected from user {authorId}", content);

                // Insert the log into the database
                //_dbConnector.Insert(log);


            }
        }
        */
        /*
        //DeletionEditHandler
        private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // Check if the message content was changed
            if (before.HasValue && after.Content != before.Value.Content)
            {
                // Create a new instance of Logs with the relevant information
                Logs log = new Logs(after.Author.Id, after.Id, "MessageUpdated", $"Message updated in channel {channel.Id}", "Content changed");

                // Insert the log into the database
                //_dbConnector.Insert(log);

            }

            //return Task.CompletedTask;
        }
        
        
        private async Task MessageDeletedAsync(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            // Create a new instance of Logs with the relevant information
            Logs log = new Logs(message.Value.Author.Id, message.Id, "MessageDeleted", $"Message deleted in channel {channel.Id}", "No specific reason");

            // Insert the log into the database
            //_dbConnector.Insert(log);

            //return Task.CompletedTask;
        }
        
        */



        /*
        //BannedUserHandler
        private async Task UserBannedAsync(SocketUser user, SocketGuild guild)
        {
            string reason = "Reason not available";
            await LogBannedUser(user.Id, guild.Id, reason);
        }

        private async Task LogBannedUser(ulong userId, ulong guildId, string reason)
        {
            Logs logs = new Logs(userId, 0, "BAN", $"User {userId} banned from guild {guildId}. Reason: {reason}", reason);
            //_dbConnector.Insert(logs);

        }

        */
    }

}

