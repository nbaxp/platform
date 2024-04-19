namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute<T> : Attribute
{
}
