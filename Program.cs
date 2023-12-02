using System;
using System.Reflection;

using Discord;
using Discord.WebSocket;
using Discord.Interactions;

using _490Bot.Handlers;
using _490Bot.Utilities;

internal class Program {
    public static Task Main(string[] args) => new Program().MainAsync();
    private DiscordSocketClient _client;
    private Lazy<ProfileHandler> _profileHandler;

    private Logger _logger;
    private Task Log(LogMessage msg) {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task MainAsync() {
        using var ct = new CancellationTokenSource();
        var task = Login(ct.Token);
        var inputTask = ReadConsoleInputAsync(ct.Token);
        await Task.WhenAny(task, inputTask);
        ct.Cancel();
        await inputTask.ContinueWith(_ => { });
        await task;
    }

    private async Task ReadConsoleInputAsync(CancellationToken ct) {
        var exit = "exit";
        var help = "help";
        var sel = 0;
        while (!ct.IsCancellationRequested) {
            var input = await Task.Run(Console.ReadLine);

            if (input.ToLower() == exit) {
                sel = 1;
            }

            if (input.ToLower() == help) {
                sel = 2;
            }

            switch (sel) {
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

    public async Task Login(CancellationToken ct) {
        try {
            var config = new DiscordSocketConfig();
            config.MessageCacheSize = 2048;
            config.AlwaysDownloadUsers = true;
            config.GatewayIntents = GatewayIntents.All;
            _client = new DiscordSocketClient(config);

            _client.MessageReceived += MessageReceived;
            _client.Log += Log;
            RegisterSlashCommands();
            
            try {
                var token = "MTE2ODQxOTkyMzc0NDI2MDIxOA.GSgNiK.R8-UmMyBc48oH1iy5HUlS3PXkviKnUSf9REJHA";
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
            } catch (Exception ex) {
                Console.WriteLine("Please ensure the token is valid...");
                Console.WriteLine(ex.ToString());
                await Cleanup(-1);
            }

            // Keep this task in limbo until the program is closed.
            await Task.Delay(-1, ct);
        } catch (Exception ex) {
            Console.WriteLine("Task Terminated");
            Console.WriteLine(ex.ToString());
        }
    }

    private void RegisterSlashCommands() {
        _client.Ready += async () => {
            var _interactionService = new InteractionService(_client.Rest);
            await _interactionService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            await _interactionService.RegisterCommandsGloballyAsync(false);
            _client.InteractionCreated += async (x) => {
                var ctx = new SocketInteractionContext(_client, x);
                await _interactionService.ExecuteCommandAsync(ctx, null);
            };
        };
    }

    private async Task Cleanup(int exitCode) {
        Console.WriteLine($"Shutting down with exit code {exitCode}...");
        // Additional cleanup actions to go here as other functions are finished
        Environment.Exit(exitCode);
        await Task.CompletedTask;
    }

    private char commandPrefix = '!'; // Add this line
    private async Task MessageReceived(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message || message.Author.IsBot) return;

        string messageContent = message.Content;
        string responseMessage = $"Message received:{messageContent}";
        var reply = new MessageReference(message.Id);
        await message.Channel.SendMessageAsync(responseMessage, false, null, null, null, reply);
    }

    
                
            
    














}