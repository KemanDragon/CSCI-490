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
            
        private List<string> badWords = new List<string> {"blitch" , "fluck", "glasshole"};
        
        public string langFilter(string content){
            string lowercasemessage = content.ToLower();
            string badWordFound = badWords.FirstOrDefault(word => lowercasemessage.Contains(word));
            return badWordFound;
            
            //return badWords.Any(word => lowercasemessage.Contains(word));
        }


    }



    public class LangLog{
    private readonly Database DBConnection = new();

    private string languageLog(ulong userID, string userName, string content){
        var _langFilter = new LangFilter();
        string badWordFound = _langFilter.langFilter(content);

        if (badWordFound != null){
            DateTime currentDateTime = DateTime.Now;
            Logging prof = new Logging(userID, userName, currentDateTime, badWordFound);
            DBConnection.InsertLogAsync(userID.ToString(),userName,currentDateTime, badWordFound);
        }
        return badWordFound;
    }
}





}






