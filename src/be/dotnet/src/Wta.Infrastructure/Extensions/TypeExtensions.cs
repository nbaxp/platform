using System.Runtime.CompilerServices;

namespace Wta.Infrastructure.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// 获取类型名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    /// <summary>
    /// 获取方法签名
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    public static string GetMethodName(this MethodInfo methodInfo)
    {
        var result = $"{methodInfo.Name}";
        if (methodInfo.IsGenericMethod)
        {
            result += $"<{string.Join(',', methodInfo.GetGenericArguments().Select(o => o.GetTypeName()))}>";
        }
        result += $"({string.Join(',', methodInfo.GetParameters().Select(o => o.ParameterType.GetTypeName()))})";
        return result;
    }

    /// <summary>
    /// 根据方法签名调用方法
    /// eg:typeof(EntityFrameworkServiceCollectionExtensions).MethodInvoke(
    /// "AddDbContext<TContext>(IServiceCollection,Action<DbContextOptionsBuilder>,ServiceLifetime,ServiceLifetime)",
    ///  [builder.Services, action, ServiceLifetime.Scoped, ServiceLifetime.Scoped],
    ///  [dbContextType]);
    /// </summary>
    public static object? MethodInvoke(this Type type, string method, object?[]? parameters = null, Type[]? genericTypeArguments = null, object? instance = null)
    {
        var methodInfo = type.GetMethods().First(o => o.GetMethodName() == method);
        if (methodInfo.IsGenericMethod)
        {
            methodInfo = methodInfo.MakeGenericMethod(genericTypeArguments ?? methodInfo.GetGenericArguments());
        }
        return methodInfo?.Invoke(instance, parameters);
    }

    /// <summary>
    /// 获取全部基类
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type[] GetBaseClasses(this Type type)
    {
        var classes = new List<Type>();
        var current = type;
        while (current.BaseType != null && current.BaseType.IsClass && current.BaseType != typeof(object))
        {
            classes.Add(current.BaseType);
            current = current.BaseType;
        }
        return classes.ToArray();
    }

    /// <summary>
    /// 是否可空值类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullableType(this Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// 原始类型（如果是可空值类型，返回Nullable的泛型参数类型）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetUnderlyingType(this Type type)
    {
        return type.IsNullableType() ? Nullable.GetUnderlyingType(type)! : type;
    }

    /// <summary>
    /// 是否具有特性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static bool HasAttribute<T>(this Type type, bool inherit = true) where T : Attribute
    {
        return type.GetCustomAttributes<T>(inherit).Any();
    }

    /// <summary>
    /// 返回DisplayAttribute.Name
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Type type)
    {
        var scope = WtaApplication.Application.Services.CreateScope();
        var localizer = scope?.ServiceProvider.GetService<IStringLocalizer>();
        var key = type.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.Name;
        return localizer?.GetString(key, type.FullName!) ?? key;
    }

    public static void InvokeExtensionMethod(this Type type, string name, Type[]? typeArguments, Type[] parameterTypes, params object[] parameters)
    {
        parameterTypes ??= parameters.Select(o => o.GetType()).Skip(1).ToArray();
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(o => o.IsDefined(typeof(ExtensionAttribute)) && o.Name == name && o.GetParameters().Select(o => o.ParameterType).Skip(1).SequenceEqual(parameterTypes));
        if (method != null && typeArguments != null)
        {
            method = method.MakeGenericMethod(typeArguments);
        }
        method?.Invoke(null, parameters);
    }

    public static object GetMetadataForType(this Type modelType)
    {
        using var scope = WtaApplication.Application.Services.CreateScope();
        var meta = scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(modelType);
        return meta.GetSchema(scope.ServiceProvider);
    }
}
