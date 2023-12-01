using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;

using _490Bot;

namespace _490Bot.Handlers.ProfileHandler {
    public class ProfileHandler {
        public IGuild Server { get; set; }
        private Profile _profile;
        public Profile Profile { 
            get { return _profile; }
            set { _profile = value; }
        }

        public void setProfile (ulong uid)
        {
            _profile = new()
            {
                UserID = uid,
                StatusField = "",
                AboutField = "",
                ExperienceCurrent = 0,
                ExperienceNeeded = 100,
                Color = "000000"
            };
        }

        void updateStatus(String newStatus) {
            _profile.StatusField = newStatus;
        }

        void updateAbout(String newAbout) {
            _profile.AboutField = newAbout;
        }

        void updateColor(String hexCode) {
            _profile.Color = hexCode;
        }
    }
}
