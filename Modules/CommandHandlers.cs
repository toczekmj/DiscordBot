using Discord.WebSocket;
using DiscordBot_tutorial.Services.LoggingService;

namespace DiscordBot_tutorial.Modules;

public partial class CommandModule
{
    private async Task PapiezCommandHandler(SocketSlashCommand command)
    {
        string t = (string)command.Data.Options.First().Value;
        await command.RespondAsync(t);
    }
    
    private async Task TestCommandHandler(SocketSlashCommand command)
    {
        await command.RespondAsync("xdxdxd");
    }
}