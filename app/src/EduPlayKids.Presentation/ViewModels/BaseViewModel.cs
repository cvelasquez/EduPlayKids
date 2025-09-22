using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// Base ViewModel for all ViewModels in the EduPlayKids application.
/// Provides common functionality for property change notification, loading states,
/// and child-safe UI patterns.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    protected readonly ILogger _logger;
    private bool _isBusy;
    private string _title = string.Empty;
    private string _busyText = "Loading...";

    /// <summary>
    /// Initializes a new instance of the BaseViewModel class.
    /// </summary>
    /// <param name="logger">Logger for debugging and error tracking.</param>
    protected BaseViewModel(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is busy performing an operation.
    /// Used to show loading indicators and prevent user interaction during async operations.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
                OnBusyStateChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the ViewModel is not busy.
    /// Useful for enabling/disabling UI elements.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Gets or sets the title of the current page or section.
    /// Used for page titles and navigation breadcrumbs.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Gets or sets the text to display during loading operations.
    /// Should be child-friendly and encouraging.
    /// </summary>
    public string BusyText
    {
        get => _busyText;
        set => SetProperty(ref _busyText, value);
    }

    /// <summary>
    /// Event that is raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets a property value and raises PropertyChanged event if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="backingStore">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Name of the property (automatically provided).</param>
    /// <returns>True if the value was changed, false otherwise.</returns>
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property (automatically provided).</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the busy state changes.
    /// Override in derived classes to handle busy state changes.
    /// </summary>
    protected virtual void OnBusyStateChanged()
    {
        // Override in derived classes for custom busy state handling
    }

    /// <summary>
    /// Executes an async operation with automatic busy state management.
    /// Shows loading indicator during execution and handles errors gracefully.
    /// </summary>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="busyText">Optional text to show during the operation.</param>
    /// <param name="showErrorDialog">Whether to show error dialogs to the user.</param>
    /// <returns>A task representing the async operation.</returns>
    protected async Task ExecuteAsync(Func<Task> operation, string? busyText = null, bool showErrorDialog = true)
    {
        if (IsBusy)
        {
            _logger.LogWarning("Attempted to execute operation while already busy");
            return;
        }

        try
        {
            if (!string.IsNullOrEmpty(busyText))
                BusyText = busyText;

            IsBusy = true;
            await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing async operation");

            if (showErrorDialog)
            {
                await ShowErrorAsync("Oops! Something went wrong. Please try again.", ex.Message);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes an async operation that returns a result with automatic busy state management.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="busyText">Optional text to show during the operation.</param>
    /// <param name="showErrorDialog">Whether to show error dialogs to the user.</param>
    /// <returns>The result of the operation, or default(T) if an error occurred.</returns>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string? busyText = null, bool showErrorDialog = true)
    {
        if (IsBusy)
        {
            _logger.LogWarning("Attempted to execute operation while already busy");
            return default;
        }

        try
        {
            if (!string.IsNullOrEmpty(busyText))
                BusyText = busyText;

            IsBusy = true;
            return await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing async operation");

            if (showErrorDialog)
            {
                await ShowErrorAsync("Oops! Something went wrong. Please try again.", ex.Message);
            }

            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Shows a child-friendly error message to the user.
    /// Override in derived classes to customize error display.
    /// </summary>
    /// <param name="title">The title of the error message.</param>
    /// <param name="message">The detailed error message.</param>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task ShowErrorAsync(string title, string message)
    {
        // In a real implementation, this would show a child-friendly error dialog
        // For now, we'll use the built-in DisplayAlert
        if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }

    /// <summary>
    /// Shows a child-friendly confirmation dialog.
    /// </summary>
    /// <param name="title">The title of the confirmation dialog.</param>
    /// <param name="message">The confirmation message.</param>
    /// <param name="accept">Text for the accept button (default: "Yes").</param>
    /// <param name="cancel">Text for the cancel button (default: "No").</param>
    /// <returns>True if the user accepted, false otherwise.</returns>
    protected virtual async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
        {
            return await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        return false;
    }

    /// <summary>
    /// Called when the ViewModel is appearing (page is being navigated to).
    /// Override in derived classes to load data or refresh the page.
    /// </summary>
    public virtual Task OnAppearingAsync()
    {
        _logger.LogDebug("{ViewModelType} appearing", GetType().Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the ViewModel is disappearing (page is being navigated away from).
    /// Override in derived classes to save state or clean up resources.
    /// </summary>
    public virtual Task OnDisappearingAsync()
    {
        _logger.LogDebug("{ViewModelType} disappearing", GetType().Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Validates the current state of the ViewModel.
    /// Override in derived classes to implement validation logic.
    /// </summary>
    /// <returns>True if the state is valid, false otherwise.</returns>
    public virtual bool Validate()
    {
        return true;
    }

    /// <summary>
    /// Resets the ViewModel to its initial state.
    /// Override in derived classes to implement reset logic.
    /// </summary>
    public virtual void Reset()
    {
        Title = string.Empty;
        IsBusy = false;
        BusyText = "Loading...";
    }
}