using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Services.LoggingService;
using Newtonsoft.Json;

namespace DiscordBot_tutorial.Services.SettingsService;

public class SettingsService
{
    public ISettings Settings;
    private readonly ILoggingService _loggingService;
    public SettingsService(ISettings settings, ILoggingService loggingService)
    {
        Settings = settings;
        _loggingService = loggingService;
    }

    public void LoadSettings()
    {
        if (!File.Exists("private.data"))
            CreateFile(); 
        ReadSettingsFromFile();
    }

    private void ReadSettingsFromFile()
    {
        var text = File.ReadAllText("private.data");
        try
        {
            Settings = JsonConvert.DeserializeObject<Settings>(text);
            _loggingService.LogLocal("Settings loaded!", LoggingPriority.Information);
        }
        catch (Exception e)
        {
            _loggingService.LogLocal("Failed to read settings from file. Seems like it may be corrupted. Do you wand to delete configuration file? (y/N)", LoggingPriority.Critical);
            var response = Console.ReadLine();
            if(response is "y" or "Y")
            {
                var backup = File.ReadAllText("private.data");
                File.WriteAllText("private.data.bak", backup);
                File.Delete("private.data");
                _loggingService.LogLocal("Private.data was deleted, but it has been backed up to private.data.bak in case for you to restore any data", LoggingPriority.Low);
            }
            else
            {
                _loggingService.LogLocal("Config was not deleted. Please look into the file and try to repair it.", LoggingPriority.Warning);
            }
            throw;
        }
    }

    private void CreateFile()
    {
        string? token;
        string? guid;
        
        do
        {
            _loggingService.LogLocal("Token: ", LoggingPriority.Information);
            token = Console.ReadLine();
        } while (string.IsNullOrEmpty(token));

        do
        {
            _loggingService.LogLocal("Guid: ", LoggingPriority.Information);
            guid = Console.ReadLine();
        } while (string.IsNullOrEmpty(guid));

        _loggingService.LogLocal("Channel rules (ChannelID,CommandName) type x to abort (channel id can be empty when command is channel-independent)", LoggingPriority.Information);
        var settingsList = ReadChannelsSettings();

        var settings = new Settings
        {
            Token = token,
            GuildId = ulong.Parse(guid),
            ChannelsSettingsList = settingsList,
        };

        var serializedSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText("private.data", serializedSettings);
    }

    private List<ChannelSettings> ReadChannelsSettings()
    {
        var response = Console.ReadLine();
        var channelsList = new List<ChannelSettings>();

        do
        {
            if (response == "x" || string.IsNullOrWhiteSpace(response) || string.IsNullOrWhiteSpace(response))
                break;

            try
            {
                var t = response.Split(',');
                channelsList.Add(new ChannelSettings
                {
                    ChannelId = ulong.Parse(t.First()),
                    CommandName = t.Last().Trim(),
                });
            }
            catch (Exception e)
            {
                _loggingService.LogLocal("Provided data was not correct.", LoggingPriority.Information);
            }

            response = Console.ReadLine();
        } while (response != "x");

        return channelsList;
    }
}