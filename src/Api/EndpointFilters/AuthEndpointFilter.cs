
namespace Api.EndpointFilters;

public class AuthEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var apikey = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault();

        if (apikey != "123456")
        {
            return Results.Unauthorized();
        }


        return await next(context);
    }
}
