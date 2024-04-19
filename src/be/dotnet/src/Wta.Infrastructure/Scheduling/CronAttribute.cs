namespace Wta.Infrastructure.Scheduling;

[AttributeUsage(AttributeTargets.Class)]
public class CronAttribute(string cron) : Attribute
{
    public string Cron = cron;
}
