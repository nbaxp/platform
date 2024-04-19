using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Monitoring;

[System, Display(Name = "监控", Order = 0)]
public class Monitor : IResource
{
    public Monitor()
    {
        var addresses = Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(o => o.AddressFamily == AddressFamily.InterNetwork)
                    .Select(o => o.ToString())
                    .ToArray();
        var drive = DriveInfo.GetDrives().FirstOrDefault(o => o.RootDirectory.FullName == Directory.GetDirectoryRoot(Path.GetPathRoot(Environment.ProcessPath!)!))!;
        var process = Process.GetCurrentProcess();
        var memoryTotal = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        OSArchitecture = RuntimeInformation.OSArchitecture.ToString();
        OSDescription = RuntimeInformation.OSDescription;
        UserName = Environment.UserName;
        ServerTime = DateTime.UtcNow;
        HostName = Dns.GetHostName();
        HostAddresses = string.Join(',', addresses);
        ProcessCount = Process.GetProcesses().Length;
        ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
        ProcessName = process.ProcessName;
        ProcessArguments = string.Join(' ', Environment.GetCommandLineArgs());
        ProcessId = process.Id;
        ProcessHandleCount = process.HandleCount;
        ProcessFileName = Environment.ProcessPath;
        DriveName = drive.Name;
        DrivieTotalSize = drive.TotalSize;
        DriveAvailableFreeSpace = drive.AvailableFreeSpace;
        CpuCount = Environment.ProcessorCount;
        MemoryTotal = memoryTotal;
        MemoryUsage = Environment.WorkingSet / memoryTotal;
        //CpuUsage = this.GetCpuUsage();
        //DiskRead = data.Disk?.Read;
        //DiskWrite = data.Disk?.Write;
        //NetworkRead = data.Networking?.Read;
        //NetworkWrite = data.Networking?.Write;
        Framework = RuntimeInformation.FrameworkDescription;
    }

    /// <summary>
    /// 操作系统架构
    /// </summary>
    public string OSArchitecture { get; set; } = default!;

    /// <summary>
    /// 操作系统描述
    /// </summary>
    public string OSDescription { get; set; } = default!;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// 服务器时间
    /// </summary>
    public DateTime ServerTime { get; set; }

    /// <summary>
    /// 主机名称
    /// </summary>
    public string HostName { get; set; } = default!;

    /// <summary>
    /// 主机地址
    /// </summary>
    public string HostAddresses { get; set; } = default!;

    public string RuntimeIdentifier { get; set; } = default!;

    /// <summary>
    /// 进程数量
    /// </summary>
    public int ProcessCount { get; set; }

    /// <summary>
    /// 进程架构
    /// </summary>
    public string ProcessArchitecture { get; set; } = default!;

    /// <summary>
    /// 进程名称
    /// </summary>
    public string ProcessName { get; set; } = default!;

    /// <summary>
    /// 进程参数
    /// </summary>
    public string ProcessArguments { get; set; } = default!;

    /// <summary>
    /// 进程文件
    /// </summary>
    public string? ProcessFileName { get; set; }

    /// <summary>
    /// 磁盘名称
    /// </summary>
    public string DriveName { get; set; } = default!;

    /// <summary>
    /// 驱动容量
    /// </summary>
    public long DrivieTotalSize { get; set; }

    /// <summary>
    /// 驱动剩余空间
    /// </summary>
    public long DriveAvailableFreeSpace { get; set; }

    /// <summary>
    /// 进程 Id
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// 进程句柄数量
    /// </summary>
    public int ProcessHandleCount { get; set; }

    /// <summary>
    /// CPU 数量
    /// </summary>
    public int CpuCount { get; set; }

    /// <summary>
    ///  内存容量
    /// </summary>
    public long? MemoryTotal { get; set; }

    /// <summary>
    /// CPU 使用率
    /// </summary>
    public double? CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率
    /// </summary>
    public double? MemoryUsage { get; set; }

    /// <summary>
    /// 网络发送速度
    /// </summary>
    public ulong? NetworkWrite { get; set; }

    /// <summary>
    /// Framework
    /// </summary>
    public string Framework { get; set; } = default!;

    /// <summary>
    /// 已发生的异常数
    /// </summary>
    public int ExceptionCount { get; set; }

    public int TotalRequests { get; set; }
    public int CurrentRequests { get; set; }
    public int BytesReceived { get; set; }
    public int BytesSent { get; set; }
}
