using EduPlayKids.App.ViewModels.Parental;

namespace EduPlayKids.App.Views.Parental;

/// <summary>
/// Code-behind for PIN verification page.
/// Handles PIN digit input and automatic verification for parental access.
/// </summary>
public partial class PinVerificationPage : ContentPage
{
    private readonly PinVerificationViewModel _viewModel;
    private readonly Entry[] _pinEntries;
    private readonly Timer? _lockoutTimer;

    public PinVerificationPage(PinVerificationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialize entry arrays for easier management
        _pinEntries = new[] { PinDigit1, PinDigit2, PinDigit3, PinDigit4 };

        // Set up entry behavior
        SetupPinEntries();

        // Subscribe to lockout changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <summary>
    /// Configures PIN entry behavior for smooth user experience.
    /// </summary>
    private void SetupPinEntries()
    {
        // Configure PIN entries
        for (int i = 0; i < _pinEntries.Length; i++)
        {
            var entry = _pinEntries[i];
            var index = i;

            entry.Focused += (s, e) =>
            {
                entry.CursorPosition = 0;
                entry.SelectionLength = entry.Text?.Length ?? 0;
            };
            entry.Unfocused += (s, e) => entry.CursorPosition = entry.Text?.Length ?? 0;
        }

        // Focus first PIN entry when page appears
        Loaded += async (s, e) =>
        {
            await Task.Delay(100); // Small delay to ensure page is fully loaded
            await CheckLockoutStatusAndFocus();
        };
    }

    /// <summary>
    /// Checks lockout status and focuses appropriate field.
    /// </summary>
    private async Task CheckLockoutStatusAndFocus()
    {
        await _viewModel.CheckLockoutStatusAsync();

        if (!_viewModel.IsLocked)
        {
            PinDigit1.Focus();
        }
    }

    /// <summary>
    /// Handles PIN digit text changes and auto-advances to next field.
    /// </summary>
    private async void OnPinDigitChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;

        var text = e.NewTextValue;

        // Ensure only digits are entered
        if (!string.IsNullOrEmpty(text) && !char.IsDigit(text.Last()))
        {
            entry.Text = e.OldTextValue;
            return;
        }

        // Update view model
        UpdatePinFromEntries();

        // Auto-advance to next field
        if (!string.IsNullOrEmpty(text))
        {
            var currentIndex = Array.IndexOf(_pinEntries, entry);
            if (currentIndex >= 0 && currentIndex < _pinEntries.Length - 1)
            {
                _pinEntries[currentIndex + 1].Focus();
            }
            else if (currentIndex == _pinEntries.Length - 1)
            {
                // Auto-verify when all digits are entered
                if (_viewModel.CanVerifyPin && !_viewModel.IsLoading)
                {
                    await _viewModel.VerifyPinCommand.ExecuteAsync(null);
                }
            }
        }
    }

    /// <summary>
    /// Updates the view model PIN property from entry fields.
    /// </summary>
    private void UpdatePinFromEntries()
    {
        var pin = string.Concat(_pinEntries.Select(e => e.Text ?? ""));
        _viewModel.Pin = pin;
    }

    /// <summary>
    /// Handles view model property changes for UI updates.
    /// </summary>
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PinVerificationViewModel.IsLocked):
                if (_viewModel.IsLocked)
                {
                    // Clear PIN entries when locked
                    ClearPinEntries();
                    // Unfocus all entries
                    foreach (var entry in _pinEntries)
                    {
                        entry.Unfocus();
                    }
                }
                else
                {
                    // Focus first entry when unlocked
                    PinDigit1.Focus();
                }
                break;

            case nameof(PinVerificationViewModel.ShouldClearPin):
                if (_viewModel.ShouldClearPin)
                {
                    ClearPinEntries();
                    PinDigit1.Focus();
                    _viewModel.ShouldClearPin = false;
                }
                break;
        }
    }

    /// <summary>
    /// Clears all PIN entry fields.
    /// </summary>
    private void ClearPinEntries()
    {
        foreach (var entry in _pinEntries)
        {
            entry.Text = string.Empty;
        }
        _viewModel.Pin = string.Empty;
    }

    /// <summary>
    /// Handles hardware back button to prevent accidental exits.
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        // Allow back navigation (will go to child interface)
        return false;
    }

    /// <summary>
    /// Clean up resources when page disappears.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Clear PIN for security
        ClearPinEntries();

        // Unsubscribe from events
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }
}