using System.Net.Mime;
using System.Security;
using Discord;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Modules;
using DiscordBot_tutorial.Quartz.Schedulers;
using DiscordBot_tutorial.Services.LoggingService;
using DiscordBot_tutorial.Services.SettingsService;
using Newtonsoft.Json;

namespace DiscordBot_tutorial
{
    class Program
    {
        private DiscordSocketClient? _client;
        private ILoggingService? _loggingService;
        private ICommandModule? _commandModule;
        private ISettingsService? _settingsService;
        private SchedulerManager? _schedulerManager;

        public static async Task Main(string[] args) 
            => await new Program().MainAsync(args);

        private async Task MainAsync(string[] args)
        {
            //create client 
            _client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    UseInteractionSnowflakeDate = false,
                }
            );

            //configure services
            _loggingService = new LoggingService(_client);
            _settingsService = new SettingsService(new Settings(), _loggingService);
            _settingsService.LoadSettings();
            _schedulerManager = new SchedulerManager(_client, _settingsService);

            //configure modules
            _commandModule = new CommandModule(_client, _settingsService, _loggingService);
            _client.Log += _loggingService.LogAsync;
            _client.SlashCommandExecuted += _commandModule.SlashCommandHandler;

            //configure jobs and triggers
            await _schedulerManager.ConfigureJob2137();

            //start client
            await _client.LoginAsync(TokenType.Bot, _settingsService.Settings.Token);
            await _client.StartAsync();

            //create commands for clean installation 
            if (args.Length > 0 && args[0] == "-ci")
            {
                _loggingService.LogLocal(
                    "Creating server commands, please keep in mind, that it may take up to 1 hour for server to apply changes.",
                    LoggingPriority.Low);
                _client.Ready += _commandModule.CreateCommands;
            }

            //prevent closing the client 
            await Task.Delay(-1);
        }
    }
}