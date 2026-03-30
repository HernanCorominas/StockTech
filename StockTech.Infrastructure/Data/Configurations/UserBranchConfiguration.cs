using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class UserBranchConfiguration : IEntityTypeConfiguration<UserBranch>
{
    public void Configure(EntityTypeBuilder<UserBranch> builder)
    {
        builder.ToTable("user_branches");

        builder.HasKey(ub => new { ub.UserId, ub.BranchId });

        builder.Property(ub => ub.UserId).HasColumnName("user_id");
        builder.Property(ub => ub.BranchId).HasColumnName("branch_id");
        builder.Property(ub => ub.RoleId).HasColumnName("role_id").IsRequired();

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.UserBranches)
            .HasForeignKey(ub => ub.UserId);

        builder.HasOne(ub => ub.Branch)
            .WithMany(b => b.UserBranches)
            .HasForeignKey(ub => ub.BranchId);

        builder.HasOne(ub => ub.Role)
            .WithMany(r => r.UserBranches)
            .HasForeignKey(ub => ub.RoleId);
    }
}
