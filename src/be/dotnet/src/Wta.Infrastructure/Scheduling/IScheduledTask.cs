namespace Wta.Infrastructure.Scheduling;

public interface IScheduledTask
{
    Task ExecuteAsync();
}
