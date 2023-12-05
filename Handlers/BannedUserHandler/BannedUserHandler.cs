using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _490Bot.Utilities;
using Discord.WebSocket;

namespace _490Bot.Handlers.BannedUserHandler
{
    internal class BannedUserHandler
    {
        private readonly Database _dbConnector = new Database();

        public Task OnUserBanned(SocketUser user, SocketGuild guild, string reason)
        {
            // Log user ban
            var userId = user.Id;
            LogBannedUser(userId, reason);

            return Task.CompletedTask;
        }

        private void LogBannedUser(ulong userId, string reason)
        {
            // Create a log for banned user
            Logs log = new Logs(userId, 0, "UserBanned", $"User {userId} banned. Reason: {reason}", reason);

            // Insert the log into the database
            _dbConnector.Insert(log);
        }
    }
}

