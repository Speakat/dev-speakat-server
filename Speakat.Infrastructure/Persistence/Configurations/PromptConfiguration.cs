using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class PromptConfiguration : IEntityTypeConfiguration<Prompt>
{
    public void Configure(EntityTypeBuilder<Prompt> builder)
    {
        builder.ToTable("prompts");

        builder.HasKey(p => p.PromptId);
        builder.Property(p => p.PromptId).HasColumnName("prompt_id");

        builder.Property(p => p.Scenario)
            .HasColumnName("scenario")
            .HasColumnType("text");

        builder.Property(p => p.SuccessCriteria)
            .HasColumnName("success_criteria")
            .HasColumnType("text");

        builder.Property(p => p.OpeningLine)
            .HasColumnName("opening_line")
            .HasColumnType("text");
    }
}
