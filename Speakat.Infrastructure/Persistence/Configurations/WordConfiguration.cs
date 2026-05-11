using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.ToTable("words");

        builder.HasKey(w => w.WordId);
        builder.Property(w => w.WordId).HasColumnName("word_id");

        builder.Property(w => w.LanguageId).HasColumnName("language_id");

        builder.Property(w => w.Text)
            .HasColumnName("word")
            .HasMaxLength(255);

        builder.Property(w => w.Definition)
            .HasColumnName("definition")
            .HasColumnType("text");

        builder.Property(w => w.Phonetic)
            .HasColumnName("phonetic")
            .HasMaxLength(255);

        builder.Property(w => w.AudioUrl)
            .HasColumnName("audio_url")
            .HasColumnType("text");

        builder.HasOne(w => w.Language)
            .WithMany()
            .HasForeignKey(w => w.LanguageId);
    }
}
