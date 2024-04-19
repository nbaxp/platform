using Microsoft.AspNetCore.SignalR;

namespace Wta.Infrastructure.Extensions;
public static class ClientProxyExtensions
{
    public static void ServerToClient(this IClientProxy clientProxy, string method, string message, string toClient, string? fromClient = null)
    {
        clientProxy.SendAsync(nameof(ServerToClient), method, message, toClient, fromClient);
    }
}
