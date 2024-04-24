namespace Wta.Infrastructure.Application.Models;

public enum OperatorType
{
    [Expression("{0} = @0")]
    Equal,

    [Expression("{0} != @0")]
    NotEqual,

    [Expression("{0} > @0")]
    GreaterThan,

    [Expression("{0} >= @0")]
    GreaterThanOrEqual,

    [Expression("{0} < @0")]
    LessThan,

    [Expression("{0} <= @0")]
    LessThanOrEqual,

    [Expression("{0}.Contains(@0)")]
    Contains,

    [Expression("{0}.StartsWith(@0)")]
    StartsWith,

    [Expression("{0}.EndsWith(@0)")]
    EndsWith,

    Ignore
}
