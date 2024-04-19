using OrchardCore.Localization;

namespace Wta.Infrastructure.Web;

public class LocalizationFileLocationProvider(IFileProvider fileProvider, IOptions<LocalizationOptions> localizationOptions) : ILocalizationFileLocationProvider
{
    private readonly string _subpath = localizationOptions.Value.ResourcesPath;

    public IEnumerable<IFileInfo> GetLocations(string cultureName)
    {
        var suffix = $"{cultureName}.po";
        var fileInfo = fileProvider.GetFileInfo(Path.Combine(_subpath, suffix));
        if (fileInfo.Exists)
        {
            yield return fileInfo;
        }
    }
}
