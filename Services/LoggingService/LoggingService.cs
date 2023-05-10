using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Services.LoggingService;

class LoggingService : ILoggingService
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

    public void LogLocal(string message, LoggingPriority priority)
    {
        Console.WriteLine(message);
    }
}