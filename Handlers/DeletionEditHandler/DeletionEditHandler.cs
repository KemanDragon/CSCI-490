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
        private readonly Logger _logger;

        public DeletionEditHandler(Logger logger)
        {
            _logger = logger;
        }

        public Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel)
        {
            if (cachedMessage.HasValue && channel.HasValue)
            {
                // Log message deletion
                return _logger.MessageDeletedAsync(cachedMessage, channel);
            }

            return Task.CompletedTask;
        }

        public Task OnMessageUpdated(Cacheable<IMessage, ulong> before, Cacheable<IMessage, ulong> after, Cacheable<IMessageChannel, ulong> channel)
        {
            if (before.HasValue && after.HasValue && channel.HasValue)
            {
                // Log message update
                return _logger.MessageUpdatedAsync(before, after.Value as SocketMessage, channel.Value as ISocketMessageChannel);
            }

            return Task.CompletedTask;
        }
    }
}
