//using System.Collections.Concurrent;
//using NCrontab;

//namespace Wta.Infrastructure.Scheduling;

//[Service<IHostedService>]
//public class SchedulerHostedService(IServiceProvider serviceProvider) : IHostedService
//{
//    public static ConcurrentDictionary<Guid, JobModel> Tasks { get; private set; } = new();

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        using var scope = serviceProvider.CreateScope();
//        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Job>>();
//        repository.Query().ForEach(this.AddTask);
//        return Task.CompletedTask;
//    }

//    private void AddTask(Guid Id, string Cron job)
//    {
//        if (!Tasks.Any(o => o.Key == job.Id))
//        {
//            var model = new JobModel { Id };
//            Tasks.TryAdd(job.Id, model);
//            model.Thread?.Start();
//        }
//    }

//    private static JobModel CreateThread(Job job)
//    {
//        var crontabSchedule = CrontabSchedule.Parse(job.Cron, new CrontabSchedule.ParseOptions() { IncludingSeconds = true });
//        if (crontabSchedule != null)
//        {
//        }
//        new Thread(async () =>
//        {
//        });
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }
//}
