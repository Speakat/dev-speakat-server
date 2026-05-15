using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speakat.Domain.Entities;

namespace Speakat.Infrastructure.Persistence.Configurations;

public class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.ToTable("game_sessions");

        builder.HasKey(gs => gs.SessionId);
        builder.Property(gs => gs.SessionId)
            .HasColumnName("session_id")
            .HasMaxLength(36)
            .HasConversion<string>()
            .ValueGeneratedNever();

        builder.Property(gs => gs.UserId).HasColumnName("user_id");
        builder.Property(gs => gs.QuestId).HasColumnName("quest_id");

        builder.Property(gs => gs.Status)
            .HasColumnName("status")
            .HasMaxLength(20);

        builder.Property(gs => gs.SemanticScore).HasColumnName("semantic_score");
        builder.Property(gs => gs.GrammarScore).HasColumnName("grammar_score");
        builder.Property(gs => gs.NaturalnessScore).HasColumnName("naturalness_score");
        
        builder.Property(gs => gs.StartedAt)
            .HasColumnName("started_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(gs => gs.EndedAt)
            .HasColumnName("ended_at");

        builder.HasOne(gs => gs.User)
            .WithMany()
            .HasForeignKey(gs => gs.UserId);

        builder.HasOne(gs => gs.Quest)
            .WithMany()
            .HasForeignKey(gs => gs.QuestId);
    }
}
