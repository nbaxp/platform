using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public ButtonType Type { get; set; }
}
