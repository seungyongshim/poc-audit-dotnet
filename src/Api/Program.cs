using Api.ExceptionHandlers;
using Api.ExtensionMethods;
using Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Templates;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Services.AddSingleton<AuditLoggingMidelware>();
builder.Services.AddScoped<Audit>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((ctx, config) =>
{
    config.WriteTo.Console(new ExpressionTemplate("{@p['audit']}\n"));
    config.Destructure.With<AuditFormatter>();
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuditLoggingMidelware>();
app.UseExceptionHandler();

app.MapPost("/", (HttpContext ctx, [FromBody] RootDto dto, Audit audit) =>
{
    return Results.Ok(dto);
})
.WithActionDescription(EnumAction.Search)
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

