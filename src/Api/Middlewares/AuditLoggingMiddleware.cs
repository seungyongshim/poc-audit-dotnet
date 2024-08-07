using System.Net;

namespace Api.Middlewares;
public class AuditLoggingMiddleware(ILogger<Audit> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var audit = context.RequestServices.GetRequiredService<Audit>();
        audit.ReqUri = context.Request.Path.ToUriComponent();
        audit.ReqDomain = context.Request.Host.Host;
        
        try
        {
            await next(context);
        }
        finally
        {
            audit.ActionResult = context.Response.StatusCode switch
            {
                < 400 => "success",
                _ => "failure-" + ((HttpStatusCode)context.Response.StatusCode).ToString()  
            };
            logger.LogInformation("{@audit}", audit );
        }
    }
}
