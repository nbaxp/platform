namespace Wta.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddHostedServiceFromServiceProvider<T>(WebApplicationBuilder builder) where T : class, IHostedService
    {
        builder.Services.AddHostedService(o => o.GetRequiredService<T>());
    }
}
