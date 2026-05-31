using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class QuestEmbeddingConfiguration : IEntityTypeConfiguration<QuestEmbedding>
{
    public void Configure(EntityTypeBuilder<QuestEmbedding> builder)
    {
        builder.ToTable("quest_embeddings");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.QuestId).HasColumnName("quest_id");

        builder.Property(e => e.ReferenceSentence)
            .HasColumnName("reference_sentence")
            .HasMaxLength(500);

        builder.HasOne(e => e.Quest)
            .WithMany()
            .HasForeignKey(e => e.QuestId);
    }
}
