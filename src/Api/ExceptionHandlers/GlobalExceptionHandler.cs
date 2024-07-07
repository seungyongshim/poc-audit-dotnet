using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var result = exception switch
        {
            BadHttpRequestException ex => TypedResults.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad request",
                detail: ex.Message,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            _ => TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unhandled error occurred",
                detail: exception.Message,
                type: exception.GetType().Name)
        };

        await result.ExecuteAsync(httpContext).ConfigureAwait(false);
        return true;
    }
}
