using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using StockTech.Application.Interfaces;

namespace StockTech.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
        services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

        services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(typeof(StockTech.Application.Interfaces.IAuthService).Assembly);

        // ─── Api Versioning ──────────────────────────────────────────────────────────
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // ─── Swagger ──────────────────────────────────────────────────────────
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockTech API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // ─── JWT Authentication ───────────────────────────────────────────────────────
        var jwtKey = configuration["Jwt:Key"]!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        // ─── Authorization ───────────────────────────────────────────────────────────
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireUserRead", policy => policy.RequireRole("Admin", "Manager", "SystemAdmin"));
            options.AddPolicy("RequireUserManage", policy => policy.RequireRole("Admin", "SystemAdmin"));
            options.AddPolicy("RequireProductWrite", policy => policy.RequireRole("Admin", "Manager", "SystemAdmin"));
            options.AddPolicy("RequireProductDelete", policy => policy.RequireRole("Admin", "SystemAdmin"));
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin", "SystemAdmin"));
            options.AddPolicy("RequireAdminPermission", policy => policy.RequireClaim("permission", "admin:*"));
        });

        // ─── CORS ────────────────────────────────────────────────────────────────────
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
                policy.WithOrigins("http://localhost:4200", "http://localhost:4201", "https://stocktechfrontend.vercel.app")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());
        });

        return services;
    }
}
