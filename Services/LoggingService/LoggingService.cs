using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot_tutorial.Interfaces;

namespace DiscordBot_tutorial.Services.LoggingService;

internal class LoggingService : ILoggingService
{
    public LoggingService(DiscordSocketClient client)
    {
        client.Log += LogAsync;
    }

    public Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            ChangeTextColour(LoggingPriority.GeneralInformation);
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}" 
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
            Console.ResetColor();
        }
        else
        {
            if(message.Message.Contains("unhandled exception"))
                ChangeTextColour(LoggingPriority.Critical);
            else ChangeTextColour(message.Severity == LogSeverity.Warning
                ? LoggingPriority.Warning
                : LoggingPriority.GeneralInformation);
            Console.WriteLine($"[General/{message.Severity}] {message}");
            Console.ResetColor();
        }

        return Task.CompletedTask;
    }

    public void LogLocal(string message, LoggingPriority priority)
    {
        ChangeTextColour(priority);
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void ChangeTextColour(LoggingPriority priority)
    {
        Console.ForegroundColor = priority switch
        {
            LoggingPriority.Critical => ConsoleColor.Red,
            LoggingPriority.Warning => ConsoleColor.Yellow,
            LoggingPriority.Low => ConsoleColor.DarkGreen,
            LoggingPriority.Information => ConsoleColor.Gray,
            LoggingPriority.GeneralInformation => ConsoleColor.DarkCyan,
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
        };
    }
}