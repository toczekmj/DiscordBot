using System.Runtime.CompilerServices;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Modules;

public class CommandHandlerModule
{
    private readonly DiscordSocketClient _client;
    private readonly ISettingsService _settingsService;

    public CommandHandlerModule(DiscordSocketClient client, ISettingsService settingsService)
    {
        _client = client;
        _settingsService = settingsService;
    }

    private async Task PapiezCommandHandler1(SocketSlashCommand command)
    {
        await _client.GetGuild(_settingsService.Settings.GuildId)
            .GetTextChannel(1105183278266331156)
            .SendMessageAsync("wiadomosc");
        var t = (string)command.Data.Options.First().Value;
        await command.RespondAsync(t);
    }

    private async Task TestCommandHandler(SocketSlashCommand command)
    {
        await command.RespondAsync("xdxdxd");
    }
}