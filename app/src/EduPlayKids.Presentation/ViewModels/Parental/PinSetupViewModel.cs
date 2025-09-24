using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels.Parental;

/// <summary>
/// ViewModel for parental PIN setup functionality.
/// Handles PIN creation, validation, and security question setup for parental controls.
/// </summary>
public partial class PinSetupViewModel : ObservableObject
{
    private readonly IParentalPinService _pinService;
    private readonly ILogger<PinSetupViewModel> _logger;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _confirmPin = string.Empty;

    [ObservableProperty]
    private string _securityQuestion = string.Empty;

    [ObservableProperty]
    private string _securityAnswer = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _canSetupPin = false;

    public PinSetupViewModel(IParentalPinService pinService, ILogger<PinSetupViewModel> logger)
    {
        _pinService = pinService ?? throw new ArgumentNullException(nameof(pinService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Watch for property changes to update validation
        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Sets up the parental PIN with security question.
    /// </summary>
    [RelayCommand]
    private async Task SetupPinAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            _logger.LogInformation("Starting PIN setup process");

            // Validate inputs
            if (!ValidateInputs())
            {
                return;
            }

            // Attempt to set up PIN
            var result = await _pinService.SetupPinAsync(Pin, SecurityQuestion, SecurityAnswer);

            if (result.IsSuccess)
            {
                _logger.LogInformation("PIN setup completed successfully");

                // Show success message and navigate back
                await ShowSuccessMessage();
                await NavigateBack();
            }
            else
            {
                _logger.LogWarning("PIN setup failed: {Error}", result.Error);
                ShowError(result.Error ?? "Failed to setup PIN. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PIN setup");
            ShowError("An unexpected error occurred. Please try again.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Cancels PIN setup and returns to previous page.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        try
        {
            _logger.LogInformation("PIN setup cancelled by user");

            var confirmed = await ConfirmCancel();
            if (confirmed)
            {
                await NavigateBack();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PIN setup cancellation");
        }
    }

    /// <summary>
    /// Validates all input fields for PIN setup.
    /// </summary>
    private bool ValidateInputs()
    {
        // Check PIN completeness
        if (string.IsNullOrWhiteSpace(Pin) || Pin.Length != 4)
        {
            ShowError("Please enter a complete 4-digit PIN.");
            return false;
        }

        // Check PIN confirmation
        if (Pin != ConfirmPin)
        {
            ShowError("PIN and confirmation do not match.");
            return false;
        }

        // Validate PIN format using service
        var pinValidation = _pinService.ValidatePinFormat(Pin);
        if (!pinValidation.IsSuccess)
        {
            ShowError(pinValidation.Error ?? "Invalid PIN format.");
            return false;
        }

        // Check security question
        if (string.IsNullOrWhiteSpace(SecurityQuestion) || SecurityQuestion.Length < 10)
        {
            ShowError("Security question must be at least 10 characters long.");
            return false;
        }

        // Check security answer
        if (string.IsNullOrWhiteSpace(SecurityAnswer) || SecurityAnswer.Length < 3)
        {
            ShowError("Security answer must be at least 3 characters long.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the CanSetupPin property based on input validation.
    /// </summary>
    private void UpdateCanSetupPin()
    {
        CanSetupPin = !IsLoading &&
                      !string.IsNullOrWhiteSpace(Pin) &&
                      Pin.Length == 4 &&
                      !string.IsNullOrWhiteSpace(ConfirmPin) &&
                      ConfirmPin.Length == 4 &&
                      !string.IsNullOrWhiteSpace(SecurityQuestion) &&
                      SecurityQuestion.Length >= 10 &&
                      !string.IsNullOrWhiteSpace(SecurityAnswer) &&
                      SecurityAnswer.Length >= 3;
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
    /// Shows success message after PIN setup completion.
    /// </summary>
    private async Task ShowSuccessMessage()
    {
        if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                "PIN Setup Complete",
                "Your parental controls PIN has been set up successfully. You can now access parental controls and settings.",
                "OK");
        }
    }

    /// <summary>
    /// Confirms with user before cancelling PIN setup.
    /// </summary>
    private async Task<bool> ConfirmCancel()
    {
        if (Microsoft.Maui.Controls.Application.Current?.MainPage == null)
            return true;

        return await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
            "Cancel Setup",
            "Are you sure you want to cancel PIN setup? Parental controls will remain disabled.",
            "Yes, Cancel",
            "Continue Setup");
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
            case nameof(ConfirmPin):
            case nameof(SecurityQuestion):
            case nameof(SecurityAnswer):
            case nameof(IsLoading):
                UpdateCanSetupPin();
                // Clear errors when user starts typing
                if (HasError && e.PropertyName != nameof(IsLoading))
                {
                    ClearError();
                }
                break;
        }
    }

    /// <summary>
    /// Gets the PIN digit at the specified position for UI binding.
    /// </summary>
    public string GetPinDigit(int position)
    {
        if (position < 0 || position >= 4 || string.IsNullOrEmpty(Pin))
            return string.Empty;

        return position < Pin.Length ? Pin[position].ToString() : string.Empty;
    }

    /// <summary>
    /// Gets the confirm PIN digit at the specified position for UI binding.
    /// </summary>
    public string GetConfirmPinDigit(int position)
    {
        if (position < 0 || position >= 4 || string.IsNullOrEmpty(ConfirmPin))
            return string.Empty;

        return position < ConfirmPin.Length ? ConfirmPin[position].ToString() : string.Empty;
    }
}