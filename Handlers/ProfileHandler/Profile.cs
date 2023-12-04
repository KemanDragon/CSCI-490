using Discord;

namespace _490Bot.Handlers {
    public class Profile {
        public ulong UserID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public TimestampTag DateJoined { get; set; }
        public String StatusField { get; set; }
        public String AboutField { get; set; }
        public String Color {  get; set; }
        public int Level { get; set; }
        public int ExperienceCurrent { get; set; }
        public int ExperienceNeeded { get; set; }

        public void LevelUp() {
            Level++;
        }
    }
}