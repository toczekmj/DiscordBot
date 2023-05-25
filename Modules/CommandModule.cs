using System.Reflection;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Services.LoggingService;
using Newtonsoft.Json;

namespace DiscordBot_tutorial.Modules;

class CommandModule : ICommandModule
{
    private readonly DiscordSocketClient _client;
    private readonly ISettingsService _settingsService;
    private readonly ILoggingService _loggingService;
    private readonly List<ICommand> _commands = new();

    public CommandModule(DiscordSocketClient client, ISettingsService settingsService, ILoggingService loggingService)
    {
        _client = client;
        _settingsService = settingsService;
        _loggingService = loggingService;
    }

    //TODO: handle handlers dynamic addition
    //TODO: create multiple options
    public async Task CreateCommands()
    {
        foreach (var jsonCommand in _settingsService.Settings.Commands)
        {
            _commands.Add(new Command
                {
                    cmd = CreateCommand(jsonCommand.Name, jsonCommand.Desc, jsonCommand.OptionName,
                        jsonCommand.OptionType, jsonCommand.OptionDesc, jsonCommand.IsRequired),
                    HandlerName = jsonCommand.HandlerName,
                    IsGlobal = jsonCommand.IsGlobal,
                });
        }

        await BuildCommands();
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var cmd = _commands.Find(x => x.cmd!.Name == command.Data.Name);
        var commandHandler = new CommandHandlerModule(_client, _settingsService);
        var method = commandHandler.GetType()
            .GetMethod(cmd!.HandlerName!, BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task)method!.Invoke(commandHandler, new object?[]{command})!;
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
                    await _client.CreateGlobalApplicationCommandAsync(command.cmd!.Build());
                else
                {
                    await guild.CreateApplicationCommandAsync(command.cmd!.Build());
                }
            }
        }
        catch (HttpException e)
        {
            var text = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);
            _loggingService.LogLocal(text, LoggingPriority.Critical);
        }
    }

    private SlashCommandBuilder CreateCommand(string name, string desc, string? optionName = null,
        ApplicationCommandOptionType? optionType = null, string? optionDesc = null, bool? isRequired = null)
    {
        var command = new SlashCommandBuilder()
            .WithName(name)
            .WithDescription(desc);

        if (optionName is not null && optionType is not null && optionDesc is not null)
            command.AddOption(optionName, (ApplicationCommandOptionType)optionType, optionDesc, isRequired: isRequired);

        return command;
    }
}