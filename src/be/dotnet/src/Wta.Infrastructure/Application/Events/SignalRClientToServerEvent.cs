namespace Wta.Infrastructure.Application.Events;

public class SignalRClientToServerEvent
{
    public string Command { get; set; } = default!;
    public string? Data { get; set; }
    public string? To { get; set; }
    public string? From { get; set; }
}
