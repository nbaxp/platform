using Wta.Infrastructure.Application.Domain;
using Wta.Infrastructure.Startup;

namespace Wta.Infrastructure;

public static class WtaApplication
{
    public static WebApplication Application { get; private set; } = default!;

    public static WebApplicationBuilder Builder { get; private set; } = default!;
    public static Dictionary<Type, Type> EntityModel { get; } = [];
    public static Dictionary<Type, List<Type>> ModuleDbContexts { get; } = [];

    public static void Initialize()
    {
        //加载实体和数据上下文关系
        ////获取配置类
        AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => !o.IsAbstract && o.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ForEach(item =>
            {
                var dbContextType = item.GetBaseClasses().First(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(BaseDbConfig<>)).GenericTypeArguments.First();
                var moduleType = dbContextType.GetCustomAttribute(typeof(DependsOnAttribute<>))!.GetType().GenericTypeArguments.First();
                if (moduleType.IsAssignableTo(typeof(IStartup)))
                {
                    if (ModuleDbContexts.TryGetValue(moduleType, out var dbContextTypeList))
                    {
                        dbContextTypeList.Add(dbContextType);
                    }
                    else
                    {
                        ModuleDbContexts.Add(moduleType, [dbContextType]);
                    }
                }
            });
        // 缓存实体和模型关系
        AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => o.IsClass &&
            !o.IsAbstract &&
            o.GetCustomAttributes(typeof(DependsOnAttribute<>)).Any(o => o.GetType().GenericTypeArguments.First().IsAssignableTo(typeof(IResource))))
            .ForEach(item =>
            {
                var entityType = item.GetCustomAttribute(typeof(DependsOnAttribute<>))?.GetType().GenericTypeArguments.First()!;
                EntityModel.Add(entityType, item);
            });
    }

    public static void Run<T>(string[] args)
        where T : IStartup
    {
        typeof(T).GetCustomAttributes().ForEach(o =>
        {
            Console.WriteLine(o.GetType().Assembly.FullName);
        });
        Initialize();
        var startup = Activator.CreateInstance<T>()!;
        var modules = ModuleDbContexts.Keys.Select(o => Activator.CreateInstance(o) as IStartup);
        Console.WriteLine("Configuring web host...");
        Builder = WebApplication.CreateBuilder(args);
        startup.ConfigureServices(Builder);
        modules.ForEach(o => o!.ConfigureServices(Builder));
        Application = Builder.Build();
        startup.Configure(Application);
        modules.ForEach(o => o!.Configure(Application));
        Console.WriteLine("Starting web host...");
        try
        {
            Application.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Program terminated unexpectedly!");
        }
        finally
        {
            Console.WriteLine("Program exted!");
        }
    }
}
