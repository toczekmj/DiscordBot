using System.Drawing;
using Discord;
using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Services.LoggingService;
using Newtonsoft.Json;
using Quartz.Util;

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
            _loggingService.LogLocal(
                "Failed to read settings from file. Seems like it may be corrupted. Do you wand to delete configuration file? (y/N)",
                LoggingPriority.Critical);
            var response = Console.ReadLine();
            if (response?.ToLower() is "y")
            {
                var backup = File.ReadAllText("private.data");
                File.WriteAllText("private.data.bak", backup);
                File.Delete("private.data");
                _loggingService.LogLocal(
                    "Private.data was deleted, but it has been backed up to private.data.bak in case for you to restore any data",
                    LoggingPriority.Low);
            }
            else
            {
                _loggingService.LogLocal("Config was not deleted. Please look into the file and try to repair it.",
                    LoggingPriority.Warning);
            }

            Environment.Exit(0);
        }
    }
    
    private void CreateFile()
    {
        string? token;
        string? guid;
        List<JsonCommand> jsonCommands = new List<JsonCommand>();

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

        Console.WriteLine("How many commands: ");

        bool response;
        int intResponse;
        do
        {
            response = int.TryParse(Console.ReadLine(), out intResponse);
        } while (!response);

        //TODO: test and debug this
        jsonCommands.AddRange(
            LoadJsonCommand(intResponse)
            );

        var settings = new Settings
        {
            Token = token,
            GuildId = ulong.Parse(guid),
            Commands = jsonCommands,
        };

        var serializedSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText("private.data", serializedSettings);
    }

    private IEnumerable<JsonCommand> LoadJsonCommand(int i)
    {
        ApplicationCommandOptionType? optionType = null;
        bool? isRequired = null;
        string? optionName = null,
            optionDesc = null;

        for (var a = 0; a < i; a++)
        {
            string? name = null;
            do
            {
                Console.WriteLine("Name:");
                name = Console.ReadLine();

            } while (name.IsNullOrWhiteSpace());

            string? desc = null;
            do
            {
                Console.WriteLine("Description:");
                desc = Console.ReadLine();
            } while (desc.IsNullOrWhiteSpace());

            string? handlerName = null;
            do
            {
                Console.WriteLine("Handler Name:");
                handlerName = Console.ReadLine();
            } while (handlerName.IsNullOrWhiteSpace());

            string? temp = null;
            do
            {
                Console.WriteLine("Is Global  (y/n):");
                temp = Console.ReadLine();
            } while (temp.IsNullOrWhiteSpace());
            bool? isGlobal = temp!.ToLower() == "y";

            Console.WriteLine("Add option? (y/n)");
            if (Console.ReadLine() == "y")
            {
                do
                {
                    Console.WriteLine("Option Name:");
                    optionName = Console.ReadLine();

                } while (optionName.IsNullOrWhiteSpace());
                
                do
                {
                    Console.WriteLine("Option Description:");
                    optionDesc = Console.ReadLine();

                } while (optionDesc.IsNullOrWhiteSpace());

                do
                {
                    Console.WriteLine("Option type:");
                    var enumValues = Enum.GetValues<ApplicationCommandOptionType>();
                    
                    for (var n = 1; n <= enumValues.Length; n++)
                        Console.WriteLine($"{(int)enumValues[n]} - {enumValues[n]}");

                    var response = byte.TryParse(Console.ReadLine(), out var responseResult);
                    optionType = (ApplicationCommandOptionType?)responseResult;

                } while (optionType is 0 or null);
                
                
                do
                {
                    Console.WriteLine("Is Required? (y/n)");
                    temp = Console.ReadLine();
                } while (temp.IsNullOrWhiteSpace());
                isRequired = temp.ToLower() == "y";
            }

            yield return new JsonCommand
            {
                Desc = desc!,
                HandlerName = handlerName!,
                IsGlobal = (bool)isGlobal,
                Name = name!,
                IsRequired = isRequired,
                OptionDesc = optionDesc,
                OptionName = optionName,
                OptionType = optionType
            };
        }
    }
}