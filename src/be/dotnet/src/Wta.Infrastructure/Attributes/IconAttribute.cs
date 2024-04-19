namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IconAttribute(string icon) : Attribute
{
    public string Icon { get; } = icon;
}
