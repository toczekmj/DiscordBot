using System.Reflection;
using System.Reflection.Emit;
using Discord;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Jobs;
using DiscordBot_tutorial.Modules;
using DiscordBot_tutorial.Services.LoggingService;
using DiscordBot_tutorial.Services.SettingsService;
using Quartz;
using Quartz.Impl;

namespace DiscordBot_tutorial
{
    class Program
    {
        private DiscordSocketClient? _client;
        private ILoggingService? _loggingService;
        private CommandModule? _commandModule;
        private SettingsService? _settingsService;
        
        public static async Task Main(string[] args) => await new Program().MainAsync(args);

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
            await ConfigureScheduler(_client, _settingsService);
            
            //configure modules
            _commandModule = new CommandModule(_client, _settingsService, _loggingService);
            _client.Log += _loggingService.LogAsync;
            _client.SlashCommandExecuted += _commandModule.SlashCommandHandler;

            //start client
            await _client.LoginAsync(TokenType.Bot, _settingsService.Settings.Token);
            await _client.StartAsync();

            //create commands for clean installation 
            if(args.Length > 0 && args[0] == "-ci")
            {
                _loggingService.LogLocal("Creating server commands, please keep in mind, that it may take up to 1 hour for server to apply changes.", LoggingPriority.Low);
                _client.Ready += _commandModule.CreateCommands;
            }

            //prevent closing the client 
            await Task.Delay(-1);
        }

        private async Task ConfigureScheduler(IAsyncDisposable client, SettingsService settings)
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            
            await scheduler.Start();
            
            var job = JobBuilder.Create<Job2137>()
                .WithIdentity("job1", "group1")
                .Build();

            job.JobDataMap.Put("_client", client);
            job.JobDataMap.Put("_settings", settings);
            
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever()
                )
                .Build();
            
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}