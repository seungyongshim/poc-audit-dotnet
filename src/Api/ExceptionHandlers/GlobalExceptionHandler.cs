using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.StatusCode == 400)
        {
            var problemDetails = new ProblemDetails
            {
                Status = 400,
                Title = "잘못된 요청",
                Detail = "요청 데이터를 확인해 주세요.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            return true;
        }
        else
        {
            var problemDetails = new ProblemDetails
            {
                Status = 500,
                Type = exception.GetType().Name,
                Title = "An unhandled error occurred",
                Detail = exception.Message
            };
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
