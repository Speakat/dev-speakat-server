using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class UserStageConfiguration : IEntityTypeConfiguration<UserStage>
{
    public void Configure(EntityTypeBuilder<UserStage> builder)
    {
        builder.ToTable("user_stages");

        builder.HasKey(us => us.UserStageId);
        builder.Property(us => us.UserStageId).HasColumnName("user_stage_id");

        builder.Property(us => us.UserId).HasColumnName("user_id");
        builder.Property(us => us.StageId).HasColumnName("stage_id");
        builder.Property(us => us.StartedAt).HasColumnName("started_at");
        builder.Property(us => us.CompletedAt).HasColumnName("completed_at");

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId);

        builder.HasOne(us => us.Stage)
            .WithMany()
            .HasForeignKey(us => us.StageId);

        builder.HasIndex(us => new { us.UserId, us.StageId })
            .HasDatabaseName("ix_user_stages_user_id_stage_id");
    }
}
