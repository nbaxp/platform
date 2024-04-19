using Wta.Infrastructure.SequentialGuid;

namespace Wta.Infrastructure.Extensions;

public static class DbContextExtensions
{
    public static Guid NewGuid(this DbContext dbContext)
    {
        var type = "SequentialAsString";
        var providerName = dbContext.Database.ProviderName!.ToLowerInvariant();
        if (providerName == "sqlserver")
        {
            type = "SequentialAtEnd";
        }
        else if (providerName == "oracle")
        {
            type = "SequentialAsBinary";
        }
        var sequentialGuid = WtaApplication.Application.Services.GetRequiredService<ISequentialGuid>();
        return sequentialGuid.Create(type);
    }
}
