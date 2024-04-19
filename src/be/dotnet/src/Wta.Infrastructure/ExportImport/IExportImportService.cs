namespace Wta.Infrastructure.ImportExport;

public interface IExportImportService
{
    byte[] Export<TModel>(List<TModel> list);

    byte[] GetImportTemplate<TImportModel>();

    IList<TImportModel> Import<TImportModel>(byte[] bytes);
}
