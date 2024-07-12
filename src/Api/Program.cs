using System.Diagnostics;
using Api.Apikey;
using Api.ExceptionHandlers;
using Api.ExtensionMethods;
using Api.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore.Http.ResultsFactory;
using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var apikeys = new ApiKeyStore()
{
    ["1"] = ["Admin", "User"],
    ["2"] = ["User"],
};

var adminPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireRole("Admin")
    .Build();

var userPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireRole("User")
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddOpenTelemetry()
    .WithTracing(x  => x.AddAspNetCoreInstrumentation()
    .AddOtlpExporter());
builder.AddFluentValidationEndpointFilter();
builder.Services.AddSingleton<IFluentValidationEndpointFilterResultsFactory, SimpleResultsFactory>();
builder.Services.AddSingleton<IValidator<RootDto>, RootDtoValidator>();
builder.Services.AddSingleton<AuditLoggingMiddleware>();
builder.Services.AddSingleton<ErrorResponseMiddleware>();
builder.Services.AddScoped<Audit>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new()
    {
        Description = "API Key Authentication",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "ApiKeyScheme",
    });

    options.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
            }, []
        },
    });
});
builder.Services.AddSingleton(apikeys);
builder.Host.UseSerilog((ctx, config) =>
{
    config.WriteTo.Console(new ExpressionTemplate("{@p['audit']}\n"));
    config.Destructure.With<AuditFormatter>();
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("traceId", $"{Activity.Current?.Id}");
        ctx.ProblemDetails.Extensions.Add("requestId", Activity.Current?.RootId);
    });
builder.Services.AddAuthentication("ApiKey")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsEnvironment("local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("").AddFluentValidationFilter();

api.MapGet("/", (HttpContext ctx, Audit audit) =>
{
    audit["what"] = "🤣😂😊";
    throw new Exception("This is a test exception");
});

api.MapPost("/", (HttpContext ctx, [FromBody] RootDto dto, Audit audit) =>
{
    audit["what"] = "🤣😂😊";
    return TypedResults.Ok(dto);
})
.RequireAuthorization(adminPolicy)
.AddActionDescription(EnumAction.Search)
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

