using DiscordBot_tutorial.Services.SettingsService;

namespace DiscordBot_tutorial.Interfaces;

public interface ISettings
{
    public string? Token { get; set; }
    public ulong GuildId { get; set; }
    public List<ChannelSettings>? ChannelsSettingsList { get; set; }
}