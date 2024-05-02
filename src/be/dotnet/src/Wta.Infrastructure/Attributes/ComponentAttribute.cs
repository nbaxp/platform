namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute(string component):Attribute
{
    public string Component { get; set; } = component;
}
