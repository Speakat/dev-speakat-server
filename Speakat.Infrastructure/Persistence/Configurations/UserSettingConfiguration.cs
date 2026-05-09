using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.ToTable("user_settings");

        builder.HasKey(s => s.UserSettingId);
        builder.Property(s => s.UserSettingId).HasColumnName("user_setting_id");

        builder.Property(s => s.UserId).HasColumnName("user_id");
        
        builder.Property(s => s.ShowNpcScript)
            .HasColumnName("show_npc_script")
            .HasDefaultValue(true);
        
        builder.Property(s => s.StreakGoal)
            .HasColumnName("streak_goal");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("uq_user_settings_user_id");
    }
}
