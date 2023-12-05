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
    private Logger _logger;
    private LangFilter _langFilter = new();
    private LangFilter MuteFunc = new();
    //private CommandService _commands;
    private IServiceProvider _services;

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

        //_logger.SetLogChannelId(1181344167394295839);
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
          //  _client.MessageDeleted += _logger.MessageDeletedAsync;
            //_client.MessageUpdated += _logger.MessageUpdatedAsync;
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
        Profile profile = await _database.GetProfile(message.Author.Id);
        await ProfileHandler.IncrementExp(profile);
        if (profile.ExperienceCurrent == profile.ExperienceNeeded)
        {
            await ProfileHandler.LevelUp(profile);          
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
        var filter = new LangFilter();
        string badWordFound = filter.langFilter(message.Content);
        if (badWordFound != null){
        Database DBConnection = new Database();
        DateTime TimeStamp = DateTime.Now;
        await DBConnection.InsertLogAsync(message.Author.Id.ToString(), message.Author.Username, TimeStamp, badWordFound);
        var guildUser = message.Author as SocketGuildUser;
        var _muteFunc = new MuteFunc(_client);
        await _muteFunc.MuteUserAsync(guildUser, TimeSpan.FromMinutes(1));
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

                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("id")
                    .WithDescription("Provide a user to view a profile. Leave empty to default to your profile.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("id", ApplicationCommandOptionType.User, "The user to view the profile for.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("status")
                    .WithDescription("Update your profile's status field.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.String, "The new status to apply to the profile.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("about")
                    .WithDescription("Update your profile's About Me field.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.String, "The new About Me to apply to the profile.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("color")
                    .WithDescription("Update the color of the embed in the profile.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("color")
                        .WithDescription("Select the color you want.")
                        .WithRequired(true)
                        .AddChoice("Blue", "blue")
                        .AddChoice("Gold", "gold")
                        .AddChoice("Green", "green")
                        .AddChoice("Magenta", "magenta")
                        .AddChoice("Orange", "orange")
                        .AddChoice("Purple", "purple")
                        .AddChoice("Red", "red")
                        .AddChoice("Teal", "teal")
                        .WithType(ApplicationCommandOptionType.String)
                    )
            );
            await _client.Rest.CreateGuildCommand(profileCommand.Build(), guildID);
            #endregion

            #region Permissions
            SlashCommandBuilder permsCommand = new SlashCommandBuilder()
                .WithName("perms")
                .WithDescription("View or edit permissions for users.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("update")
                    .WithDescription("Update perms of a user. Note: Only moderators may update permissions.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("id", ApplicationCommandOptionType.User, "The user to update perms for.", isRequired: true)
                    .AddOption("value", ApplicationCommandOptionType.Integer, "The value to update perms to", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("check")
                    .WithDescription("Checks the permissions of the given user")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("id", ApplicationCommandOptionType.User, "The ID of the user to check perms of", isRequired: true)
                );
            await _client.Rest.CreateGuildCommand(permsCommand.Build(), guildID);
            #endregion

            #region Log
            SlashCommandBuilder logCommand = new SlashCommandBuilder()
                .WithName("log")
                .WithDescription("View logs for a user")
                .AddOption("id", ApplicationCommandOptionType.User, "The ID of the user to lookup.", isRequired: true);
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
        var option = command.Data.Options.First().Name;
        SocketGuildUser user = command.User as SocketGuildUser;
        ulong userID = command.User.Id;
        EmbedBuilder embedBuilder;
        Profile profile = new();
        string output;
        switch (option)
        {
            case "id":
                var valueOption = command.Data.Options.First().Options.First();
                var value = valueOption?.Value as SocketGuildUser;
                embedBuilder = await ProfileHandler.FormatProfile(value);
                await command.RespondAsync(embed: embedBuilder?.Build());
                break;
            case "status":
                profile = await _database.GetProfile(userID);
                string newStatus = (string)command.Data.Options.First().Options.First().Value;
                ProfileHandler.UpdateStatus(profile, newStatus);
                output = "Your status field has successfully been updated.";
                await command.RespondAsync(output);
                break;
            case "about":
                profile = await _database.GetProfile(userID);
                string newAbout = (string)command.Data.Options.First().Options.First().Value;
                ProfileHandler.UpdateAbout(profile, newAbout);
                output = "Your about field has successfully been updated.";
                await command.RespondAsync(output);
                break;
            case "color":
                profile = await _database.GetProfile(userID);
                string newColor = (string)command.Data.Options.First().Options.First().Value;
                ProfileHandler.UpdateColor(profile, newColor);
                output = "Your profile's color has successfully been updated.";
                await command.RespondAsync(output);
                break;
            default:
                embedBuilder = await ProfileHandler.FormatProfile(user);
                await command.RespondAsync(embed: embedBuilder?.Build());
                break;
        }
    }
    private bool ContainsOffensiveLanguage(string text)
    {
        // Assuming you have an OffensiveLanguageDetector class
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

    public async Task HandleLogCommand(SocketSlashCommand command)
    {
        var option = command.Data.Options.First();
        ulong userID = command.User.Id;
        int perm = await PassivePermissionsHandler.GetPerms(userID);
        if (perm != 2)
        {
            await command.RespondAsync("Only a moderator can view logs");
            return;
        }

        userID = (ulong)command.Data.Options.First().Options.First().Value;
        Logs log = await _database.GetLog(userID);
        if (log == null)
        {
            await command.RespondAsync("Log not found.");
        }
        EmbedBuilder embedBuilder = new EmbedBuilder();
    }
    public async Task HandlePermsCommand(SocketSlashCommand command)
    {
        var option = command.Data.Options.First().Name;
        ulong userID = command.User.Id;

        string getOrSet = command.Data.Options.First().Name;
        SocketUser? user;
        string output;
        int result;

        switch (option)
        {
            case "check":
                user = (SocketUser?)command.Data.Options.First().Options.First()?.Value;
                if (user == null)
                {
                    user = command.User;
                }
                result = await PassivePermissionsHandler.GetPerms(user.Id);
                output = $"Permission level of this user is: {result}";
                await command.RespondAsync(output); break;

            case "update":
                int commandAuthorPerms = await PassivePermissionsHandler.GetPerms(userID);
                if (commandAuthorPerms != 2)
                {
                    output = "Insufficient permissions to edit user permissions. Only a moderator can edit permissions.";
                    await command.RespondAsync(output); break;
                }
                else
                {
                    int newPerms = (int)command.Data.Options.First().Options.FirstOrDefault(option => option.Name == "value").Value;
                    user = (SocketUser)command.Data.Options.First().Options.FirstOrDefault(option => option.Name == "id").Value;
                    await PassivePermissionsHandler.UpdatePerms(user.Id, newPerms);
                    output = "User permissions successfully updated.";
                    await command.RespondAsync(output); break;
                }
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

    //private char commandPrefix = '!'; // Add this line



    
        
        
        

}

        





