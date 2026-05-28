using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class QuestConfiguration : IEntityTypeConfiguration<Quest>
{
    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        builder.ToTable("quests");

        builder.HasKey(q => q.QuestId);
        builder.Property(q => q.QuestId).HasColumnName("quest_id");

        builder.Property(q => q.StageId).HasColumnName("stage_id");
        builder.Property(q => q.PromptId).HasColumnName("prompt_id");

        builder.Property(q => q.Title)
            .HasColumnName("title")
            .HasMaxLength(255);

        builder.Property(q => q.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(q => q.Thumbnail)
            .HasColumnName("thumbnail")
            .HasColumnType("text");

        builder.Property(q => q.SortOrder)
            .HasColumnName("sort_order");
        
        builder.Property(q => q.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(q => q.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

        builder.HasOne(q => q.Stage)
            .WithMany()
            .HasForeignKey(q => q.StageId);

        builder.HasOne(q => q.Prompt)
            .WithMany()
            .HasForeignKey(q => q.PromptId);
    }
}
