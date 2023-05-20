using Discord;

namespace DiscordBot_tutorial.Interfaces;

public interface IJsonCommand
{
    public string Name { get; set; }
    public string Desc { get; set; }
    
    public string HandlerName { get; set; }
    public string? OptionName { get; set; }
    public ApplicationCommandOptionType? OptionType { get; set; }
    public string? OptionDesc { get; set; }
    public bool? IsRequired { get; set; }
    public bool IsGlobal { get; set; }
}