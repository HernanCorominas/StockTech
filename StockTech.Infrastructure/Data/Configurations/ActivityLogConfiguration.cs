using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        
        builder.Property(x => x.Message).HasColumnName("message").IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").IsRequired().HasMaxLength(50);
        builder.Property(x => x.Details).HasColumnName("details");
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id");
        
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.SetNull);

        // Ignoring properties that don't exist in the current Supabase schema
        builder.Ignore(x => x.BranchId);
        builder.Ignore(x => x.Branch);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
