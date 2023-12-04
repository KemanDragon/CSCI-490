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
        private static Profile _profile;
        private static readonly Database _database = new();
      
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

        public static async Task SetProfile (SocketGuildUser arg)
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

        public static async void UpdateStatus(String newStatus) 
        {
            _profile.StatusField = newStatus;
            await _database.UpdateProfile(_profile);
        }

        public static async void UpdateAbout(String newAbout) 
        {
            _profile.AboutField = newAbout;
            await _database.UpdateProfile(_profile);
        }

        public static async void UpdateColor(String hexCode) 
        {
            _profile.Color = hexCode;
            await _database.UpdateProfile(_profile);
        }

        public static async Task<EmbedBuilder> FormatProfile(SocketGuildUser arg)
        {
            _profile = await _database.GetProfile(arg.Id);
            TimestampTag date = TimestampTag.FromDateTimeOffset(arg.JoinedAt.GetValueOrDefault(), TimestampTagStyles.LongDate);
            return new EmbedBuilder()
                .WithTitle($"User Profile: {arg.DisplayName}")
                .WithDescription("**Ranking**\n"
                + $"**__Total Experience:__** {_profile.ExperienceCurrent}\n"
                + $"**__Level:__** {_profile.Level}\n"
                + $"**__Permission Level:__** {await _database.GetPermissionLevel(_profile.UserID)}\n\n"
                + "**Age**\n"
                + $"**__Joined Server On:__** {date}")
                .WithColor(Color.DarkPurple)
                .WithCurrentTimestamp();
            /*return $"**User Profile: {_profile.Name}**\n\n"
                + "**Ranking**\n"
                + $"**__Total Experience:__** {_profile.ExperienceCurrent}\n"
                + $"**__Level:__** {_profile.Level}\n"
                + $"**__Permission Level:__** {await _database.GetPermissionLevel(_profile.UserID)}\n\n"
                + "**Age**\n"
                + $"**__Joined Server On:__** {date}";*/
        }

        public async Task<bool> CheckForProfile(SocketUser arg)
        {
            bool result = await _database.CheckIfProfileExists(arg.Id);
            return result;
        }
    }
}
