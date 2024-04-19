namespace Wta.Infrastructure.Web;

public class DisplayModelMetadata(IStringLocalizer stringLocalizer, IModelMetadataProvider provider, ICompositeMetadataDetailsProvider detailsProvider, DefaultMetadataDetails details, DefaultModelBindingMessageProvider modelBindingMessageProvider) : DefaultModelMetadata(provider, detailsProvider, details, modelBindingMessageProvider)
{
    public override string? DisplayName
    {
        get
        {
            var name = base.DisplayName;
            if (string.IsNullOrEmpty(name))
            {
                name = ContainerType == null ? ModelType?.Name : PropertyName;
            }
            return stringLocalizer.GetString(name!);
        }
    }
}
