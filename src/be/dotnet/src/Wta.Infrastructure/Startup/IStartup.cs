namespace Wta.Infrastructure.Startup;

public interface IStartup
{
    void Configure(WebApplication webApplication);

    void ConfigureServices(WebApplicationBuilder builder);
}
