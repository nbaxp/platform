namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ViewAttribute(string component) : Attribute
{
    public string Component { get; } = component;
}
