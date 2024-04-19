namespace Wta.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ExpressionAttribute : Attribute
{
    public ExpressionAttribute(string expression)
    {
        Expression = expression;
    }

    public string Expression { get; }
}
