using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class SessionObjectiveConfiguration : IEntityTypeConfiguration<SessionObjective>
{
    public void Configure(EntityTypeBuilder<SessionObjective> builder)
    {
        builder.ToTable("session_objectives");

        builder.HasKey(so => so.SessionObjectiveId);
        builder.Property(so => so.SessionObjectiveId).HasColumnName("session_objective_id");

        builder.Property(so => so.SessionId)
            .HasColumnName("session_id")
            .HasMaxLength(36)
            .HasConversion<string>();

        builder.Property(so => so.QuestObjectiveId).HasColumnName("quest_objective_id");
        builder.Property(so => so.IsAchieved).HasColumnName("is_achieved");

        builder.HasOne(so => so.GameSession)
            .WithMany()
            .HasForeignKey(so => so.SessionId);

        builder.HasOne(so => so.QuestObjective)
            .WithMany()
            .HasForeignKey(so => so.QuestObjectiveId);
    }
}
