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
        private readonly Database _database = new();
      
        public Profile Profile { 
            get { return _profile; }
            set { _profile = value; }
        }
        public async Task<Profile> GetProfile(ulong uid)
        {
            _profile = await _database.GetProfile(uid);
            return _profile;
        }

        public async Task SetProfile (ulong uid)
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

            await _database.InsertProfile(_profile);
        }

        public async void UpdateStatus(String newStatus) {
            _profile.StatusField = newStatus;
            await _database.UpdateProfile(_profile);
        }

        public async void UpdateAbout(String newAbout) {
            _profile.AboutField = newAbout;
            await _database.UpdateProfile(_profile);
        }

        public async void UpdateColor(String hexCode) {
            _profile.Color = hexCode;
            await _database.UpdateProfile(_profile);

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
