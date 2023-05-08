using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot_tutorial;

class LoggingService
{
    public LoggingService(DiscordSocketClient client)
    {
        client.Log += LogAsync;
    }

    public Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}" 
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
        {
            Console.WriteLine($"[General/{message.Severity}] {message}");
        }

        return Task.CompletedTask;
    }
}