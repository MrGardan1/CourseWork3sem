using Microsoft.EntityFrameworkCore;

namespace CtfApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CtfTask> Tasks { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }  // ← ДОБАВИЛИ

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ctf_game;Username=admin;Password=123");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка составного ключа для UserTask
        modelBuilder.Entity<UserTask>()
            .HasKey(ut => new { ut.UserId, ut.TaskId });

        base.OnModelCreating(modelBuilder);
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    public bool IsAdmin { get; set; } = false;
}

public class CtfTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    public int Points { get; set; }
}

// ← НОВАЯ МОДЕЛЬ
public class UserTask
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
}
