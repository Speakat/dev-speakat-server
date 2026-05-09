using Microsoft.EntityFrameworkCore;
using Speakat.Domain.Entities;
using Speakat.Infrastructure.Persistence.Configurations;

namespace Speakat.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<UserStage> UserStages => Set<UserStage>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<Npc> Npcs => Set<Npc>();
    public DbSet<QuestNpc> QuestNpcs => Set<QuestNpc>();
    public DbSet<Prompt> Prompts => Set<Prompt>();
    public DbSet<Language> Languages => Set<Language>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
        
        modelBuilder.ApplyConfiguration(new StageConfiguration());
        modelBuilder.ApplyConfiguration(new UserStageConfiguration());
        modelBuilder.ApplyConfiguration(new QuestConfiguration());
        
        modelBuilder.ApplyConfiguration(new NpcConfiguration());
        modelBuilder.ApplyConfiguration(new QuestNpcConfiguration());
        modelBuilder.ApplyConfiguration(new PromptConfiguration());
        
        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
    }
}
