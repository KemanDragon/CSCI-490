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
        
    private List<string> badWords = new List<string> {"badword" , "fluck", "glasshole"};
       
    public bool langFilter(string content){
        string lowercasemessage = content.ToLower();
        return badWords.Any(word => lowercasemessage.Contains(word));
    }


}



public class LangLog{
    private readonly Database DBConnection = new();

    private void languageLog(ulong userID, string userName, string content){
        var _langFilter = new LangFilter();

        if (_langFilter.langFilter(content)){
            DateTime currentDateTime = DateTime.Now;
            Logging prof = new Logging(userID, userName, currentDateTime);

            DBConnection.InsertLogAsync(userID.ToString(),userName,currentDateTime);
        }
    }
}




}






