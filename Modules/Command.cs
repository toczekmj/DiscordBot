using Discord;
using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Modules;

public class Command : ICommand
{
    public SlashCommandBuilder cmd { get; set; }
    public bool IsGlobal { get; set; }
    public string HandlerName { get; set; }
}