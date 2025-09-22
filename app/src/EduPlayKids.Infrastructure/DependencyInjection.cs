using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Application.Services;
using EduPlayKids.Infrastructure.Data.Context;
using EduPlayKids.Infrastructure.Repositories;

namespace EduPlayKids.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// Registers all repositories, services, and data access components for the EduPlayKids application.
/// Optimized for mobile performance with proper lifetime management.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container.
    /// Configures Entity Framework, repositories, and data access services.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database context with SQLite configuration optimized for mobile
        services.AddDbContext<EduPlayKidsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=eduplaykids.db";

            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly(typeof(EduPlayKidsDbContext).Assembly.FullName);
                sqliteOptions.CommandTimeout(30); // 30 second timeout for mobile
            });

            // Configure for mobile performance
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.EnableServiceProviderCaching(true);
            options.EnableSensitiveDataLogging(false);

#if DEBUG
            options.EnableSensitiveDataLogging(true);
            options.EnableDetailedErrors(true);
            options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        });

        // Register the database context factory for background services
        services.AddDbContextFactory<EduPlayKidsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=eduplaykids.db";
            options.UseSqlite(connectionString);
        });

        // Register repositories with appropriate lifetimes
        RegisterRepositories(services);

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register application services
        RegisterApplicationServices(services);

        // Register infrastructure services
        RegisterInfrastructureServices(services);

        return services;
    }

    /// <summary>
    /// Registers all repository interfaces and implementations.
    /// Uses scoped lifetime for proper Entity Framework integration.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    private static void RegisterRepositories(IServiceCollection services)
    {
        // Register generic repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register specific repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChildRepository, ChildRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityQuestionRepository, ActivityQuestionRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    }

    /// <summary>
    /// Registers application services for complex business operations.
    /// Uses scoped lifetime for proper dependency injection and state management.
    /// NOTE: Service implementations will be added in Phase 3 - Application Services
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    private static void RegisterApplicationServices(IServiceCollection services)
    {
        // Application services will be implemented in Phase 3
        // For now, keep this method for future service registration

        // TODO: Implement application services in Phase 3:
        // - Educational and learning services
        // - Authentication and security services
        // - Content and curriculum services
        // - Analytics and reporting services
        // - Subscription and billing services
        // - Notification and communication services
        // - COPPA compliance and audit services
    }

    /// <summary>
    /// Registers infrastructure-specific services and utilities.
    /// Includes background services, caching, and mobile-specific optimizations.
    /// NOTE: Service implementations will be added in Phase 3 - Infrastructure Services
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        // Caching services optimized for mobile
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 100; // Limit cache size for mobile devices
            options.CompactionPercentage = 0.25; // Aggressive compaction for memory management
        });

        // TODO: Implement infrastructure services in Phase 3:
        // - Background services for maintenance and cleanup
        // - File and asset management services
        // - Health check services
        // - Configuration and settings services
        // - Security and encryption services
        // - Mobile-specific services
    }

    /// <summary>
    /// Configures Entity Framework options for optimal mobile performance.
    /// Sets up connection pooling, query optimization, and memory management.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">The application configuration</param>
    public static void ConfigureEntityFramework(IServiceCollection services, IConfiguration configuration)
    {
        // Configure connection pool for mobile optimization
        services.AddDbContextPool<EduPlayKidsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=eduplaykids.db";

            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly(typeof(EduPlayKidsDbContext).Assembly.FullName);
            });

            // Mobile-specific optimizations
            options.EnableServiceProviderCaching(true);
            options.EnableSensitiveDataLogging(false);
            options.ConfigureWarnings(warnings =>
            {
                warnings.Default(WarningBehavior.Log);
            });
        },
        poolSize: 32); // Limited pool size for mobile devices

        // Query optimization configured in AddDbContext method above
    }

    /// <summary>
    /// Validates the dependency injection configuration.
    /// Ensures all required services are properly registered.
    /// </summary>
    /// <param name="services">The service collection to validate</param>
    /// <returns>True if configuration is valid, false otherwise</returns>
    public static bool ValidateConfiguration(IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();

            // Validate critical services
            var requiredServices = new[]
            {
                typeof(EduPlayKidsDbContext),
                typeof(IUnitOfWork),
                typeof(IUserRepository),
                typeof(IChildRepository),
                typeof(IEducationalProgressService),
                typeof(IGameificationService)
            };

            foreach (var serviceType in requiredServices)
            {
                var service = serviceProvider.GetService(serviceType);
                if (service == null)
                {
                    Console.WriteLine($"Required service {serviceType.Name} is not registered");
                    return false;
                }
            }

            // Validate database context can be created
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EduPlayKidsDbContext>();
            var canConnect = dbContext.Database.CanConnect();

            if (!canConnect)
            {
                Console.WriteLine("Database connection validation failed");
                return false;
            }

            Console.WriteLine("Dependency injection configuration validation passed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dependency injection validation failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Configures logging for the Infrastructure layer.
    /// Sets up appropriate log levels and filtering for mobile performance.
    /// NOTE: Logging configuration will be handled by the MAUI app startup
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">The application configuration</param>
    public static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement logging configuration in MAUI app startup
        // Basic logging is already configured by default .NET 8 infrastructure
    }

    /// <summary>
    /// Performs infrastructure health checks and initialization.
    /// Validates database connectivity and performs necessary migrations.
    /// </summary>
    /// <param name="serviceProvider">The configured service provider</param>
    /// <returns>Task representing the async health check operation</returns>
    public static async Task<bool> PerformInfrastructureHealthCheckAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("HealthCheck");

            logger.LogInformation("Performing infrastructure health check...");

            // Check database connectivity
            var dbContext = scope.ServiceProvider.GetRequiredService<EduPlayKidsDbContext>();
            var canConnect = await dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogError("Database connectivity check failed");
                return false;
            }

            // Ensure database is created and migrations are applied
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Validate Unit of Work
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var healthCheck = await unitOfWork.PerformHealthCheckAsync();

            if (!(bool)(healthCheck["DatabaseConnectivity"] ?? false))
            {
                logger.LogError("Unit of Work health check failed");
                return false;
            }

            logger.LogInformation("Infrastructure health check completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Infrastructure health check failed: {ex.Message}");
            return false;
        }
    }
}