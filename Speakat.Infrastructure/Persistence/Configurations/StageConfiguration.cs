using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class StageConfiguration : IEntityTypeConfiguration<Stage>
{
    public void Configure(EntityTypeBuilder<Stage> builder)
    {
        
        builder.ToTable("stages");

        builder.HasKey(s => s.StageId);
        builder.Property(s => s.StageId).HasColumnName("stage_id");

        builder.Property(s => s.Title)
            .HasColumnName("title")
            .HasMaxLength(255);

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(s => s.SortOrder)
            .HasColumnName("sort_order");

        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
    }
}