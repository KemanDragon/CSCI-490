using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _490Bot.Utilities;
using Discord;

namespace _490Bot.Handlers.ProfileHandler {
    public class Profile {
        public ulong UserID { get; set; }
        public String StatusField { get; set; }
        public String AboutField { get; set; }
        public String Color {  get; set; }
        public int Level { get; set; }
        public int ExperienceCurrent { get; set; }
        public int ExperienceNeeded { get; set; }

        private readonly static Database _database = new();

        public async void LevelUp() {
            Level++;
            ExperienceNeeded *= 2;
            await _database.UpdateProfile(this);
        }

        public async void IncrementExp()
        {
            ExperienceCurrent++;
            await _database.UpdateProfile(this);
        }
    }
}