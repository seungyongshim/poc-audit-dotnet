using Api.Middlewares;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AuditLoggingMidelware>();
builder.Services.AddScoped<Audit>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuditLoggingMidelware>();

app.MapPost("/", (HttpContext ctx, [FromBody] RootDto dto, [FromServices] Audit audit) =>
{
    audit["GreatAgain"] = "Yes";
    return Results.Ok(dto);
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

