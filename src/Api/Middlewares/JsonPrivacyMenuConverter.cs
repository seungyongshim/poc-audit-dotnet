using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Middlewares;

public class JsonPrivacyMenuConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value ? "true" : "false");
}
