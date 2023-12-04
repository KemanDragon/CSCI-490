using System.Reflection;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;

using _490Bot.Handlers;
using _490Bot.Utilities;
using _490Bot.Handlers.OffensiveLanguageHandler;

internal class Program
{
    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    private DiscordSocketClient _client;
    private readonly ProfileHandler _profileHandler = new();
    private readonly PassivePermissionsHandler _permissions = new();
    private readonly Database _database = new();
    private CommandHandler _commands;
    private Logger _logger = new Logger();

    private Task Log(LogMessage msg)
    {
        var translateLevel = msg.Severity;
        switch (translateLevel)
        {
            case LogSeverity.Info:
                Console.WriteLine($"[INFO] {msg.ToString()}");
                break;
            case LogSeverity.Verbose:
                Console.WriteLine($"[VERBOSE] {msg.ToString()}");
                break;
            case LogSeverity.Warning:
                Console.WriteLine($"[WARNING] {msg.ToString()}");
                break;
            case LogSeverity.Error:
                Console.WriteLine($"[ERROR] {msg.ToString()}");
                break;
            case LogSeverity.Critical:
                Console.WriteLine($"[CRITICAL] {msg.ToString()}");
                break;
            default:
                Console.WriteLine($"Unable to log a gateway level: {msg.ToString()}");
                break;
        }

        return Task.CompletedTask;
    }

    private async Task MainAsync()
    {
        using var ct = new CancellationTokenSource();
        var task = Login(ct.Token);
        var inputTask = ReadConsoleInputAsync(ct.Token);
        await Task.WhenAny(task, inputTask);
        ct.Cancel();
        await inputTask.ContinueWith(_ => { });
        await task;

        _logger.SetLogChannelId(1181344167394295839);
    }

    private async Task ReadConsoleInputAsync(CancellationToken ct)
    {
        var exit = "exit";
        var help = "help";
        var disconnect = "disconnect";
        var sel = 0;
        while (!ct.IsCancellationRequested)
        {
            var input = await Task.Run(Console.ReadLine);

            if (input.ToLower() == exit)
            {
                sel = 1;
            }

            if (input.ToLower() == help)
            {
                sel = 2;
            }

            if (input.ToLower() == disconnect)
            {
                sel = 3;
            }

            switch (sel)
            {
                case 1:
                    sel = 0;
                    await Cleanup(-1);
                    break;
                case 2:
                    Console.WriteLine("Help command is WIP, lol");
                    break;
                case 3:
                    sel = 0;
                    await _database.CloseConnection();
                    Console.WriteLine("Database connection forced to close.");
                    break;
                default:
                    Console.WriteLine($"'{input}' is not recognized as an internal command. Try 'help' for more information.");
                    sel = 0;
                    break;
            }
        }
    }

    public async Task Login(CancellationToken ct)
    {
        try
        {
            var config = new DiscordSocketConfig
            {
                MessageCacheSize = 2048,
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All
            };
            _client = new DiscordSocketClient(config);
            await RegisterSlashCommands();

            _client.MessageReceived += MessageReceived;
            _client.SlashCommandExecuted += SlashCommandHandler;
            


            _client.MessageReceived += MessageReceived;
            _client.MessageDeleted += _logger.MessageDeletedAsync;
            _client.MessageUpdated += _logger.MessageUpdatedAsync;
            //_client.UserBanned += _logger.LogBannedUserAsync;

            _client.Log += Log;

            try
            {
                var token = "MTE2ODQxOTkyMzc0NDI2MDIxOA.GSgNiK.R8-UmMyBc48oH1iy5HUlS3PXkviKnUSf9REJHA";
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please ensure the token is valid...");
                Console.WriteLine(ex.ToString());
                await Cleanup(-1);
            }

            // Keep this task in limbo until the program is closed.
            await Task.Delay(-1, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Task Terminated");
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task UserJoined(SocketGuildUser arg)
    {
        await ProfileHandler.SetProfile(arg);
        await _database.InsertPermissions(arg.Id);
    }

    public async Task MessageReceived(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message || message.Author.IsBot) return;
        
        if (await _profileHandler.CheckForProfile(message.Author) == false)
        {
            SocketGuildUser user = (SocketGuildUser)message.Author;
            await ProfileHandler.SetProfile(user);
            await _database.InsertPermissions(user.Id);
            
        }
        
        if (ContainsOffensiveLanguage(message.Content))
        {
            var authorId = message.Author.Id;
            var content = message.Content;

            // Send a DM to the user
            var authorUser = message.Author as SocketUser;
            if (authorUser != null)
            {
                await authorUser.SendMessageAsync($"Your message was flagged for offensive language. Please refrain from using inappropriate language.");
            }

            // Log offensive language
            await _logger.LogOffensiveLanguageAsync(authorId, content);
        }
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
                .AddOption("id", ApplicationCommandOptionType.User, "If you want to view another user's profile, provide their ID here.", isRequired: false)
                .AddOption("status", ApplicationCommandOptionType.String, "Update your status field in the profile.", isRequired: false)
                .AddOption("about", ApplicationCommandOptionType.String, "Update your About Me in the profile.", isRequired: false)
                .AddOption("color", ApplicationCommandOptionType.String, "Update the color of the stripe on the profile's embed.", isRequired: false);
            await _client.Rest.CreateGuildCommand(profileCommand.Build(), guildID);
            #endregion

            #region Permissions
            SlashCommandBuilder permsCommand = new SlashCommandBuilder()
                .WithName("perms")
                .WithDescription("View or edit permissions for users.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("id")
                    .WithDescription("Gets or sets the field A")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("update")
                        .WithDescription("Change given user's permission level to the specified number")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("value", ApplicationCommandOptionType.Integer, "The value to change the permissions to", isRequired: true)
                    ).AddOption(new SlashCommandOptionBuilder()
                        .WithName("check")
                        .WithDescription("Checks the permissions of the given user")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("id", ApplicationCommandOptionType.User, "The ID of the user to check perms of", isRequired: true)
                    )
                );
            await _client.Rest.CreateGuildCommand(permsCommand.Build(), guildID);
            #endregion

            await _interactionService.RegisterCommandsGloballyAsync(false);
            _client.InteractionCreated += async (x) => {
                SocketInteractionContext ctx = new(_client, x);
                await _interactionService.ExecuteCommandAsync(ctx, null);
            };
        };
    }

    public async Task HandleProfileCommand(SocketSlashCommand command)
    {
        var option = command.Data.Options?.First().Name;
        var user = command.User as SocketGuildUser;
        EmbedBuilder embedBuilder;
        Profile profile;
        string output;

        switch (option)
        {
            case "ID":
                var value = (SocketGuildUser)command.Data.Options?.First().Value;
                embedBuilder = await ProfileHandler.FormatProfile(value);
                await command.RespondAsync(embed: embedBuilder.Build()); break;
            case "Status":
                profile = await _database.GetProfile(user.Id);
                string newStatus = (string)command.Data.Options?.First().Value;
                ProfileHandler.UpdateStatus(newStatus);
                output = "Your status field has successfully been updated.";
                await command.RespondAsync(output); break;
            case "About":
                profile = await _database.GetProfile(user.Id);
                string newAbout = (string)command.Data.Options?.First().Value;
                ProfileHandler.UpdateAbout(newAbout);
                output = "Your about field has successfully been updated.";
                await command.RespondAsync(output); break;
            default:
                embedBuilder = await ProfileHandler.FormatProfile(user);
                await command.RespondAsync(embed: embedBuilder.Build()); break;
        }
    }
    private bool ContainsOffensiveLanguage(string text)
    {
        
        var offensiveLanguageDetector = new OffensiveLanguageDetector();
        return offensiveLanguageDetector.ContainsOffensiveLanguage(text);
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "profile":
                await HandleProfileCommand(command);
                break;
            case "perms":
                await HandlePermsCommand(command);
                break;
        }
    }
    public async Task HandlePermsCommand(SocketSlashCommand command)
    {
        var option = command.Data.Options.First().Name;
        ulong userID = command.User.Id;
        SocketUser value = (SocketUser)command.Data.Options.First().Value;
        string getOrSet = command.Data.Options.First().Options.First().Name;
        string output;
        int result;

        switch (option)
        {
            case "id":
                if (getOrSet == "check")
                {
                    result = await PassivePermissionsHandler.GetPerms(value.Id);
                    output = $"Permission level of this user is: {result}";
                    await command.RespondAsync(output); break;
                }
                else if (getOrSet == "update")
                {
                    int commandAuthorPerms = await PassivePermissionsHandler.GetPerms(userID);
                    if (commandAuthorPerms != 2)
                    {
                        output = "Insufficient permissions to edit user permissions. Only a moderator can edit permissions.";
                        await command.RespondAsync(output); break;
                    }
                    else
                    {
                        int newPerms = (int)command.Data.Options.First().Options.First().Value;
                        await PassivePermissionsHandler.UpdatePerms(value.Id, newPerms);
                        output = "User permissions successfully updated.";
                        await command.RespondAsync(output); break;
                    }
                }
                break;
                /*case "check":
                    result = await PassivePermissionsHandler.GetPerms(userID);
                    output = $"Your permission level is: {result}";
                    await command.RespondAsync(output); break;
                case "update":
                    output = "You are not able to set your own permissions.";
                    await command.RespondAsync(output); break;*/
        }
    }

    private async Task Cleanup(int exitCode)
    {
        Console.WriteLine($"Shutting down with exit code {exitCode}...");
        // Additional cleanup actions to go here as other functions are finished
        await _database.CloseConnection(); // Just in case there's a lingering DB connection, call the method to close it
        Environment.Exit(exitCode);
        await Task.CompletedTask;
    }
}