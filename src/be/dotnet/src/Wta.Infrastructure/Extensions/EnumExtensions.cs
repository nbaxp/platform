namespace Wta.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
    {
        return enumValue.GetType().GetMember(enumValue.ToString()).First()
            .GetCustomAttributes<T>(inherit: false)
            .FirstOrDefault();
    }

    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var field = type.GetField(enumValue.ToString())!;
        var scope = WtaApplication.Application.Services.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer>();
        var key = field.GetCustomAttribute<DisplayAttribute>()?.Name ?? field.Name;
        return localizer.GetString(key);
    }
}
