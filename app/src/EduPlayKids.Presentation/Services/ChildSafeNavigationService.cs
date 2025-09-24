using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.Services;

/// <summary>
/// Implementation of child-safe navigation service.
/// Provides controlled, logged, and child-friendly navigation throughout the app.
/// </summary>
public class ChildSafeNavigationService : IChildSafeNavigationService
{
    private readonly ILogger<ChildSafeNavigationService> _logger;

    public ChildSafeNavigationService(ILogger<ChildSafeNavigationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        try
        {
            _logger.LogInformation("Navigating to route: {Route}", route);

            if (parameters != null)
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }

            _logger.LogDebug("Navigation to {Route} completed successfully", route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to route: {Route}", route);
            await ShowErrorAsync("Navigation Error", "Sorry, we couldn't go to that page. Let's try again!");
        }
    }

    public async Task GoBackAsync()
    {
        try
        {
            _logger.LogInformation("Navigating back");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating back");
            // If back navigation fails, go to home as a safe fallback
            await GoToHomeAsync();
        }
    }

    public async Task NavigateBackAsync()
    {
        // Alias for GoBackAsync for compatibility
        await GoBackAsync();
    }

    public async Task GoToHomeAsync()
    {
        try
        {
            _logger.LogInformation("Navigating to home");
            await Shell.Current.GoToAsync("//home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to home");
            await ShowErrorAsync("Home Error", "Let's go back to the main menu!");
        }
    }

    public async Task GoToAgeSelectionAsync()
    {
        await NavigateToAsync("//ageselection");
    }

    public async Task GoToSubjectSelectionAsync(int childId)
    {
        var parameters = new Dictionary<string, object>
        {
            ["childId"] = childId
        };
        await NavigateToAsync("//subjects", parameters);
    }

    public async Task GoToActivityAsync(int childId, int activityId)
    {
        var parameters = new Dictionary<string, object>
        {
            ["childId"] = childId,
            ["activityId"] = activityId
        };
        await NavigateToAsync("//activity", parameters);
    }

    public async Task GoToParentalControlsAsync()
    {
        // TODO: Implement PIN verification before allowing access
        await NavigateToAsync("//parental");
    }

    public async Task ShowLoadingAsync(string message = "Loading...")
    {
        try
        {
            _logger.LogDebug("Showing loading screen: {Message}", message);

            // TODO: Implement custom loading overlay with child-friendly animations
            // For now, we'll use a simple approach
            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                // This is a placeholder - in a real implementation, we'd show a custom loading view
                // with child-friendly animations like spinning toys or characters
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing loading screen");
        }
    }

    public async Task HideLoadingAsync()
    {
        try
        {
            _logger.LogDebug("Hiding loading screen");

            // TODO: Hide custom loading overlay
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hiding loading screen");
        }
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        try
        {
            _logger.LogWarning("Showing error to user: {Title} - {Message}", title, message);

            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                // Show child-friendly error dialog
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    title,
                    message,
                    "OK! Let's try again! 😊");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing error dialog");
        }
    }

    public async Task ShowCelebrationAsync(string title, string message)
    {
        try
        {
            _logger.LogInformation("Showing celebration: {Title} - {Message}", title, message);

            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                // Show celebratory dialog with animations and sounds
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    $"🎉 {title} 🎉",
                    $"{message} 🌟",
                    "Awesome! 🚀");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing celebration dialog");
        }
    }
}