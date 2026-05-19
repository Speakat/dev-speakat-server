using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class NpcConfiguration : IEntityTypeConfiguration<Npc>
{
    public void Configure(EntityTypeBuilder<Npc> builder)
    {
        builder.ToTable("npcs");

        builder.HasKey(n => n.NpcId);
        builder.Property(n => n.NpcId).HasColumnName("npc_id");

        builder.Property(n => n.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.Property(n => n.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(n => n.Tone)
            .HasColumnName("tone")
            .HasColumnType("text");

        builder.Property(n => n.Voice)
            .HasColumnName("voice")
            .HasMaxLength(255)
            .IsRequired(false);
    }
}
