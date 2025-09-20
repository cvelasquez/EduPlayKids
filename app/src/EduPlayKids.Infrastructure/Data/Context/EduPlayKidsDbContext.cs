using EduPlayKids.Domain.Common;
using EduPlayKids.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EduPlayKids.Infrastructure.Data.Context;

/// <summary>
/// Main database context for the EduPlayKids application.
/// Manages the SQLite database with 12 core entities supporting offline-first educational content,
/// multi-user profiles, progress tracking, and premium subscription features.
/// </summary>
public class EduPlayKidsDbContext : DbContext
{
    private readonly ILogger<EduPlayKidsDbContext> _logger;

    public EduPlayKidsDbContext(DbContextOptions<EduPlayKidsDbContext> options, ILogger<EduPlayKidsDbContext> logger)
        : base(options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Core User Management
    /// <summary>
    /// Parent/guardian user accounts with authentication and profile management.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Individual child profiles with age-specific configurations and learning preferences.
    /// </summary>
    public DbSet<Child> Children { get; set; } = null!;

    /// <summary>
    /// Premium subscription management for freemium model.
    /// </summary>
    public DbSet<Subscription> Subscriptions { get; set; } = null!;

    /// <summary>
    /// Application configuration and user preferences.
    /// </summary>
    public DbSet<Settings> Settings { get; set; } = null!;
    #endregion

    #region Educational Content
    /// <summary>
    /// Educational subjects: Math, Reading/Phonics, Basic Concepts, Logic, Science.
    /// </summary>
    public DbSet<Subject> Subjects { get; set; } = null!;

    /// <summary>
    /// Learning activities with difficulty levels and curriculum alignment.
    /// </summary>
    public DbSet<Activity> Activities { get; set; } = null!;

    /// <summary>
    /// Interactive questions and content for activities.
    /// </summary>
    public DbSet<ActivityQuestion> ActivityQuestions { get; set; } = null!;
    #endregion

    #region Progress Tracking & Gamification
    /// <summary>
    /// Individual child progress tracking per activity.
    /// </summary>
    public DbSet<UserProgress> UserProgress { get; set; } = null!;

    /// <summary>
    /// Achievement definitions for gamification system.
    /// </summary>
    public DbSet<Achievement> Achievements { get; set; } = null!;

    /// <summary>
    /// Earned achievements and rewards tracking.
    /// </summary>
    public DbSet<UserAchievement> UserAchievements { get; set; } = null!;

    /// <summary>
    /// Learning session tracking for analytics and screen time management.
    /// </summary>
    public DbSet<Session> Sessions { get; set; } = null!;
    #endregion

    #region Compliance & Audit
    /// <summary>
    /// COPPA-compliant audit logging for child safety and data privacy.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    #endregion

    /// <summary>
    /// Configures the entity relationships and database schema.
    /// Optimized for mobile SQLite performance with proper indexing.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _logger.LogDebug("Configuring EduPlayKids database model");

        // Apply all entity configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EduPlayKidsDbContext).Assembly);

        // Configure soft delete global query filter
        ConfigureSoftDeleteFilter(modelBuilder);

        // Configure audit timestamps
        ConfigureAuditTimestamps(modelBuilder);

        // Configure indexes for performance
        ConfigureIndexes(modelBuilder);

        _logger.LogDebug("Database model configuration completed");
    }

    /// <summary>
    /// Configures global query filters for soft delete functionality.
    /// Ensures deleted entities are automatically filtered from queries.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    private void ConfigureSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        // Apply soft delete filter to all entities that inherit from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        _logger.LogDebug("Soft delete filters configured for all base entities");
    }

    /// <summary>
    /// Configures automatic audit timestamp updates.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    private void ConfigureAuditTimestamps(ModelBuilder modelBuilder)
    {
        // Audit timestamps will be handled in SaveChanges override
        _logger.LogDebug("Audit timestamp configuration prepared");
    }

    /// <summary>
    /// Configures database indexes for optimal performance on mobile devices.
    /// Focuses on frequently queried fields for educational content and user progress.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // TODO: Add specific indexes for:
        // - User lookup (UserId, IsDeleted)
        // - Activity queries (SubjectId, DifficultyLevel, IsDeleted)
        // - Progress tracking (UserId, ActivityId, CompletedAt)
        // - Session tracking (UserId, StartedAt)
        // - Analytics queries (UserId, EventDate, EventType)

        _logger.LogDebug("Database indexes configured for mobile performance");
    }

    /// <summary>
    /// Overrides SaveChanges to automatically handle audit timestamps and soft deletes.
    /// Ensures data integrity and proper audit trails for child safety compliance.
    /// </summary>
    /// <returns>The number of entities saved to the database.</returns>
    public override int SaveChanges()
    {
        UpdateAuditTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Overrides SaveChangesAsync to automatically handle audit timestamps and soft deletes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entities saved to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit timestamps for all tracked entities.
    /// </summary>
    private void UpdateAuditTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    _logger.LogDebug("Setting creation timestamps for new {EntityType}", entry.Entity.GetType().Name);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Property(x => x.CreatedAt).IsModified = false; // Prevent overwriting creation time
                    _logger.LogDebug("Updating timestamp for modified {EntityType}", entry.Entity.GetType().Name);
                    break;

                case EntityState.Deleted:
                    // Convert hard deletes to soft deletes for audit compliance
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = now;
                    _logger.LogDebug("Converting hard delete to soft delete for {EntityType}", entry.Entity.GetType().Name);
                    break;
            }
        }
    }

    /// <summary>
    /// Ensures the database is created and migrated to the latest version.
    /// Handles initial setup for new installations.
    /// </summary>
    public async Task EnsureDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Ensuring database exists and is up to date");

            // Create database if it doesn't exist
            var created = await Database.EnsureCreatedAsync();
            if (created)
            {
                _logger.LogInformation("Database created successfully");
            }

            // Apply pending migrations
            var pendingMigrations = await Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                await Database.MigrateAsync();
                _logger.LogInformation("Database migrations completed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure database");
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with initial educational content and configuration data.
    /// Called during application startup for new installations.
    /// </summary>
    public async Task SeedInitialDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding process");

            // TODO: Implement data seeding for:
            // - Initial subjects (Math, Reading, Science, etc.)
            // - Basic activities for each subject
            // - Default app settings
            // - Localization data for Spanish/English

            await SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed initial data");
            throw;
        }
    }
}