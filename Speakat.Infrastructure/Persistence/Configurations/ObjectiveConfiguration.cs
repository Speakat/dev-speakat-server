using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
{
    public void Configure(EntityTypeBuilder<Objective> builder)
    {
        builder.ToTable("objectives");

        builder.HasKey(o => o.ObjectiveId);
        builder.Property(o => o.ObjectiveId).HasColumnName("objective_id");

        builder.Property(o => o.Name)
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(o => o.Description)
            .HasColumnName("description")
            .HasColumnType("text");
    }
}
