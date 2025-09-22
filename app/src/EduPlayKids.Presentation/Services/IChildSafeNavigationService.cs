namespace EduPlayKids.App.Services;

/// <summary>
/// Navigation service interface designed specifically for child-safe navigation.
/// Provides controlled navigation with parental controls and child-friendly patterns.
/// </summary>
public interface IChildSafeNavigationService
{
    /// <summary>
    /// Navigates to a page using the specified route.
    /// All navigation is logged for parental oversight.
    /// </summary>
    /// <param name="route">The route to navigate to.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <returns>A task representing the navigation operation.</returns>
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Navigates back to the previous page.
    /// Ensures children can always return to a safe state.
    /// </summary>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoBackAsync();

    /// <summary>
    /// Navigates to the root/home page.
    /// Provides a quick way for children to return to the main menu.
    /// </summary>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoToHomeAsync();

    /// <summary>
    /// Navigates to the age selection page for setting up a child profile.
    /// </summary>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoToAgeSelectionAsync();

    /// <summary>
    /// Navigates to the subject selection page.
    /// </summary>
    /// <param name="childId">The child's identifier.</param>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoToSubjectSelectionAsync(int childId);

    /// <summary>
    /// Navigates to an activity page.
    /// </summary>
    /// <param name="childId">The child's identifier.</param>
    /// <param name="activityId">The activity identifier.</param>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoToActivityAsync(int childId, int activityId);

    /// <summary>
    /// Navigates to the parental control section.
    /// Requires PIN verification for access.
    /// </summary>
    /// <returns>A task representing the navigation operation.</returns>
    Task GoToParentalControlsAsync();

    /// <summary>
    /// Shows a child-friendly loading screen during navigation.
    /// </summary>
    /// <param name="message">The loading message to display.</param>
    /// <returns>A task representing the operation.</returns>
    Task ShowLoadingAsync(string message = "Loading...");

    /// <summary>
    /// Hides the loading screen.
    /// </summary>
    /// <returns>A task representing the operation.</returns>
    Task HideLoadingAsync();

    /// <summary>
    /// Shows a child-friendly error message.
    /// </summary>
    /// <param name="title">The error title.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A task representing the operation.</returns>
    Task ShowErrorAsync(string title, string message);

    /// <summary>
    /// Shows a child-friendly celebration for achievements.
    /// </summary>
    /// <param name="title">The celebration title.</param>
    /// <param name="message">The celebration message.</param>
    /// <returns>A task representing the operation.</returns>
    Task ShowCelebrationAsync(string title, string message);
}