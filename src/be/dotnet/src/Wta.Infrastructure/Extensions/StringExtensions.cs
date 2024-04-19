using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Wta.Infrastructure.Extensions;

public static partial class StringExtensions
{
    public static Guid ToGuid(this string input)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }

    public static string TrimStart(this string input, string start)
    {
        return input.StartsWith(start) ? input[start.Length..] : input;
    }

    public static string TrimEnd(this string input, string end)
    {
        return input.EndsWith(end) ? input[..^end.Length] : input;
    }

    public static string ToMd5(this string input)
    {
        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input)));
    }

    public static string? ToSlugify(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        return SlugifyRegex().Replace(value, "$1-$2").ToLower();
    }

    public static string ToLowerCamelCase(this string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        if (!char.IsUpper(name[0]))
        {
            return name;
        }

        var stringBuilder = new StringBuilder();

        for (var index = 0; index < name.Length; index++)
        {
            if (index != 0 && index + 1 < name.Length && !char.IsUpper(name[index + 1]))
            {
                stringBuilder.Append(name.AsSpan(index));
                break;
            }
            else
            {
                stringBuilder.Append(char.ToLowerInvariant(name[index]));
            }
        }

        return stringBuilder.ToString();
    }

    public static object? GetValue(this string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        if (type.IsValueType)
        {
            //值类型
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];//取原始类型
            }
            if (type.IsEnum)
            {
                //枚举
                return Enum.GetNames(type)
                    .Select(o => new KeyValuePair<string, Enum>(o, (Enum)Enum.Parse(type, o)))
                    .Where(o => o.Value.ToString() == value)
                    .Select(o => o.Value)
                    .FirstOrDefault();
            }
            else if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }
        }
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex SlugifyRegex();
}
