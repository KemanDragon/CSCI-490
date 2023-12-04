using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Interactions;
using MySql.Data.MySqlClient;

namespace _490Bot.Utilities{

    public class MuteFunc{
    
    
        
        private readonly DiscordSocketClient _client;
        private readonly ulong _mutedRoleId = 1181093135623929898;

        public MuteFunc(DiscordSocketClient client)
            {
            _client = client;
            }

        
        public async Task MuteUserAsync(SocketGuildUser user, TimeSpan duration)
            {
        
            var mutedRole = user.Guild.GetRole(_mutedRoleId);

            
            if (mutedRole != null)
            {
                await user.AddRoleAsync(mutedRole);
            }

            
            _ = Task.Run(async () =>
            {
                await Task.Delay(duration);
                await UnmuteUserAsync(user);
            });
        }

        public async Task UnmuteUserAsync(SocketGuildUser user)
        {
            
            var mutedRole = user.Guild.GetRole(_mutedRoleId);

            
            if (mutedRole != null)
            {
                await user.RemoveRoleAsync(mutedRole);
            }
        }
        
        
        
        
        
        
        
       
    }




}

