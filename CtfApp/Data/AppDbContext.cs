using Microsoft.EntityFrameworkCore;

namespace CtfApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CtfTask> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ctf_game;Username=admin;Password=123");
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
