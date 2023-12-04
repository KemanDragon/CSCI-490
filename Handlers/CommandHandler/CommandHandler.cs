using System.Reflection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions;

using _490Bot.Utilities;

namespace _490Bot.Handlers {
    public class CommandHandler {
        public DiscordSocketClient _client {  get; set; }
        private readonly Database _database = new();
        public CommandHandler(DiscordSocketClient _client)
        {
            this._client = _client;
        }
        public async Task RegisterSlashCommands()
        {
            _client.Ready += async () =>
            {
                ulong guildID = 1158429342616006727;
                var _interactionService = new InteractionService(_client.Rest);
                await _interactionService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

                #region Profile
                SlashCommandBuilder profileCommand = new SlashCommandBuilder()
                    .WithName("profile")
                    .WithDescription("View or edit your user profile.")
                    .AddOption("ID", ApplicationCommandOptionType.User, "If you want to view another user's profile, provide their ID here.", isRequired: false)
                    .AddOption("Status", ApplicationCommandOptionType.String, "Update your status field in the profile.", isRequired: false)
                    .AddOption("About", ApplicationCommandOptionType.String, "Update your About Me in the profile.", isRequired: false)
                    .AddOption("Color", ApplicationCommandOptionType.String, "Update the color of the stripe on the profile's embed.", isRequired: false);
                await _client.Rest.CreateGuildCommand(profileCommand.Build(), guildID);
                #endregion

                #region Permissions
                SlashCommandBuilder permsCommand = new SlashCommandBuilder()
                    .WithName("perms")
                    .WithDescription("View or edit permissions for users.")
                    .AddOption("ID", ApplicationCommandOptionType.User, "Specify the user to edit perms of. Note: Permissions may only be edited by a moderator.", isRequired: false)
                    .AddOption("Check", ApplicationCommandOptionType.User, "Check permissions of the given user with the specified User ID.")
                    .AddOption("Update", ApplicationCommandOptionType.Integer, "Change given user's permission level to the specified number.");
                await _client.Rest.CreateGuildCommand(permsCommand.Build(), guildID);
                #endregion

                await _interactionService.RegisterCommandsGloballyAsync(false);
                _client.InteractionCreated += async (x) => {
                    SocketInteractionContext ctx = new(_client, x);
                    await _interactionService.ExecuteCommandAsync(ctx, null);
                };
            };
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch(command.Data.Name)
            {
                case "profile":
                    await HandleProfileCommand(command);
                    break;
                case "perms":
                    await HandlePermsCommand(command);
                    break;
            }
        }

        public async Task HandleProfileCommand(SocketSlashCommand command)
        {
            var option = command.Data.Options?.First().Name;
            EmbedBuilder embedBuilder;
            SocketGuildUser user = command.User as SocketGuildUser;
            Profile profile;
            string output;
            
            switch (option)
            {
                case "ID":
                    var value = (SocketGuildUser)command.Data.Options?.First().Value;
                    embedBuilder = await ProfileHandler.FormatProfile(value);
                    await command.Channel.SendMessageAsync(embed: embedBuilder.Build()); break;
                case "Status":
                    profile = await _database.GetProfile(user.Id);
                    string newStatus = (string)command.Data.Options?.First().Value;
                    ProfileHandler.UpdateStatus(newStatus);
                    output = "Your status field has successfully been updated.";
                    await command.Channel.SendMessageAsync(output); break;
                case "About":
                    profile = await _database.GetProfile(user.Id);
                    string newAbout = (string)command.Data.Options?.First().Value;
                    ProfileHandler.UpdateAbout(newAbout);
                    output = "Your about field has successfully been updated.";
                    await command.Channel.SendMessageAsync(output); break;
                default:
                    embedBuilder = await ProfileHandler.FormatProfile(user);
                    await command.Channel.SendMessageAsync(embed: embedBuilder.Build()); break;
            }
        }

        public async Task HandlePermsCommand(SocketSlashCommand command)
        {
            var option = command.Data.Options.First().Name;
            ulong userID = command.User.Id;
            string output;
            int result;

            switch (option)
            {
                case "ID":
                    var value = (ulong)command.Data.Options.First().Value;
                    var getOrSet = command.Data.Options.First().Options.First().Name;
                    if(getOrSet == "get")
                    {
                        result = await PassivePermissionsHandler.GetPerms(value);
                        output = $"Permission level of this user is: {result}";
                        await command.Channel.SendMessageAsync(output); break;
                    } 
                    else
                    {
                        int commandAuthorPerms = await PassivePermissionsHandler.GetPerms(command.User.Id);
                        if (commandAuthorPerms != 2) 
                        {
                            output = "Insufficient permissions to edit user permissions. Only a moderator can edit permissions.";
                            await command.Channel.SendMessageAsync(output); break;
                        }
                        else
                        {
                            int newPerms = (int)command.Data.Options.First().Options.First().Value;
                            await PassivePermissionsHandler.UpdatePerms(value, newPerms);
                            output = "User permissions successfully updated.";
                            await command.Channel.SendMessageAsync(output); break;
                        }
                    }
                case "Get":
                    result = await PassivePermissionsHandler.GetPerms(userID);
                    output = $"Your permission level is: {result}";
                    await command.Channel.SendMessageAsync(output); break;
                case "Set":
                    output = "You are not able to set your own permissions.";
                    await command.Channel.SendMessageAsync(output); break;
            }
        }
    }
}
