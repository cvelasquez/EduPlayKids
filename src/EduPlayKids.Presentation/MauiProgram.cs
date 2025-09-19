using EduPlayKids.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App;

/// <summary>
/// Main entry point for configuring the EduPlayKids MAUI application.
/// Sets up dependency injection, database context, and services optimized for children's educational apps.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Creates and configures the MAUI application with all required services.
	/// </summary>
	/// <returns>The configured MAUI application.</returns>
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				// Child-friendly fonts - Nunito will be added later per design system
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Configure logging for development and debugging
		ConfigureLogging(builder);

		// Configure database services
		ConfigureDatabase(builder);

		// Configure application services
		ConfigureApplicationServices(builder);

		// Configure presentation services (ViewModels, Pages)
		ConfigurePresentationServices(builder);

		return builder.Build();
	}

	/// <summary>
	/// Configures logging services for the application.
	/// </summary>
	/// <param name="builder">The MAUI app builder.</param>
	private static void ConfigureLogging(MauiAppBuilder builder)
	{
#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

		// Add structured logging for better debugging of educational activities
		builder.Logging.AddConsole();
	}

	/// <summary>
	/// Configures Entity Framework and database services.
	/// </summary>
	/// <param name="builder">The MAUI app builder.</param>
	private static void ConfigureDatabase(MauiAppBuilder builder)
	{
		// Configure SQLite database for offline-first educational content
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "eduplaykids.db");

		builder.Services.AddDbContext<EduPlayKidsDbContext>(options =>
		{
			options.UseSqlite($"Data Source={dbPath}");

#if DEBUG
			options.EnableSensitiveDataLogging();
			options.EnableDetailedErrors();
#endif
		});

		// Register database initialization service
		builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
	}

	/// <summary>
	/// Configures application layer services.
	/// </summary>
	/// <param name="builder">The MAUI app builder.</param>
	private static void ConfigureApplicationServices(MauiAppBuilder builder)
	{
		// TODO: Register application services from EduPlayKids.Application
		// Examples:
		// - User management services
		// - Educational content services
		// - Progress tracking services
		// - Analytics services (COPPA-compliant)

		// Localization services for Spanish/English support
		builder.Services.AddLocalization();
	}

	/// <summary>
	/// Configures presentation layer services (ViewModels and Pages).
	/// </summary>
	/// <param name="builder">The MAUI app builder.</param>
	private static void ConfigurePresentationServices(MauiAppBuilder builder)
	{
		// TODO: Register ViewModels and Pages as they are created
		// Navigation service for child-safe navigation
		builder.Services.AddSingleton<INavigationService, NavigationService>();
	}
}

// Placeholder service interfaces and implementations
// TODO: Move these to appropriate projects as they are implemented

/// <summary>
/// Service for database initialization and seeding.
/// </summary>
public interface IDatabaseInitializationService
{
	Task InitializeAsync();
	Task SeedDataAsync();
}

/// <summary>
/// Service for child-safe navigation between pages.
/// </summary>
public interface INavigationService
{
	Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
	Task GoBackAsync();
	Task GoToRootAsync();
}

public class DatabaseInitializationService : IDatabaseInitializationService
{
	private readonly EduPlayKidsDbContext _context;
	private readonly ILogger<DatabaseInitializationService> _logger;

	public DatabaseInitializationService(EduPlayKidsDbContext context, ILogger<DatabaseInitializationService> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task InitializeAsync()
	{
		await _context.EnsureDatabaseAsync();
	}

	public async Task SeedDataAsync()
	{
		await _context.SeedInitialDataAsync();
	}
}

public class NavigationService : INavigationService
{
	public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
	{
		await Shell.Current.GoToAsync(route, parameters);
	}

	public async Task GoBackAsync()
	{
		await Shell.Current.GoToAsync("..");
	}

	public async Task GoToRootAsync()
	{
		await Shell.Current.GoToAsync("//");
	}
}
