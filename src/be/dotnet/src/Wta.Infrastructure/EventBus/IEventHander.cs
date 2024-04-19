namespace Wta.Infrastructure.Event;
public interface IEventHander<T>
{
    Task Handle(T data);
}

public interface IEventPublisher
{
    Task Publish<T>(T data);
}

public abstract class BaseEvent
{
    public DateTime CreationDate { get; } = DateTime.UtcNow;
}

public abstract class EntityEvent<T>(T data) : BaseEvent
{
    public T Data { get; } = data;
}

public class EntityCreatedEvent<T>(T data) : EntityEvent<T>(data)
{
}

public class EntityUpdatedEvent<T>(T data) : EntityEvent<T>(data)
{
}

public class EntityDeletedEvent<T>(T data) : EntityEvent<T>(data)
{
}
