using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Interactions;
using MySql.Data.MySqlClient;

namespace _490Bot.Utilities{
    public class Logging{
        public ulong UserID{get; set;}
        public string UserName{get; set;}
        public DateTime TimeStamp{get; set;}

        public Logging(ulong userID, string userName){
            UserID = userID;
            UserName = userName;
            TimeStamp = DateTime.Now;

        }


    }


    



}








