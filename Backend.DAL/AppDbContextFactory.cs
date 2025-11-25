using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Backend.DAL;

/// <summary>
/// Фабрика для design-time, чтобы EF Core мог создавать AppDbContext при миграциях
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Подключение к SQLite (можно заменить на PostgreSQL или SQL Server)
        // Для production берите путь или строку подключения из environment variables
        optionsBuilder.UseSqlite("Data Source=app.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}