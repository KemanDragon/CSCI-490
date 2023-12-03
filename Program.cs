using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Discord.Commands;

using _490Bot.Handlers;
using _490Bot.Utilities;

internal class Program 
{
    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    private DiscordSocketClient _client;
    private ProfileHandler _profileHandler = new();
    private Logger _logger;
    private CommandService _commands;
    private IServiceProvider _services;

    private Task Log(LogMessage msg) 
    {
        var translateLevel = msg.Severity;
        switch(translateLevel)
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
    }

    private async Task ReadConsoleInputAsync(CancellationToken ct) 
    {
        var exit = "exit";
        var help = "help";
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

            switch (sel) 
            {
                case 1:
                    sel = 0;
                    await Cleanup(-1);
                    break;
                case 2:
                    Console.WriteLine("Help command is WIP, lol");
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
            _commands = new();
            _services = new ServiceCollection().BuildServiceProvider();

            _client.MessageReceived += MessageReceived;
            // _client.MessageDeleted += _logger.MessageDeletedAsync;
            
            
            _client.Log += Log;
            RegisterSlashCommands();
            
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

    private void RegisterSlashCommands() 
    {
        _client.Ready += async () => 
        {
            var _interactionService = new InteractionService(_client.Rest);
            await _interactionService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            await _interactionService.RegisterCommandsGloballyAsync(false);
            _client.InteractionCreated += async (x) => {
                var ctx = new SocketInteractionContext(_client, x);
                await _interactionService.ExecuteCommandAsync(ctx, null);
            };
        };
    }

    public async Task UserJoined(SocketGuildUser arg)
    {
        await _profileHandler.SetProfile(arg);
    }

    public async Task MessageReceived(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message || message.Author.IsBot) return;
    }

    public async Task RegisterCommands(string commandName, string description)
    {
        _client.Ready += async () =>
        {
            await RegisterSlashCommand(commandName, description);
        };

        _client.MessageReceived += HandleCommand;
    }

    public async Task HandleCommand(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(_client, message);

        int argPos = 0;
        if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                return;
            }
        }
    }

    public async Task RegisterSlashCommand(string commandName, string description)
    {
        ulong guildID = 1158429342616006727;

        var command = new SlashCommandBuilder().WithName(commandName).WithDescription(description).Build();

        await _client.Rest.CreateGuildCommand(command, guildID);
    }

    private async Task Cleanup(int exitCode) 
    {
        Console.WriteLine($"Shutting down with exit code {exitCode}...");
        // Additional cleanup actions to go here as other functions are finished
        Environment.Exit(exitCode);
        await Task.CompletedTask;
    }
}