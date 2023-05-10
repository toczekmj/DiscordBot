using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Services.SettingsService;

public class Settings : ISettings
{
    public string? Token { get; set; }
    public ulong GuildId { get; set; }
    public List<ChannelSettings>? ChannelsSettingsList { get; set; }
}