using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels.Parental;

/// <summary>
/// ViewModel for parental PIN verification functionality.
/// Handles PIN verification, lockout management, and access control for parental features.
/// </summary>
public partial class PinVerificationViewModel : ObservableObject
{
    private readonly IParentalPinService _pinService;
    private readonly ILogger<PinVerificationViewModel> _logger;
    private Timer? _lockoutTimer;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _canVerifyPin = false;

    [ObservableProperty]
    private bool _isLocked = false;

    [ObservableProperty]
    private string _lockoutMessage = string.Empty;

    [ObservableProperty]
    private bool _shouldClearPin = false;

    /// <summary>
    /// Event fired when PIN verification is successful.
    /// </summary>
    public event EventHandler? PinVerificationSuccessful;

    public PinVerificationViewModel(IParentalPinService pinService, ILogger<PinVerificationViewModel> logger)
    {
        _pinService = pinService ?? throw new ArgumentNullException(nameof(pinService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Watch for property changes to update validation
        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Initializes the view model by checking lockout status.
    /// </summary>
    public async Task InitializeAsync()
    {
        await CheckLockoutStatusAsync();
    }

    /// <summary>
    /// Verifies the entered PIN.
    /// </summary>
    [RelayCommand]
    private async Task VerifyPinAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            _logger.LogInformation("Starting PIN verification");

            // Check if PIN entry is locked
            if (await _pinService.IsPinEntryLockedAsync())
            {
                await UpdateLockoutStatus();
                return;
            }

            // Verify PIN
            var result = await _pinService.VerifyPinAsync(Pin);

            if (result.IsSuccess)
            {
                _logger.LogInformation("PIN verification successful");

                // Clear PIN for security
                Pin = string.Empty;
                ShouldClearPin = true;

                // Fire success event
                PinVerificationSuccessful?.Invoke(this, EventArgs.Empty);

                // Navigate to parental dashboard
                await NavigateToParentalDashboard();
            }
            else
            {
                _logger.LogWarning("PIN verification failed: {Error}", result.Error);

                // Clear PIN and show error
                Pin = string.Empty;
                ShouldClearPin = true;
                ShowError(result.Error ?? "Invalid PIN. Please try again.");

                // Check if now locked
                await UpdateLockoutStatus();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PIN verification");
            ShowError("An unexpected error occurred. Please try again.");

            // Clear PIN for security
            Pin = string.Empty;
            ShouldClearPin = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handles forgot PIN functionality.
    /// </summary>
    [RelayCommand]
    private async Task ForgotPinAsync()
    {
        try
        {
            _logger.LogInformation("User requested PIN reset");

            var securityQuestion = await _pinService.GetSecurityQuestionAsync();
            if (string.IsNullOrEmpty(securityQuestion))
            {
                await ShowAlert("No Security Question",
                    "No security question is configured. Please contact support for assistance.");
                return;
            }

            // Navigate to PIN reset page with security question
            await NavigateToPinReset(securityQuestion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling forgot PIN request");
            ShowError("Unable to process forgot PIN request. Please try again.");
        }
    }

    /// <summary>
    /// Cancels PIN verification and returns to child interface.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        try
        {
            _logger.LogInformation("PIN verification cancelled by user");

            // Clear PIN for security
            Pin = string.Empty;

            // Navigate back to child interface
            await NavigateBack();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PIN verification cancellation");
        }
    }

    /// <summary>
    /// Checks and updates the lockout status.
    /// </summary>
    public async Task CheckLockoutStatusAsync()
    {
        try
        {
            await UpdateLockoutStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking lockout status");
        }
    }

    /// <summary>
    /// Updates the lockout status and message.
    /// </summary>
    private async Task UpdateLockoutStatus()
    {
        var isLocked = await _pinService.IsPinEntryLockedAsync();

        if (isLocked)
        {
            var remainingSeconds = await _pinService.GetLockoutRemainingSecondsAsync();
            IsLocked = true;
            UpdateLockoutMessage(remainingSeconds);

            // Start countdown timer
            StartLockoutTimer(remainingSeconds);
        }
        else
        {
            IsLocked = false;
            LockoutMessage = string.Empty;
            StopLockoutTimer();
        }
    }

    /// <summary>
    /// Updates the lockout message with remaining time.
    /// </summary>
    private void UpdateLockoutMessage(int remainingSeconds)
    {
        if (remainingSeconds <= 0)
        {
            LockoutMessage = string.Empty;
            return;
        }

        var minutes = remainingSeconds / 60;
        var seconds = remainingSeconds % 60;

        if (minutes > 0)
        {
            LockoutMessage = $"Too many failed attempts. Try again in {minutes}m {seconds}s.";
        }
        else
        {
            LockoutMessage = $"Too many failed attempts. Try again in {seconds} seconds.";
        }
    }

    /// <summary>
    /// Starts the lockout countdown timer.
    /// </summary>
    private void StartLockoutTimer(int remainingSeconds)
    {
        StopLockoutTimer();

        _lockoutTimer = new Timer(async _ =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await UpdateLockoutStatus();
            });
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Stops the lockout countdown timer.
    /// </summary>
    private void StopLockoutTimer()
    {
        _lockoutTimer?.Dispose();
        _lockoutTimer = null;
    }

    /// <summary>
    /// Updates the CanVerifyPin property based on input validation.
    /// </summary>
    private void UpdateCanVerifyPin()
    {
        CanVerifyPin = !IsLoading &&
                       !IsLocked &&
                       !string.IsNullOrWhiteSpace(Pin) &&
                       Pin.Length == 4;
    }

    /// <summary>
    /// Shows an error message to the user.
    /// </summary>
    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        _logger.LogWarning("Showing error to user: {Message}", message);
    }

    /// <summary>
    /// Clears any existing error message.
    /// </summary>
    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    /// <summary>
    /// Shows an alert dialog to the user.
    /// </summary>
    private async Task ShowAlert(string title, string message)
    {
        if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }

    /// <summary>
    /// Navigates to the parental dashboard.
    /// </summary>
    private async Task NavigateToParentalDashboard()
    {
        await Shell.Current.GoToAsync("//parentaldashboard");
    }

    /// <summary>
    /// Navigates to the PIN reset page.
    /// </summary>
    private async Task NavigateToPinReset(string securityQuestion)
    {
        var parameters = new Dictionary<string, object>
        {
            { "SecurityQuestion", securityQuestion }
        };

        await Shell.Current.GoToAsync("pinreset", parameters);
    }

    /// <summary>
    /// Navigates back to the previous page.
    /// </summary>
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Handles property changes to update validation.
    /// </summary>
    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Pin):
            case nameof(IsLoading):
            case nameof(IsLocked):
                UpdateCanVerifyPin();
                // Clear errors when user starts typing
                if (HasError && e.PropertyName == nameof(Pin))
                {
                    ClearError();
                }
                break;
        }
    }

    /// <summary>
    /// Cleanup when view model is disposed.
    /// </summary>
    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // Dispose timer if view model is being cleaned up
        if (e.PropertyName == null)
        {
            StopLockoutTimer();
        }
    }
}