using _490Bot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _490Bot.Utilities;

namespace _490Bot.Handlers.OffensiveLanguageHandler
{
   
    internal class OffensiveLanguageHandler
    {
        private readonly Database _dbConnector;
        _dbConnector = new Database();
        //OffensiveLanguageHandler
        private void LogOffensiveLanguage(ulong authorId, string content)
        {
            // Create an instance of OffensiveLanguageDetector
            var offensiveLanguageDetector = new OffensiveLanguageDetection.OffensiveLanguageDetector();

            // Check if the content contains offensive language
            if (offensiveLanguageDetector.ContainsOffensiveLanguage(content))
            {
                // Create a log for offensive language
                Logs log = new Logs(authorId, 0, "OffensiveLanguage", $"Offensive language detected from user {authorId}", content);

                // Insert the log into the database
                _dbConnector.Insert(log);


            }
        }
    }
}
