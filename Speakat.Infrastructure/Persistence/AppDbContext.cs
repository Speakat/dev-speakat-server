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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
        modelBuilder.ApplyConfiguration(new StageConfiguration());
        modelBuilder.ApplyConfiguration(new UserStageConfiguration());
        modelBuilder.ApplyConfiguration(new QuestConfiguration());
    }
}
