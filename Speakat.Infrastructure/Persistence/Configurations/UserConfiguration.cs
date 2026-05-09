using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).HasColumnName("user_id");

        builder.Property(u => u.UserUuid)
            .HasColumnName("user_uuid")
            .HasMaxLength(36)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(u => u.SocialType)
            .HasColumnName("social_type")
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(u => u.SocialId)
            .HasColumnName("social_id")
            .HasMaxLength(255);

        builder.Property(u => u.Nickname)
            .HasColumnName("nickname")
            .HasMaxLength(50);

        builder.Property(u => u.Gender)
            .HasColumnName("gender")
            .HasMaxLength(10)
            .HasConversion<string>();
        
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(u => u.ProfileImageKey)
            .HasColumnName("profile_image_key")
            .HasMaxLength(500);

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(u => new { u.SocialType, u.SocialId })
            .IsUnique()
            .HasDatabaseName("uq_users_social");

        builder.HasIndex(u => u.UserUuid)
            .IsUnique()
            .HasDatabaseName("uq_users_user_uuid");

        builder.HasOne(u => u.Setting)
            .WithOne(s => s.User)
            .HasForeignKey<UserSetting>(s => s.UserId);
    }
}
