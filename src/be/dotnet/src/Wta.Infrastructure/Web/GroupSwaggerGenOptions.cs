using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wta.Infrastructure.Web;

public class GroupSwaggerGenOptions(IApiDescriptionGroupCollectionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiDescriptionGroups.Items)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = description.GroupName });
        }
    }
}

