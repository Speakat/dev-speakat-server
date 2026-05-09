using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class FlashcardConfiguration : IEntityTypeConfiguration<Flashcard>
{
    public void Configure(EntityTypeBuilder<Flashcard> builder)
    {
        builder.ToTable("flashcards");

        builder.HasKey(f => f.FlashcardId);
        builder.Property(f => f.FlashcardId).HasColumnName("flashcard_id");

        builder.Property(f => f.WordId).HasColumnName("word_id");
        builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        builder.Property(f => f.IsMastered).HasColumnName("is_mastered");

        builder.HasOne(f => f.Word)
            .WithMany()
            .HasForeignKey(f => f.WordId);
    }
}
