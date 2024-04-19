using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wta.Infrastructure.Web;

public class LanguageSwaggerFilter(IOptions<RequestLocalizationOptions> options) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }
        var list = options.Value.SupportedUICultures?.Select(o => new OpenApiString(o.Name)).ToList<IOpenApiAny>()!;
        var defaultValue = list.FirstOrDefault(o => (o as OpenApiString)!.Value == options.Value.DefaultRequestCulture.Culture.Name);
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = list,
                Default = defaultValue,
            }
        });
    }
}
