using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class QuestNpcConfiguration : IEntityTypeConfiguration<QuestNpc>
{
    public void Configure(EntityTypeBuilder<QuestNpc> builder)
    {
        builder.ToTable("quest_npcs");

        builder.HasKey(qn => qn.QuestNpcId);
        builder.Property(qn => qn.QuestNpcId).HasColumnName("quest_npc_id");

        builder.Property(qn => qn.QuestId).HasColumnName("quest_id");
        builder.Property(qn => qn.NpcId).HasColumnName("npc_id");

        builder.Property(qn => qn.Role)
            .HasColumnName("role")
            .HasColumnType("text");

        builder.HasOne(qn => qn.Quest)
            .WithMany()
            .HasForeignKey(qn => qn.QuestId);

        builder.HasOne(qn => qn.Npc)
            .WithMany()
            .HasForeignKey(qn => qn.NpcId);
    }
}
