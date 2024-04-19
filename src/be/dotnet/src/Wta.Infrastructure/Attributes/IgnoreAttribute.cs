namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public class IgnoreAttribute : Attribute
{
}
