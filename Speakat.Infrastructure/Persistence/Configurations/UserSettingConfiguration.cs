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

        builder.Property(s => s.UserId).HasColumnName("user_id").HasColumnType("char(36)").HasMaxLength(36);
        builder.Property(s => s.ShowNpcScript).HasColumnName("show_npc_script");
        builder.Property(s => s.StreakGoal).HasColumnName("streak_goal");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("uq_user_settings_user_id");
    }
}
