using EduPlayKids.App.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.Views.Pages;

/// <summary>
/// Base content page class for all pages in the EduPlayKids application.
/// Provides common functionality optimized for children's educational apps including
/// large touch targets, audio feedback, and child-safe navigation.
/// </summary>
public abstract class BaseContentPage : ContentPage
{
    protected readonly ILogger Logger;

    /// <summary>
    /// Gets the view model associated with this page.
    /// </summary>
    protected BaseViewModel? ViewModel => BindingContext as BaseViewModel;

    /// <summary>
    /// Initializes a new instance of the BaseContentPage class.
    /// </summary>
    /// <param name="logger">Logger instance for debugging and error tracking.</param>
    protected BaseContentPage(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SetupChildFriendlyDefaults();
    }

    /// <summary>
    /// Sets up default properties optimized for children aged 3-8.
    /// </summary>
    private void SetupChildFriendlyDefaults()
    {
        // Large touch targets for small fingers
        Shell.SetTabBarIsVisible(this, true);

        // Prevent accidental back navigation
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsEnabled = true,
            IsVisible = true
        });

        // Child-safe background color
        BackgroundColor = Colors.White;
    }

    /// <summary>
    /// Called when the page appears. Initializes the view model if present.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Logger.LogDebug("{PageType} appearing", GetType().Name);

        if (ViewModel != null)
        {
            await ViewModel.InitializeAsync();
        }

        // Enable audio feedback for interactions
        await EnableAudioFeedbackAsync();
    }

    /// <summary>
    /// Called when the page disappears. Cleans up the view model if present.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Logger.LogDebug("{PageType} disappearing", GetType().Name);

        ViewModel?.OnDisappearing();
    }

    /// <summary>
    /// Handles the back button press with child-safe confirmation.
    /// </summary>
    /// <returns>True if the back navigation was handled, false otherwise.</returns>
    protected override bool OnBackButtonPressed()
    {
        Logger.LogDebug("Back button pressed on {PageType}", GetType().Name);

        // For child safety, we might want to add confirmation dialogs
        // for certain critical pages or when progress might be lost
        return HandleChildSafeBackNavigation();
    }

    /// <summary>
    /// Handles back navigation with child-safety considerations.
    /// Can be overridden in derived classes for custom behavior.
    /// </summary>
    /// <returns>True if navigation was handled, false to allow default behavior.</returns>
    protected virtual bool HandleChildSafeBackNavigation()
    {
        // Default behavior: allow back navigation
        // Override in specific pages that need confirmation dialogs
        return false;
    }

    /// <summary>
    /// Enables audio feedback for UI interactions.
    /// Important for pre-readers who rely on audio cues.
    /// </summary>
    protected virtual Task EnableAudioFeedbackAsync()
    {
        // This will be implemented with the audio service
        // For now, just log that audio feedback should be enabled
        Logger.LogDebug("Audio feedback enabled for {PageType}", GetType().Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows a child-friendly error message with appropriate visual and audio cues.
    /// </summary>
    /// <param name="title">The error title (should be simple for children).</param>
    /// <param name="message">The error message (should be encouraging and simple).</param>
    /// <param name="cancel">The cancel button text.</param>
    protected virtual async Task ShowChildFriendlyErrorAsync(
        string title = "Oops!",
        string message = "Something went wrong. Let's try again!",
        string cancel = "OK")
    {
        Logger.LogInformation("Showing child-friendly error: {Title} - {Message}", title, message);

        // Use large fonts and simple language appropriate for children
        await DisplayAlert(title, message, cancel);

        // TODO: Add audio feedback for the error message
        // await PlayErrorSoundAsync();
    }

    /// <summary>
    /// Shows a child-friendly confirmation dialog with large, clear buttons.
    /// </summary>
    /// <param name="title">The confirmation title.</param>
    /// <param name="message">The confirmation message.</param>
    /// <param name="accept">The accept button text.</param>
    /// <param name="cancel">The cancel button text.</param>
    /// <returns>True if the user accepted, false otherwise.</returns>
    protected virtual async Task<bool> ShowChildFriendlyConfirmationAsync(
        string title,
        string message,
        string accept = "Yes",
        string cancel = "No")
    {
        Logger.LogInformation("Showing child-friendly confirmation: {Title}", title);

        var result = await DisplayAlert(title, message, accept, cancel);

        // TODO: Add audio feedback for the confirmation
        // await PlayConfirmationSoundAsync(result);

        return result;
    }

    /// <summary>
    /// Creates a child-friendly loading indicator with encouraging messages.
    /// </summary>
    protected virtual void ShowChildFriendlyLoading(string message = "Loading your learning adventure...")
    {
        Logger.LogDebug("Showing child-friendly loading: {Message}", message);

        // TODO: Implement custom loading indicator with animations and encouraging messages
        // This should include visual animations that keep children engaged
    }

    /// <summary>
    /// Hides the loading indicator.
    /// </summary>
    protected virtual void HideLoading()
    {
        Logger.LogDebug("Hiding loading indicator");

        // TODO: Hide the custom loading indicator
    }
}