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
        //
        
        //DeletionEditHandler
        public class MessageDeleteAuditLogData : object, IAuditLogData
        {
            public MessageDeleteAuditLogData() { }
        }

        //BannedUserHandler
        public class BanAuditLogData : object, IAuditLogData {
            public IUser Target { get; }
        }

    }
}