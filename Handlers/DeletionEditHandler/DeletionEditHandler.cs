using _490Bot.Utilities;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _490Bot.Handlers.DeletionEditHandler
{
    internal class DeletionEditHandler
    {
        private readonly Database _dbConnector = new Database();

        public Task OnMessageDeleted(Cacheable<Discord.IMessage, ulong> cachedMessage, ISocketMessageChannel channel)
        {
            // Log message deletion
            var messageId = cachedMessage.Id;
            LogDeletionOrEdit("Message Deleted", messageId);

            return Task.CompletedTask;
        }

        public Task OnMessageUpdated(Discord.IMessage before, Discord.IMessage after)
        {
            // Log message update
            var messageId = after.Id;
            LogDeletionOrEdit("Message Updated", messageId);

            return Task.CompletedTask;
        }

        private void LogDeletionOrEdit(string action, ulong messageId)
        {
            // Create a log for message deletion or update
            Logs log = new Logs(0, messageId, "DeletionEdit", $"{action} - Message ID: {messageId}", "");

            // Insert the log into the database
            _dbConnector.Insert(log);
        }
    }
}
