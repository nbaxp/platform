namespace Wta.Infrastructure.Scheduling;

public class JobModel
{
    public Guid Id { get; set; }
    public string Cron { get; set; } = default!;
    public IScheduledTask? Task { get; set; }
    public Thread? Thread { get; set; }
}
