using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace _490Bot.Handlers.ProfileHandler {
    public class Profile {
        public int UserID { get; set; }
        public String StatusField { get; set; }
        public String AboutField { get; set; }
        public Badge[] Badges { get; set; }
        public int Level { get; set; }
        public int ExperienceCurrent { get; set; }
        public int ExperienceNeeded { get; set; }

        int levelUp() {
            int newLevel = 0;
            return newLevel;
        }
    }
}
