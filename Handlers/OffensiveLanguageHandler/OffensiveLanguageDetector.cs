// OffensiveLanguageDetector.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace _490Bot.Handlers.OffensiveLanguageHandler
{
    public class OffensiveLanguageDetector
    {
        private readonly List<string> offensiveWords;

        public OffensiveLanguageDetector(List<string> customOffensiveWords = null)
        {
            // Initialize with a default list of offensive words
            offensiveWords = new List<string>
            {
                "retard",
                "cunt",
                // Add more
            };

            // If custom offensive words are provided, append them to the list
            if (customOffensiveWords != null)
                offensiveWords.AddRange(customOffensiveWords);
        }

        public bool ContainsOffensiveLanguage(string text)
        {
            // Convert the text to lowercase for case-insensitive matching
            text = text.ToLower();

            // Check if any offensive word is present in the text
            return offensiveWords.Any(offensiveWord => text.Contains(offensiveWord));
        }
    }
}

