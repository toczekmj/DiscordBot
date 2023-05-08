using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBot_tutorial;

class CommandModule
{
    private readonly DiscordSocketClient _client;
    private readonly ulong _guildId;
        
    public CommandModule(DiscordSocketClient client, ulong guildId)
    {
        _guildId = guildId;
        _client = client;
    }
        
    public async Task CreateCommands()
    {
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("czy-mozna");
        globalCommand.WithDescription("Czy mozna?");
        globalCommand.AddOption("zapytanie", ApplicationCommandOptionType.String, "zadaj pytanie", isRequired: true);

        var listRoles = new SlashCommandBuilder()
            .WithName("list-roles")
            .WithDescription("List all roles of an user.")
            .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);
            
        try
        {
            await _client.Rest.CreateGuildCommand(listRoles.Build(), _guildId);
            await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        }
        catch (HttpException e)
        {
            var json = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
    
    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "list-roles":
                await HandleListRoleCommand(command);
                break;
            case "czy-mozna":
                await HandleFirstGlobalCommand(command);
                break;
        }
    }

    private async Task HandleFirstGlobalCommand(SocketSlashCommand command)
    {
        var text = (string)command.Data.Options.First().Value;
        var response = string.Empty;

        if (text.Contains('?'))
        {
            response += "Pytasz czy można " + text + '\n';
        }
        else response += "Pytasz czy można " + text + "?\n";
        
        
        
        FileAttachment attachment =
            new FileAttachment("/Users/toczekmj/RiderProjects/DiscordBot_tutorial/mozna.mp4", "mozna.mp4");

        await command.RespondWithFileAsync(attachment, response + " @everyone");
    }

    private async Task HandleListRoleCommand(SocketSlashCommand command)
    {
        var guildUser = (SocketGuildUser)command.Data.Options.First().Value;
        var roleList = string.Join("\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            .WithTitle("Roles")
            .WithDescription(roleList)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);

    }
}