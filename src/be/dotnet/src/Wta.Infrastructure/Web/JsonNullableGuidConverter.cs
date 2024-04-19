namespace Wta.Infrastructure.Web;

public class JsonNullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString()?.Trim();
            if (!string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out var value))
            {
                return value;
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value?.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
