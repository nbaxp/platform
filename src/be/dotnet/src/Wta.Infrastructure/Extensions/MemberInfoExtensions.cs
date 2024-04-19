using Swashbuckle.AspNetCore.Annotations;

namespace Wta.Infrastructure.Extensions;

public static class MemberInfoExtensions
{
    public static string GetDisplayName(this MemberInfo memberInfo)
    {
        var scope = WtaApplication.Application.Services.CreateScope();
        var localizer = scope?.ServiceProvider.GetService<IStringLocalizer>();
        var key = memberInfo.GetCustomAttribute<SwaggerOperationAttribute>()?.Summary ?? memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? memberInfo.Name;
        return localizer?.GetString(key, $"{memberInfo.ReflectedType!.Name}.{key}") ?? key;
    }
}
