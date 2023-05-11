using Discord.WebSocket;
using DiscordBot_tutorial.Services.SettingsService;
using Quartz;

namespace DiscordBot_tutorial.Jobs;

public class Job2137 : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var client = (DiscordSocketClient)dataMap["_client"];
        var settings = (SettingsService)dataMap["_settings"];

        var flag = false;
        
        if(DateTime.Now.ToShortTimeString() == "21:37" && !flag)
        {
            var channel = client.GetGuild(settings.Settings.GuildId)
                .GetTextChannel(1105183278266331156);
     
            await channel.SendFileAsync("/Users/toczekmj/RiderProjects/DiscordBot/mozna.mp4",
                "Jak najbardziej");
            await channel.SendMessageAsync("Jeszcze jak");
            
            flag = true;
        }

        if (DateTime.Now.ToShortTimeString() == "21:38" && flag)
            flag = false;
    }
}