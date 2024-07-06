using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;

namespace Api.Middlewares;

public class AuditLoggingMidelware(ILogger<Audit> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        
        var audit = context.RequestServices.GetRequiredService<Audit>();
        audit["Method"] = context.Request.Method;
        audit["Path"] = context.Request.Path;
        audit["QueryString"] = context.Request.QueryString;
        audit["StatusCode"] = context.Response.StatusCode;

        try
        {
            await next(context);
        }
        finally
        {
            logger.LogInformation("{@audit}", audit );
        }
    }

}

public class Audit : Dictionary<string, object>
{
 
}

public class AuditFormatter : IDestructuringPolicy
{
    public object? GetFormat(Type? formatType) => this;

    public JsonSerializerOptions options = new()
    {
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
