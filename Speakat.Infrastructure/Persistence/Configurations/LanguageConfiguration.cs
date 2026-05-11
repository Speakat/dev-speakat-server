using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages");

        builder.HasKey(l => l.LanguageId);
        builder.Property(l => l.LanguageId).HasColumnName("language_id");

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
    }
}
