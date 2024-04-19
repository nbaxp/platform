using Microsoft.AspNetCore.Mvc;

namespace Wta.Infrastructure.Web;

public class DisplayModelMetaDataProvider : DefaultModelMetadataProvider
{
    private readonly IStringLocalizer _stringLocalizer;

    public DisplayModelMetaDataProvider(IStringLocalizer stringLocalizer, ICompositeMetadataDetailsProvider detailsProvider, IOptions<MvcOptions> optionsAccessor) : base(detailsProvider, optionsAccessor)
    {
        _stringLocalizer = stringLocalizer;
    }

    protected override ModelMetadata CreateModelMetadata(DefaultMetadataDetails entry)
    {
        return new DisplayModelMetadata(_stringLocalizer, this, DetailsProvider, entry, ModelBindingMessageProvider);
    }
}
