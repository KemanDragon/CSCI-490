using System;
using System.IO;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket; //Since Discord bots use Sockets in order to be able to connect to different servers, I decided to add it here
using MySql.data.MySqlClient;

namespace _490Bot.Handlers.LogHandler {
    public class logger
    {
        private readonly DiscordSocketClient _client;
        //private Logger _logger;

        public Logger()
        {
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;


            // Add event asyncs
            _client.UserBanned += UserBannedAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.MessageUpdated += MessageUpdatedAsync;
            _client.MessageDeleted += MessageDeletedAsync;
        }

        //OffensiveLanguageHandler

        private Task MessageReceivedAsync(SocketMessage message)
        {
            if (message is SocketUserMessage userMessage)
            {
                // Check for offensive language in the message content
                if (ContainsOffensiveLanguage(userMessage.Content))
                {
                    // Log the event using the Logger
                    _logger.LogOffensiveLanguage(userMessage.Author.Id, userMessage.Content);

                    // Optionally, you can delete the offending message
                    await userMessage.DeleteAsync();

                    // You can also send a warning or take other actions as needed
                    await message.Channel.SendMessageAsync($"@{message.Author.Username}, please refrain from using offensive language.");
                }
            }
            return Task.CompletedTask;
        }

        //DeletionEditHandler
        private Task MessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // Log message edits here using _logger.LogDeletionEdit
            return Task.CompletedTask;
        }

        private Task MessageDeletedAsync(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            // Log message deletions here using _logger.LogDeletionEdit
            return Task.CompletedTask;
        }

        //BannedUserHandler
        private Task UserBannedAsync(SocketUser user, SocketGuild guild, string reason) // _client will use this handler since
                                                                                        // this is essentially the BannedUserHandler
        {
            _logger.LogBannedUser(user.Id, guild.Id, reason); //Log user that got banned, the guild they were bannded from,
                                                              // and the reason for which the user was banned.
            return Task.CompletedTask; //Task has been completed.
        }



    }
}
