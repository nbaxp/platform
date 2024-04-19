namespace Wta.Infrastructure.Attributes;

public interface IImplementAttribute
{
    ServiceLifetime Lifetime { get; }
    Type ServiceType { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute<T>(ServiceLifetime lifetime = ServiceLifetime.Transient) : Attribute, IImplementAttribute
{
    public Type ServiceType { get; } = typeof(T);
    public ServiceLifetime Lifetime { get; } = lifetime;
}
