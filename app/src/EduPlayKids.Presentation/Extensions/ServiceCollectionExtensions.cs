using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Infrastructure.Data.Repositories;
using EduPlayKids.Infrastructure.Services.Audio;
using EduPlayKids.Infrastructure.Services;

namespace EduPlayKids.App.Extensions;

/// <summary>
/// Extension methods for configuring services in the EduPlayKids application.
/// Provides centralized service registration for audio, repositories, and application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all repository services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register Unit of Work pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register all repositories
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
        services.AddScoped<IParentalPinRepository, ParentalPinRepository>();

        return services;
    }

    /// <summary>
    /// Registers parental control services for secure access and child safety.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddParentalControlServices(this IServiceCollection services)
    {
        // Register PIN management service
        services.AddScoped<IParentalPinService, ParentalPinService>();

        // TODO: Add other parental control services here as they are implemented
        // - IParentalDashboardService
        // - IChildSafetyService
        // - IScreenTimeService

        return services;
    }

    /// <summary>
    /// Registers audio services optimized for children's educational applications.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="isDevelopment">Whether the app is running in development mode</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddAudioServices(this IServiceCollection services, bool isDevelopment = false)
    {
        // Register audio configuration based on environment
        services.AddSingleton<AudioConfiguration>(provider =>
        {
            return isDevelopment ? AudioConfiguration.Debug : AudioConfiguration.Default;
        });

        // Register main audio service
        services.AddSingleton<IAudioService, AudioService>();

        // Register audio resource managers
        services.AddSingleton<AudioResourceManager>();
        services.AddSingleton<BilingualAudioManager>();

        return services;
    }

    /// <summary>
    /// Registers ViewModels for the EduPlayKids application.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        // Register main ViewModels
        services.AddTransient<EduPlayKids.App.ViewModels.AgeSelectionViewModel>();
        services.AddTransient<EduPlayKids.App.ViewModels.SubjectSelectionViewModel>();
        services.AddTransient<EduPlayKids.App.ViewModels.ActivityViewModel>();

        // Register audio-specific ViewModels
        services.AddTransient<EduPlayKids.App.ViewModels.ParentalAudioSettingsViewModel>();

        // Register parental control ViewModels
        services.AddTransient<EduPlayKids.App.ViewModels.Parental.PinSetupViewModel>();
        services.AddTransient<EduPlayKids.App.ViewModels.Parental.PinVerificationViewModel>();
        services.AddTransient<EduPlayKids.App.ViewModels.Parental.ParentalDashboardViewModel>();

        return services;
    }

    /// <summary>
    /// Registers Pages for the EduPlayKids application.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPages(this IServiceCollection services)
    {
        // Register main Pages
        services.AddTransient<EduPlayKids.App.Views.AgeSelectionPage>();
        services.AddTransient<EduPlayKids.App.Views.SubjectSelectionPage>();
        services.AddTransient<EduPlayKids.App.Views.ActivityPage>();

        // Register audio-specific Pages
        services.AddTransient<EduPlayKids.App.Views.ParentalAudioSettingsPage>();

        // Register parental control Pages
        services.AddTransient<EduPlayKids.App.Views.Parental.PinSetupPage>();
        services.AddTransient<EduPlayKids.App.Views.Parental.PinVerificationPage>();
        services.AddTransient<EduPlayKids.App.Views.Parental.ParentalDashboardPage>();

        return services;
    }

    /// <summary>
    /// Registers navigation services for child-safe navigation.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddNavigationServices(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<EduPlayKids.App.Services.IChildSafeNavigationService, EduPlayKids.App.Services.ChildSafeNavigationService>();

        return services;
    }

    /// <summary>
    /// Registers localization services for Spanish/English support.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
    {
        services.AddLocalization();

        return services;
    }

    /// <summary>
    /// Registers all application services in the correct order.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="isDevelopment">Whether the app is running in development mode</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddEduPlayKidsServices(this IServiceCollection services, bool isDevelopment = false)
    {
        return services
            .AddRepositories()
            .AddParentalControlServices()
            .AddAudioServices(isDevelopment)
            .AddNavigationServices()
            .AddLocalizationServices()
            .AddViewModels()
            .AddPages();
    }
}

// Placeholder interfaces that will be moved to appropriate projects
// TODO: Move these to EduPlayKids.Application.Interfaces when that project is updated

/// <summary>
/// Service for child-safe navigation between pages.
/// </summary>
public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task GoBackAsync();
    Task GoToRootAsync();
}

/// <summary>
/// Navigation service implementation using MAUI Shell.
/// </summary>
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