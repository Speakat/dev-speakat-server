using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class UserFlashcardConfiguration : IEntityTypeConfiguration<UserFlashcard>
{
    public void Configure(EntityTypeBuilder<UserFlashcard> builder)
    {
        builder.ToTable("user_flashcards");

        builder.HasKey(uf => uf.UserFlashcardId);
        builder.Property(uf => uf.UserFlashcardId).HasColumnName("user_flashcard_id");

        builder.Property(uf => uf.UserId).HasColumnName("user_id");

        builder.Property(uf => uf.FlashcardId).HasColumnName("flashcard_id");
        builder.Property(uf => uf.QuestId).HasColumnName("quest_id");

        builder.Property(uf => uf.RecommendationReason)
            .HasColumnName("recommendation_reason")
            .HasColumnType("text");

        builder.Property(uf => uf.SavedAt).HasColumnName("saved_at");

        builder.HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId);

        builder.HasOne(uf => uf.Flashcard)
            .WithMany()
            .HasForeignKey(uf => uf.FlashcardId);

        builder.HasOne(uf => uf.Quest)
            .WithMany()
            .HasForeignKey(uf => uf.QuestId);
    }
}
