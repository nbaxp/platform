namespace Wta.Infrastructure.Web;
public class TrimModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(string) && context.BindingInfo.BindingSource != BindingSource.Body)
        {
            return new TrimModelBinder();
        }

        return null;
    }
}
