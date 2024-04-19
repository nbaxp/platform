using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Wta.Infrastructure.Web;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) { return null; }
        var str = value.ToString();
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        return Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
