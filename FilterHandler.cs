using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Interactions;
using MySql.Data.MySqlClient;

namespace _490Bot.Utilities{
public class LangFilter{
        private readonly DiscordSocketClient _client;
        private List<string> badWords = new List<string> {"blitch" , "fluck", "glasshole"};
       // private static readonly string[] badWords = {"blitch" , "fluck", "glasshole"};

        public LangFilter(DiscordSocketClient client){
            _client = client;
            _client.MessageReceived += HandleMessageReceivedAsync;
        }

        public async Task HandleMessageReceivedAsync(SocketMessage message){
            if (message is not IUserMessage userMessage || message.Author.IsBot)
            {
                return;
            }
            if(BadLang(userMessage.Content)){
                Logging badlanglog = new Logging(userMessage.Author.Id, userMessage.Author.Username);

            }
        }
        
        private bool BadLang(string content){
        
            string lowercasemessage = content.ToLower();
            return badWords.Any(word => lowercasemessage.Contains(word));

        }


        


    }
}





