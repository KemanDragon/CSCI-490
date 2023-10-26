using System;
using System.IO;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Rest;

namespace logger
{

    public class Logger
    {
        /*A dd the attributes and methods to this:
     * member: Member
     * server: Guild
     * 
    */
        public string messageContent { get; set; }
        //OffensiveLanguageHandler
        public class OffensiveLanguageHandler {

            public class AutoModFlaggedMessageAuditLogData : object, IAuditLogData

                public string AutoModRuleName { get; set; } //Get the name of the auto moderation rule that got triggered
        }
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
        public class BannedUserHandler {

            public class BanAuditLogData : object, IAuditLogData {

                public IUser Target { get; }
            }
        }

    }
}