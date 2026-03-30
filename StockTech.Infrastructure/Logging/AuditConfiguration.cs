using System.Text.Json;
using Audit.Core;
using Microsoft.Extensions.DependencyInjection;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Logging;

public static class AuditConfiguration
{
    public static void ConfigureAudit(this IServiceCollection services)
    {
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
                    auditEntity.UserId = ev.CustomFields.ContainsKey("User") ? ev.CustomFields["User"].ToString() : ev.Environment.UserName;

                    if (ev.CustomFields.TryGetValue("BranchId", out var branchId) && branchId != null)
                    {
                        auditEntity.BranchId = (Guid?)branchId;
                    }
                })
                .IgnoreMatchedProperties(true));
    }
}
