using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Scheduling;

[System, Display(Name = "定时任务", Order = 1000)]
public class Job : Entity
{
    public string Name { get; set; } = default!;
    public string Cron { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool Disabled { get; set; }
}
