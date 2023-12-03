using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using _490Bot.Utilities;

namespace _490Bot.Handlers
{
    public class ProfileHandler 
    {
        public IGuild Server { get; set; }
        private Profile _profile;
        private readonly Database _database = new();
      
        public Profile Profile 
        { 
            get { return _profile; }
            set { _profile = value; }
        }
        public async Task<Profile> GetProfile(ulong uid) 
        {
            _profile = await _database.GetProfile(uid);
            return _profile;
        }

        public async Task SetProfile (SocketGuildUser arg)
        {
            _profile = new()
            {
                UserID = arg.Id,
                Username = arg.Username,
                Name = arg.DisplayName,
                DateJoined = TimestampTag.FromDateTimeOffset(arg.JoinedAt.GetValueOrDefault(), TimestampTagStyles.LongDate),
                StatusField = "",
                AboutField = "",
                ExperienceCurrent = 0,
                ExperienceNeeded = 100,
                Color = "000000"
            };

            await _database.InsertProfile(_profile);
        }

        public async void UpdateStatus(String newStatus) 
        {
            _profile.StatusField = newStatus;
            await _database.UpdateProfile(_profile);
        }

        public async void UpdateAbout(String newAbout) 
        {
            _profile.AboutField = newAbout;
            await _database.UpdateProfile(_profile);
        }

        public async void UpdateColor(String hexCode) 
        {
            _profile.Color = hexCode;
            await _database.UpdateProfile(_profile);
        }

        public async Task<string> FormatProfile(ulong userID)
        {
            _profile = await _database.GetProfile(userID);
            return $"**User Profile: {_profile.Name} - {_profile.Username}**\n\n"
                + "**Ranking**\n"
                + $"**__Total Experience:__** {_profile.ExperienceCurrent}\n"
                + $"**__Level:__** {_profile.Level}\n"
                + $"**__Permission Level:__** {_database.GetPermissionLevel(_profile.UserID)}\n\n"
                + "**Age**\n"
                + $"**__Joined Server On:__** {_profile.DateJoined}";
        }
    }
}
