using System.Net;
using System.Text.Json;

namespace Api.Middlewares;

public class ErrorResponseMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode >= (int)HttpStatusCode.BadRequest)
            {
                await HandleErrorResponseAsync(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleErrorResponseAsync(HttpContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            StatusCode = response.StatusCode,
            Message = "An error occurred while processing your request.",
            Details = GetErrorDetails(response.StatusCode)
        };

        var errorJson = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(errorJson);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = context.Response;
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            StatusCode = response.StatusCode,
            Message = "An unexpected error occurred.",
            Details = ex.Message
        };

        var errorJson = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(errorJson);
    }

    private string GetErrorDetails(int statusCode)
    {
        return statusCode switch
        {
            (int)HttpStatusCode.BadRequest => "The request could not be understood or was missing required parameters.",
            (int)HttpStatusCode.Unauthorized => "Authentication failed or user does not have permissions for the requested operation.",
            (int)HttpStatusCode.Forbidden => "Access denied.",
            (int)HttpStatusCode.NotFound => "The requested resource could not be found.",
            (int)HttpStatusCode.InternalServerError => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };
    }
}
