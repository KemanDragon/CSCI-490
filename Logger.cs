using System;
using System.IO;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket; //Since Discord bots use Sockets in order to be able to connect to different servers, I decided to add it here

//Changes that need to be made:
// - Figure out how to insert Guild and Member attributes
// - Figure out how to use a log file so that everything the logger logs gets saved to the log file

public class logger
{
    private readonly DiscordSocketClient _client;
    private Logger _logger;

    public Logger()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        command.Log += LogAsync;
     }

        //OffensiveLanguageHandler
       

         public class AutoModFlaggedMessageAuditLogData : object, IAuditLogData

         public string AutoModRuleName { get; set; } //Get the name of the auto moderation rule that got triggered
        
        //DeletionEditHandler
        public class DeletionEditHandler {
            public class MessageDeleteAuditLogData : object, IAuditLogData
            {
                public ulong ChannelId { get; } //Gets the ID of the channel that the messages were deleted from.

                public IUser Target { get; } //Gets the user of the messages that were deleted.
            }

            public class MemberUpdateAuditLogData : object, IAuditLogData
            {
                public MemberInfo After { get; } //Gets the member information after the changes.

                public MemberInfo Before { get; } //Gets the member information before the changes.

                public IUser Target { get; } // Gets the user that the changes were performed on.
            }
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