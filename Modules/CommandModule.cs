using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Services.LoggingService;
using DiscordBot_tutorial.Services.SettingsService;
using Newtonsoft.Json;

namespace DiscordBot_tutorial.Modules;

partial class CommandModule
{
    private readonly DiscordSocketClient _client;
    private readonly SettingsService _settingsService;
    private readonly ILoggingService _loggingService;
    private List<ICommand> _commands = new();

    public CommandModule(DiscordSocketClient client, SettingsService settingsService, ILoggingService loggingService)
    {
        _client = client;
        _settingsService = settingsService;
        _loggingService = loggingService;
    }

    //TODO: make commands creation dynamic using reflections and load data from JSON file
    //seems like whole assembly creator needs to be done for this, but not sure
    public async Task CreateCommands()
    {
        var cmd1 = new Command
        {
            cmd = CreateCommand("papiez", "this is a test command description", "askemeanything",
                ApplicationCommandOptionType.String, "provide text here"),
            HandlerName = "PapiezCommandHandler",
            IsGlobal = false,
        };

        var cmd2 = new Command
        {
            cmd = CreateCommand("testsecondcommand", "this is a second command description", "ask",
                ApplicationCommandOptionType.String, "text here", true),
            HandlerName = "SecondCommandHandler",
            IsGlobal = false,
        };
        
        _commands.Add(cmd1); 
        _commands.Add(cmd2); 
        
        await BuildCommands();
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var cmd = _commands.Find(x => x.cmd.Name == command.Data.Name);
        var t = new object?[1];
        t[0] = command;
        var method = this.GetType().GetMethod(cmd!.HandlerName, BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task)method.Invoke(this, t);
        await task.ConfigureAwait(false);
    }
    private async Task BuildCommands()
    {
        var guild = _client.GetGuild(_settingsService.Settings.GuildId);
        try
        {
            foreach (var command in _commands)
            {
                if (command.IsGlobal)
                    await _client.CreateGlobalApplicationCommandAsync(command.cmd.Build());
                else
                {
                    await guild.CreateApplicationCommandAsync(command.cmd.Build());
                }
            }
        }
        catch (HttpException e)
        {
            var text = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);
            _loggingService.LogLocal(text, LoggingPriority.Critical);
        }
    }

    private SlashCommandBuilder CreateCommand(string name, string desc, string? optionName = null, ApplicationCommandOptionType? optionType = null, string? optionDesc = null, bool? isRequired = null)
    {
        var command = new SlashCommandBuilder()
            .WithName(name)
            .WithDescription(desc);

        if (optionName is not null && optionType is not null && optionDesc is not null)
            command.AddOption(optionName, (ApplicationCommandOptionType)optionType, optionDesc, isRequired: isRequired);

        return command;
    }
}