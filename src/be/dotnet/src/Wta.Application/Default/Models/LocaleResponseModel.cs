namespace Wta.Application.Default.Models;

public class LocaleResponseModel
{
    public string Locale { get; set; } = default!;
    public Dictionary<string, object> Messages { get; set; } = new Dictionary<string, object>();
    public List<KeyValuePair<string, string>> Options { get; set; } = new List<KeyValuePair<string, string>>();
}
