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
using System.Text;
using Serilog;
using Asp.Versioning;
using StockTech.API.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ─────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console()
                 .Enrich.FromLogContext());

// ─── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<StockTechDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(3)));

// ─── Repositories & UnitOfWork ───────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ─── Infrastructure Services ──────────────────────────────────────────────────
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

// ─── Application Services ────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IUserService, UserService>();

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireProductWrite", policy => policy.RequireClaim("permission", "product:write"));
    options.AddPolicy("RequireProductDelete", policy => policy.RequireClaim("permission", "product:delete"));
    options.AddPolicy("RequireInvoiceCreate", policy => policy.RequireClaim("permission", "invoice:create"));
    options.AddPolicy("RequireUserManage", policy => policy.RequireClaim("permission", "user:manage"));
    options.AddPolicy("RequireUserRead", policy => policy.RequireClaim("permission", "user:read"));
});

// ─── Audit.NET Configuration ──────────────────────────────────────────────────
Audit.Core.Configuration.Setup()
    .UseEntityFramework(_ => _
        .AuditTypeMapper(t => typeof(AuditLog))
        .AuditEntityAction<AuditLog>((ev, ent, auditEntity) =>
        {
            auditEntity.TableName = ent.Table;
            auditEntity.Action = ent.Action;
            auditEntity.KeyValues = JsonSerializer.Serialize(ent.PrimaryKey);
            auditEntity.OldValues = ent.Action == "Update" ? JsonSerializer.Serialize(ent.Changes) : null;
            auditEntity.NewValues = (ent.Action == "Insert" || ent.Action == "Update") ? JsonSerializer.Serialize(ent.ColumnValues) : null;
            auditEntity.User = ev.CustomFields.ContainsKey("User") ? ev.CustomFields["User"].ToString() : ev.Environment.UserName;
            auditEntity.CreatedAt = DateTime.UtcNow;
            auditEntity.UpdatedAt = DateTime.UtcNow;
        })
        .IgnoreMatchedProperties(true));

// ─── CORS ────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201", "https://stocktechfrontend.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ─── Controllers & Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(typeof(StockTech.Application.Interfaces.IAuthService).Assembly);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
