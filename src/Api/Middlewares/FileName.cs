using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Middlewares;

public class AuditLoggingMidelware : IMiddleware
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
            Console.WriteLine(JsonSerializer.Serialize(audit, options: new()
            {
                WriteIndented = true
            }));
        }
    }

}

public class Audit : Dictionary<string, object>
{
 
}
