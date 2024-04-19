namespace Wta.Infrastructure.Event;

[Service<IEventPublisher>(ServiceLifetime.Singleton)]
public class LocalEventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    public async Task Publish<T>(T data)
    {
        var subscribers = serviceProvider.GetServices<IEventHander<T>>().ToList();
        foreach (var item in subscribers)
        {
            await item.Handle(data).ConfigureAwait(false);
        }
    }
}
