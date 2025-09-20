using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels.Base;

/// <summary>
/// Base view model class for all ViewModels in the EduPlayKids application.
/// Provides common functionality like property change notifications, loading states,
/// and error handling optimized for children's educational apps.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly ILogger Logger;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    /// <summary>
    /// Initializes a new instance of the BaseViewModel class.
    /// </summary>
    /// <param name="logger">Logger instance for debugging and error tracking.</param>
    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sets the busy state and optionally shows a loading indicator.
    /// Prevents multiple simultaneous operations that could confuse young users.
    /// </summary>
    /// <param name="isBusy">Whether the view model is busy.</param>
    /// <param name="showLoading">Whether to show loading indicator.</param>
    protected virtual void SetBusyState(bool isBusy, bool showLoading = true)
    {
        IsBusy = isBusy;
        IsLoading = isBusy && showLoading;

        if (isBusy)
        {
            ClearError();
        }
    }

    /// <summary>
    /// Sets an error message and logs the error.
    /// Error messages should be child-friendly when displayed to users.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    /// <param name="exception">Optional exception for logging.</param>
    protected virtual void SetError(string message, Exception? exception = null)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);

        if (exception != null)
        {
            Logger.LogError(exception, "Error in {ViewModelType}: {Message}", GetType().Name, message);
        }
        else
        {
            Logger.LogWarning("Error in {ViewModelType}: {Message}", GetType().Name, message);
        }
    }

    /// <summary>
    /// Clears any current error state.
    /// </summary>
    protected virtual void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    /// <summary>
    /// Executes an async operation safely with error handling and busy state management.
    /// Ideal for child-safe operations that shouldn't crash the app.
    /// </summary>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="showLoading">Whether to show loading indicator.</param>
    /// <param name="errorMessage">Custom error message if operation fails.</param>
    protected async Task ExecuteSafelyAsync(
        Func<Task> operation,
        bool showLoading = true,
        string? errorMessage = null)
    {
        if (IsBusy) return;

        try
        {
            SetBusyState(true, showLoading);
            await operation();
        }
        catch (Exception ex)
        {
            var message = errorMessage ?? "Something went wrong. Please try again.";
            SetError(message, ex);
        }
        finally
        {
            SetBusyState(false);
        }
    }

    /// <summary>
    /// Executes an async operation safely with return value and error handling.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="showLoading">Whether to show loading indicator.</param>
    /// <param name="errorMessage">Custom error message if operation fails.</param>
    /// <returns>The result of the operation, or default(T) if an error occurred.</returns>
    protected async Task<T?> ExecuteSafelyAsync<T>(
        Func<Task<T>> operation,
        bool showLoading = true,
        string? errorMessage = null)
    {
        if (IsBusy) return default;

        try
        {
            SetBusyState(true, showLoading);
            return await operation();
        }
        catch (Exception ex)
        {
            var message = errorMessage ?? "Something went wrong. Please try again.";
            SetError(message, ex);
            return default;
        }
        finally
        {
            SetBusyState(false);
        }
    }

    /// <summary>
    /// Command to clear the current error state.
    /// Useful for child-friendly retry mechanisms.
    /// </summary>
    [RelayCommand]
    protected virtual void ClearErrorCommand()
    {
        ClearError();
    }

    /// <summary>
    /// Virtual method called when the view model is initialized.
    /// Override in derived classes for initialization logic.
    /// </summary>
    public virtual Task InitializeAsync()
    {
        Logger.LogDebug("{ViewModelType} initialized", GetType().Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Virtual method called when the view model is being disposed.
    /// Override in derived classes for cleanup logic.
    /// </summary>
    public virtual void OnDisappearing()
    {
        Logger.LogDebug("{ViewModelType} disappearing", GetType().Name);
    }
}