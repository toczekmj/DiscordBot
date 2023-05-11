using Discord.WebSocket;
using DiscordBot_tutorial.Services.LoggingService;

namespace DiscordBot_tutorial.Modules;

public partial class CommandModule
{
    private async Task PapiezCommandHandler(SocketSlashCommand command)
    {
        await _client.GetGuild(_settingsService.Settings.GuildId).GetTextChannel(1105183278266331156).SendMessageAsync("wiadomosc");
        string t = (string)command.Data.Options.First().Value;
        await command.RespondAsync(t);
    }
    
    private async Task TestCommandHandler(SocketSlashCommand command)
    {
        await command.RespondAsync("xdxdxd");
    }
}