using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StockTech.Application.Services;
using StockTech.Application.Interfaces;

namespace StockTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IInventoryMovementService, InventoryMovementService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<ISystemSettingService, SystemSettingService>();
        services.AddScoped<ISkuGenerator, SkuGenerator>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
