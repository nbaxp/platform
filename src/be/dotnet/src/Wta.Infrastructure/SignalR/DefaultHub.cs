using Microsoft.AspNetCore.SignalR;
using Wta.Infrastructure.Application.Events;

namespace Wta.Infrastructure.SignalR;

public class DefaultHub(ILogger<DefaultHub> logger, IEventPublisher eventPublisher) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation($"{Context.ConnectionId} Connected");
        await Login().ConfigureAwait(false);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"{Context.ConnectionId} Disconnected");
        await Logout().ConfigureAwait(false);
    }

    public async Task Login()
    {
        var httpContext = Context.GetHttpContext();
        var userName = httpContext?.User.Identity?.Name;
        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userName).ConfigureAwait(false);
        }
    }

    public async Task Logout()
    {
        var httpContext = Context.GetHttpContext();
        var userName = httpContext?.User.Identity?.Name;
        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName).ConfigureAwait(false);
        }
    }

    public async Task ClientToServer(string command, string data, string? from = null, string? to = null)
    {
        await eventPublisher.Publish(new SignalRClientToServerEvent
        {
            Command = command,
            Data = data,
            To = to,
            From = from
        }).ConfigureAwait(false);
    }
}
