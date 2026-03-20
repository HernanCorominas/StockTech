using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(200);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(x => x.Username).IsUnique();
    }
}
