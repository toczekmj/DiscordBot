namespace DiscordBot_tutorial.Interfaces;

public interface ISettingsService
{
    public ISettings Settings { get; set; }
    public void LoadSettings();
}