using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Infrastructure.Data.Context;

/// <summary>
/// Design-time factory for EduPlayKidsDbContext.
/// Used by Entity Framework Core tools for migrations and scaffolding.
/// </summary>
public class EduPlayKidsDbContextFactory : IDesignTimeDbContextFactory<EduPlayKidsDbContext>
{
    public EduPlayKidsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EduPlayKidsDbContext>();

        // Configure SQLite for design-time operations
        optionsBuilder.UseSqlite("Data Source=eduplaykids.db");

        // Create a logger factory for design-time use
        using var loggerFactory = LoggerFactory.Create(builder => { });
        var logger = loggerFactory.CreateLogger<EduPlayKidsDbContext>();

        return new EduPlayKidsDbContext(optionsBuilder.Options, logger);
    }
}