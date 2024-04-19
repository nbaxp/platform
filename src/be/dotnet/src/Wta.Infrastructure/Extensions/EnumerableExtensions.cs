namespace Wta.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// https://stackoverflow.com/questions/200574/linq-equivalent-of-foreach-for-ienumerablet
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var item in values)
        {
            action(item);
        }
    }
}
