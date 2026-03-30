using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTech.Application.Interfaces;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;
using StockTech.Infrastructure.Persistence;
using StockTech.Infrastructure.Services;
using StockTech.Infrastructure.Repositories;
using StockTech.Infrastructure.Security;
using StockTech.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace StockTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StockTechDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IStorageService, LocalStorageService>(); // Usamos LocalStorage como fallback

        // ─── RBAC Infrastructure ─────────────────────────────────────────────────────
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}
