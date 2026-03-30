using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TableName).HasColumnName("table_name").IsRequired().HasMaxLength(100);
        builder.Property(x => x.Action).HasColumnName("action").IsRequired().HasMaxLength(50);
        builder.Property(x => x.KeyValues).HasColumnName("key_values");
        builder.Property(x => x.OldValues).HasColumnName("old_values");
        builder.Property(x => x.NewValues).HasColumnName("new_values");
        builder.Property(x => x.UserId).HasColumnName("user").HasMaxLength(100);
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
