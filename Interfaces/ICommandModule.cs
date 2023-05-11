using Discord.WebSocket;

namespace DiscordBot_tutorial.Interfaces;

public interface ICommandModule
{
    public Task CreateCommands();
    public Task SlashCommandHandler(SocketSlashCommand command);
}