namespace Wta.Infrastructure.Web;

public class TrimJsonConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString()?.Trim();
            if (!string.IsNullOrEmpty(stringValue))
            {
                return stringValue;
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (!string.IsNullOrEmpty(value))
        {
            writer.WriteStringValue(value?.Trim());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
