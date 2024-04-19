using Wta.Infrastructure.Application.Models;

namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class OperatorTypeAttribute : Attribute
{
    public OperatorTypeAttribute(OperatorType operatorType, string? propertyName = null)
    {
        OperatorType = operatorType;
        PropertyName = propertyName;
    }

    public OperatorType OperatorType { get; }
    public string? PropertyName { get; }
}
