namespace Wta.Infrastructure.Extensions;

public static class CustomAttributeProviderExtensions
{
    public static T? GetAttribute<T>(this ICustomAttributeProvider customAttributeProvider) where T : Attribute
    {
        return customAttributeProvider.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    }
}
