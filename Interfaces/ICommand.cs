using Discord;

namespace DiscordBot_tutorial.Interfaces;

public interface ICommand
{
    public SlashCommandBuilder? cmd { get; set; }
    public bool IsGlobal { get; set; }
    public string? HandlerName { get; set; }
}