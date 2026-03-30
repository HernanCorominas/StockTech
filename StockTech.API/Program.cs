using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StockTech.Application.Interfaces;
using StockTech.Application.Services;
using StockTech.Domain.Entities;
using Audit.Core;
using System.Text.Json;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;
using StockTech.Infrastructure.Persistence;
using StockTech.Infrastructure.Services;
using StockTech.Infrastructure.Logging;
using System.Text;
using StockTech.Application.Hubs;
using Serilog;
using Asp.Versioning;
using StockTech.API.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using StockTech.Application;
using StockTech.Infrastructure;
using StockTech.API;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ─────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console()
                 .Enrich.FromLogContext());

// ─── Clean Architecture DI ───────────────────────────────────────────────────
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);

// ─── Audit.NET Configuration ──────────────────────────────────────────────────
builder.Services.ConfigureAudit();

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockTech API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// ─── Data Seeding ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StockTechDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

app.Run();
