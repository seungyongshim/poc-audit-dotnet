using Api.Apikey;
using Api.EndpointFilters;
using Api.ExceptionHandlers;
using Api.ExtensionMethods;
using Api.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddSingleton<AuditLoggingMidelware>();
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
        Scheme = "ApiKeyScheme"
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
                }
            },
            new List<string>()
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
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication("ApiKey")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", null);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuditLoggingMidelware>();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("");

api.MapPost("/", (HttpContext ctx, [FromBody] RootDto dto, Audit audit) =>
{
    return Results.Ok(dto);
})
.RequireAuthorization(adminPolicy)
.AddActionDescription(EnumAction.Search)
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

