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
        public static async Task<Profile> GetProfile(ulong uid)
        {
            _profile = await _database.GetProfile(uid);
            return _profile;
        }

        public static async Task SetProfile(SocketGuildUser arg)
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

        public static async void UpdateStatus(Profile profile, string newStatus)
        {
            profile.StatusField = newStatus;
            await _database.UpdateProfile(profile);
        }

        public static async void UpdateAbout(Profile profile, string newAbout)
        {
            profile.AboutField = newAbout;
            await _database.UpdateProfile(profile);
        }

        public static async void UpdateColor(Profile profile, string newColor)
        {
            profile.Color = newColor;
            await _database.UpdateProfile(profile);
        }

        private static async Task<uint> SetColor(string color)
        {
            switch(color)
            {
                case "blue":
                    return Convert.ToUInt32("3498DB", 16);
                case "gold":
                    return Convert.ToUInt32("F1C40F", 16);
                case "green":
                    return Convert.ToUInt32("2ECC71", 16);
                case "magenta":
                    return Convert.ToUInt32("E91E63", 16);
                case "orange":
                    return Convert.ToUInt32("E67E22", 16);
                case "purple":
                    return Convert.ToUInt32("9B59B6", 16);
                case "red":
                    return Convert.ToUInt32("E74C3C", 16);
                case "teal":
                    return Convert.ToUInt32("1ABC9C", 16);
            }
            return Convert.ToUInt32("1ABC9C", 16);
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

        public static async Task LevelUp(Profile profile)
        {
            profile.Level++;
            profile.ExperienceNeeded *= 2;
            await _database.UpdateProfile(profile);
        }

        public static async Task IncrementExp(Profile profile)
        {
            profile.ExperienceCurrent++;
            await _database.UpdateProfile(profile);
        }

        public static async Task<EmbedBuilder> FormatProfile(SocketGuildUser arg)
        {
            _profile = await _database.GetProfile(arg.Id);
            TimestampTag date = TimestampTag.FromDateTimeOffset(arg.JoinedAt.GetValueOrDefault(), TimestampTagStyles.LongDate);

            uint color = await SetColor(_profile.Color);
            return new EmbedBuilder()
                .WithTitle($"User Profile: {arg.DisplayName}")
                .WithDescription( $"{_profile.StatusField}\n"
                + "**Ranking**\n"
                + $"**__Total Experience:__** {_profile.ExperienceCurrent}\n"
                + $"**__Level:__** {_profile.Level}\n"
                + $"**__Permission Level:__** {await _database.GetPermissionLevel(_profile.UserID)}\n"
                + $"{_profile.AboutField}\n"
                + "**Age**\n"
                + $"**__Joined Server On:__** {date}")
                .WithColor(color)
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