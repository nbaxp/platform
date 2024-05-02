using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using Wta.Infrastructure.Monitoring;
using Monitor = Wta.Infrastructure.Monitoring.Monitor;

namespace Wta.Application.Default.Controllers;

public class MonitorController(JsonSerializerOptions jsonSerializerOptions) : BaseController, IResourceService<Monitor>
{
    [AllowAnonymous]
    [HttpGet]
    public async Task Index()
    {
        Response.ContentType = "text/event-stream";
        var process = Process.GetCurrentProcess();
        while (!this.HttpContext.RequestAborted.IsCancellationRequested)
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(o => o.AddressFamily == AddressFamily.InterNetwork)
            .Select(o => o.ToString())
            .ToArray();
            var memoryTotal = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            var drive = DriveInfo.GetDrives().FirstOrDefault(o => o.RootDirectory.FullName == Directory.GetDirectoryRoot(Path.GetPathRoot(Environment.ProcessPath!)!))!;
            var line = new Monitor
            {
                OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                OSDescription = RuntimeInformation.OSDescription,
                UserName = Environment.UserName,
                ServerTime = DateTime.UtcNow,
                HostName = Dns.GetHostName(),
                HostAddresses = string.Join(',', addresses),
                RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
                ProcessCount = Process.GetProcesses().Length,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                ProcessName = process.ProcessName,
                ProcessArguments = string.Join(' ', Environment.GetCommandLineArgs()),
                ProcessId = process.Id,
                ProcessHandleCount = process.HandleCount,
                ProcessFileName = Environment.ProcessPath,
                DriveName = drive.Name,
                DrivieTotalSize = drive.TotalSize,
                DriveAvailableFreeSpace = drive.AvailableFreeSpace,
                CpuCount = Environment.ProcessorCount,
                MemoryTotal = memoryTotal,
                CpuUsage = EventListenerHostedService.Data.GetValueOrDefault("cpu-usage"),
                MemoryUsage = EventListenerHostedService.Data.GetValueOrDefault("working-set") * 1024 * 1024 / memoryTotal,
                Framework = RuntimeInformation.FrameworkDescription,
                ExceptionCount = (int)EventListenerHostedService.Data.GetValueOrDefault("exception-count"),
                TotalRequests = (int)EventListenerHostedService.Data.GetValueOrDefault("total-requests"),
                CurrentRequests = (int)EventListenerHostedService.Data.GetValueOrDefault("current-requests"),
                BytesReceived = (int)EventListenerHostedService.Data.GetValueOrDefault("bytes-received"),
                BytesSent = (int)EventListenerHostedService.Data.GetValueOrDefault("bytes-sent"),
            };
            var message = "data:" + JsonSerializer.Serialize(line, jsonSerializerOptions) + "\n\n";
            await Response.Body.WriteAsync(Encoding.UTF8.GetBytes(message)).ConfigureAwait(false);
            await Response.Body.FlushAsync().ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);
        }
    }
}
