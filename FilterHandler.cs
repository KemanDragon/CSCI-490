using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Interactions;
using _490Bot.Handlers.ProfileHandler;
using _490Bot.Handlers.LogHandler;



public class messScan{

    public async Task messageReceivedAsync(SocketMessage message){

        if (message is not IUserMessage userMessage || message.Author.IsBot)
        {
            return;
        }

        string unwantedLanguage = "shrimp"

        if (userMessage.Content.Contains(profanityWord, StringComparison.OrdinalIgnoreCase)){


            
        }

    }


}








