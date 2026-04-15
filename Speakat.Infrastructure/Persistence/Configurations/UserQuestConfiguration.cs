using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class UserQuestConfiguration : IEntityTypeConfiguration<UserQuest>
{
    public void Configure(EntityTypeBuilder<UserQuest> builder)
    {
        builder.ToTable("user_quests");

        builder.HasKey(uq => uq.UserQuestId);
        builder.Property(uq => uq.UserQuestId).HasColumnName("user_quest_id");

        builder.Property(uq => uq.UserId).HasColumnName("user_id");
        builder.Property(uq => uq.QuestId).HasColumnName("quest_id");
        builder.Property(uq => uq.StartedAt).HasColumnName("started_at");
        builder.Property(uq => uq.CompletedAt).HasColumnName("completed_at");

        builder.HasOne(uq => uq.User)
            .WithMany()
            .HasForeignKey(uq => uq.UserId);

        builder.HasOne(uq => uq.Quest)
            .WithMany()
            .HasForeignKey(uq => uq.QuestId);

        builder.HasIndex(uq => new { uq.UserId, uq.QuestId })
            .HasDatabaseName("ix_user_quests_user_id_quest_id");
    }
}
