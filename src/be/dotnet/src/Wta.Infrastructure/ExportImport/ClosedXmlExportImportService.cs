using ClosedXML.Excel;
using ClosedXML.Graphics;

namespace Wta.Infrastructure.ImportExport;

[Service<IExportImportService>]
public class ClosedXMLExportImportService() : IExportImportService
{
    static ClosedXMLExportImportService()
    {
        using var fallbackFontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wta.Infrastructure.Resources.carlito.ttf");
        LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateOnlyWithFonts(fallbackFontStream);
    }

    public byte[] Export<TModel>(List<TModel> list)
    {
        var type = typeof(TModel);
        using var workbook = new XLWorkbook();
        var name = type.GetDisplayName();
        var ws = workbook.Worksheets.Add(name);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToList();
        var rowIndex = 1;
        SetHeader(ws, properties, rowIndex);
        foreach (var item in list)
        {
            rowIndex++;
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var columnIndex = i + 1;
                var cell = ws.Cell(rowIndex, columnIndex);
                SetCell(item, cell, property);
            }
        }
        SetStyle(ws);
        return GetResult(workbook);
    }

    public byte[] GetImportTemplate<TModel>()
    {
        var type = typeof(TModel);
        using var workbook = new XLWorkbook();
        var name = type.GetDisplayName();
        var ws = workbook.Worksheets.Add(name);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToList();
        var rowIndex = 1;
        SetHeader(ws, properties, rowIndex);
        SetStyle(ws);
        return GetResult(workbook);
    }

    public IList<TModel> Import<TModel>(byte[] bytes)
    {
        var type = typeof(TModel);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToList();
        var result = new List<TModel>();
        using var ms = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(ms);
        var ws = workbook.Worksheets.FirstOrDefault();
        if (ws != null)
        {
            for (var i = 1; i < ws.RowsUsed().Count(); i++)
            {
                var rowIndex = i + 1;
                var row = ws.Row(rowIndex);
                var model = Activator.CreateInstance<TModel>();
                result.Add(model);
                for (var j = 0; j < ws.ColumnsUsed().Count(); j++)
                {
                    var columnIndex = j + 1;
                    var cell = row.Cell(columnIndex);
                    var value = cell.Value.ToString().Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        var headerName = ws.Cell(0, columnIndex).Value.ToString().Trim();
                        var property = properties.FirstOrDefault(o => o.GetDisplayName() == headerName);
                        if (property != null)
                        {
                            SetModelProperty(model, value, property!);
                        }
                    }
                }
            }
        }
        return result;
    }

    private static void SetModelProperty<TModel>(TModel? model, string value, PropertyInfo property)
    {
        var propertyType = property.PropertyType.GetUnderlyingType();
        if (propertyType.IsEnum)
        {
            var enumValue = Enum.GetNames(propertyType)
                 .Select(o => new KeyValuePair<string, Enum>(o, (Enum)Enum.Parse(propertyType, o)))
                 .Where(o => o.Value.GetDisplayName() == value.ToString())
                 .Select(o => o.Value)
                 .FirstOrDefault();
            property.SetValue(model, enumValue);
        }
        else if (propertyType.Name == nameof(Boolean))
        {
            if (value == "是")
            {
                property.SetValue(model, true);
            }
            else
            {
                property.SetValue(model, false);
            }
        }
        else if (propertyType == typeof(Guid))
        {
            property.SetValue(model, Guid.Parse(value));
        }
        else
        {
            var propertyValue = Convert.ChangeType(value.ToString(), propertyType, CultureInfo.InvariantCulture);
            property.SetValue(model, propertyValue);
        }
    }

    private static void SetHeader(IXLWorksheet ws, List<PropertyInfo> properties, int rowIndex)
    {
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var columnIndex = i + 1;
            var cell = ws.Cell(rowIndex, columnIndex);
            cell.Value = property.GetDisplayName();
        }
    }

    private static void SetCell<TExportModel>(TExportModel? model, IXLCell cell, PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var value = property.GetValue(model)?.ToString()?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        if (propertyType == typeof(bool))
        {
            cell.Value = (bool)property.GetValue(model)! ? "是" : "否";
        }
        else if (propertyType.IsEnum)
        {
            cell.Value = (Enum.Parse(propertyType, value) as Enum)?.GetDisplayName();
        }
        else if (propertyType == typeof(DateTime))
        {
            cell.Value = ((DateTime)property.GetValue(model)!).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        else
        {
            cell.Value = value;
        }
    }

    private static void SetStyle(IXLWorksheet ws)
    {
        ws.RangeUsed().Style.Border.TopBorder =
        ws.RangeUsed().Style.Border.RightBorder =
        ws.RangeUsed().Style.Border.BottomBorder =
        ws.RangeUsed().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        ws.RangeUsed().Style.Border.TopBorderColor =
        ws.RangeUsed().Style.Border.RightBorderColor =
        ws.RangeUsed().Style.Border.BottomBorderColor =
        ws.RangeUsed().Style.Border.LeftBorderColor = XLColor.Black;
        //ws.RangeUsed().SetAutoFilter();
        ws.ColumnsUsed().AdjustToContents();
        //ws.RowsUsed().AdjustToContents();
    }

    private static byte[] GetResult(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }
}
