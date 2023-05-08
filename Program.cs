using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot_tutorial
{
    class Program
    {
        private DiscordSocketClient? _client;
        private LoggingService? _loggingService;
        private CommandModule? _commandModule;
        private ulong _guildId;

        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        private async Task MainAsync(string[] args)
        {
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                UseInteractionSnowflakeDate = false,
            };

            
            if (!File.Exists("private.data"))
            {

                Console.WriteLine("Token: ");
                var response = Console.ReadLine();
                Console.WriteLine("Guid: ");
                var response2 = Console.ReadLine();
                
                File.WriteAllText("private.data", response + ",\n" + response2);
            }

            var text = File.ReadAllText("private.data").Split(',');
            var token = text[0];

            _guildId = ulong.Parse(text[1]);
            _client = new DiscordSocketClient(config);
            _loggingService = new LoggingService(_client);
            _commandModule = new CommandModule(_client, _guildId);

            _client.Log += _loggingService.LogAsync;
            _client.SlashCommandExecuted += _commandModule.SlashCommandHandler;
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            if(args.Length > 0 && args[0] == "-ci")
            {
                Console.WriteLine("Creating server commands, please keep in mind, that it may take up to 1 hour for server to apply changes.");
                _client.Ready += _commandModule.CreateCommands;
            }
            
            
            await Task.Delay(-1);
        }

        
    }
}