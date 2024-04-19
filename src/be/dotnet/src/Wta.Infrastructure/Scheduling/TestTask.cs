namespace Wta.Infrastructure.Scheduling;

[Cron("0,30 * * * * ? ")]
[Display(Name = "测试任务")]
[Service<IScheduledTask>]
public class TestTask : IScheduledTask
{
    public Task ExecuteAsync()
    {
        Console.WriteLine($"正在执行：{this.GetType().FullName}");
        return Task.CompletedTask;
    }
}
