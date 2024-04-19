using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace Wta.Infrastructure.Monitoring;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
/// </summary>
[Service<IHostedService>]
public class EventListenerHostedService : EventListener, IHostedService
{
    private static readonly Dictionary<string, string[]> _eventSources = new()
    {
        ["System.Runtime"] = ["cpu-usage", "working-set", "exception-count"],
        ["Microsoft.AspNetCore.Hosting"] = ["current-requests", "failed-requests", "requests-per-second", "total-requests"],
        ["Microsoft.AspNetCore.Http.Connections"] = ["connections-duration", "current-connections", "connections-started", "connections-stopped", "connections-timed-out"],
        ["System.Net.Sockets"] = ["bytes-received", "bytes-sent"],
    };

    public static ConcurrentDictionary<string, double> Data { get; } = [];

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (_eventSources.ContainsKey(eventSource.Name))
        {
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string?> { ["EventCounterIntervalSec"] = "1" });
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName == "EventCounters" &&
            eventData?.Payload?.First() is IDictionary<string, object> data &&
            data.TryGetValue("CounterType", out var counterType))
        {
            var name = data["Name"]?.ToString()!;
            if (_eventSources.TryGetValue(eventData.EventSource.Name, out var keys) && keys.Contains(name))
            {
                var type = counterType.ToString();
                var key = type == "Sum" ? "Increment" : "Mean";
                if (data.TryGetValue(key, out var value))
                {
                    Data.AddOrUpdate(name, key => (double)value, (key, oldValue) => (double)value);
                }
            }
        }
    }
}
