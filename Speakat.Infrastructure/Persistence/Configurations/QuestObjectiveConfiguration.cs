using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class QuestObjectiveConfiguration : IEntityTypeConfiguration<QuestObjective>
{
    public void Configure(EntityTypeBuilder<QuestObjective> builder)
    {
        builder.ToTable("quest_objectives");

        builder.HasKey(qo => qo.QuestObjectiveId);
        builder.Property(qo => qo.QuestObjectiveId).HasColumnName("quest_objective_id");

        builder.Property(qo => qo.QuestId).HasColumnName("quest_id");
        builder.Property(qo => qo.ObjectiveId).HasColumnName("objective_id");
        
        builder.Property(qo => qo.SortOrder).HasColumnName("sort_order");

        builder.HasOne(qo => qo.Quest)
            .WithMany()
            .HasForeignKey(qo => qo.QuestId);

        builder.HasOne(qo => qo.Objective)
            .WithMany()
            .HasForeignKey(qo => qo.ObjectiveId);
    }
}
