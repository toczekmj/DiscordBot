using Discord;
using DiscordBot_tutorial.Services.LoggingService;

namespace DiscordBot_tutorial.Interfaces;

public interface ILoggingService
{
    public Task LogAsync(LogMessage message);
    public void LogLocal(string message, LoggingPriority priority);
}