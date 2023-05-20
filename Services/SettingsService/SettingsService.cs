using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Services.LoggingService;
using Newtonsoft.Json;

namespace DiscordBot_tutorial.Services.SettingsService;

public class SettingsService : ISettingsService
{
    public ISettings Settings { get; set; }
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

    //TODO: read IJsonCommands from file 
    private void ReadSettingsFromFile()
    {
        var text = File.ReadAllText("private.data");
        try
        {
            Settings = JsonConvert.DeserializeObject<Settings>(text)!;
            _loggingService.LogLocal("Settings loaded!", LoggingPriority.Information);
        }
        catch (Exception)
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
            Environment.Exit(0);
        }
    }

    //TODO: update CreateFile to handle new commands builder and handler
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
        
        var settings = new Settings
        {
            Token = token,
            GuildId = ulong.Parse(guid),
            //TODO: add List<IJsonCommand> here 
        };

        var serializedSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText("private.data", serializedSettings);
    }
}