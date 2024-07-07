using Api.Apikey;
using Api.ExceptionHandlers;
using Api.ExtensionMethods;
using Api.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;

var apikeys = new ApiKeyStore()
{
    ["key1"] = ["Admin", "User"],
    ["key2"] = ["User"],
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
builder.Services.AddSingleton<AuditLoggingMiddleware>();
builder.Services.AddSingleton<ErrorResponseMiddleware>();
builder.Services.AddScoped<Audit>();
builder.Services.AddEndpointsApiExplorer();
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
        ctx.ProblemDetails.Extensions.Add("trace-id", ctx.HttpContext.TraceIdentifier);
        ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
    });
builder.Services.AddAuthentication("ApiKey")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", null);
builder.Services.AddAuthorization();

var app = builder.Build();

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

var api = app.MapGroup("");

api.MapPost("/", (HttpContext ctx, [FromBody] RootDto dto, Audit audit) =>
{
    audit["what"] = "🤣😂😊";
    return Results.Ok(dto);
})
.RequireAuthorization(adminPolicy)
.AddActionDescription(EnumAction.Search)
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

