namespace Wta.Infrastructure.Extensions;

public static class AppDomainExtensions
{
    private static readonly object lockObj = new object();
    private static Assembly[]? Assemblies = default!;

    public static Assembly[] GetCustomerAssemblies(this AppDomain appDomain)
    {
        if (Assemblies == null)
        {
            lock (lockObj)
            {
                Assemblies ??= appDomain.GetAssemblies().Where(Exclude).Where(Include).ToArray();
            }
        }
        return Assemblies!;
    }

    public static Func<Assembly, bool> Exclude = o => !o.FullName!.StartsWith("Microsoft.") && !o.FullName!.StartsWith("System.");
    public static Func<Assembly, bool> Include = o => o.FullName!.StartsWith("Wta");
}
