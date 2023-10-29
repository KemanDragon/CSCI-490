using System;
using System.IO;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket; //Since Discord bots use Sockets in order to be able to connect to different servers, I decided to add it here
using MySQL.Data;


public class logger
{
    private readonly DiscordSocketClient _client;
    private Logger _logger;

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
            // You can add logic to check for offensive language here
            // If offensive language is detected, call _logger.LogOffensiveLanguage
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
