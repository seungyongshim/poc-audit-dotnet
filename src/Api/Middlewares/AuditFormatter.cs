using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog.Core;
using Serilog.Events;

namespace Api.Middlewares;

public class AuditFormatter : IDestructuringPolicy
{
    public object? GetFormat(Type? formatType) => this;

    public JsonSerializerOptions options = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (arg is Audit audit)
        {
            return JsonSerializer.Serialize(audit, options);
        }
        return arg?.ToString() ?? string.Empty;
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        if (value is Audit dict)
        {
            var json = JsonSerializer.Serialize(dict, options);
            result = new ScalarValue(json);
            return true;
        }

        result = null;
        return false;
    }
}
