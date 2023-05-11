using DiscordBot_tutorial.Interfaces;
using DiscordBot_tutorial.Quartz.Jobs;
using Quartz;
using Quartz.Impl;

namespace DiscordBot_tutorial.Quartz.Schedulers;

public class SchedulerManager 
{
    private readonly IAsyncDisposable _client;
    private readonly ISettingsService _settingsService;
    private readonly StdSchedulerFactory _factory;

    public SchedulerManager(IAsyncDisposable client, ISettingsService settingsService)
    {
        _client = client;
        _settingsService = settingsService;
        _factory = new StdSchedulerFactory();
    }
    
    public async Task ConfigureJob2137()
    {
        var scheduler = await _factory.GetScheduler();
        await scheduler.Start();
            
        var job = JobBuilder.Create<Job2137>()
            .WithIdentity("job2137")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("trigger2137")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(30)
                .RepeatForever()
            )
            .Build();
            
        job.JobDataMap.Put("_client", _client);
        job.JobDataMap.Put("_settings", _settingsService);
        
        await scheduler.ScheduleJob(job, trigger);
    }
    
}