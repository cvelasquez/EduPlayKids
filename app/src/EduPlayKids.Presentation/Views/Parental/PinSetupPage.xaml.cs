using EduPlayKids.App.ViewModels.Parental;

namespace EduPlayKids.App.Views.Parental;

/// <summary>
/// Code-behind for PIN setup page.
/// Handles PIN digit input navigation and validation for parental controls setup.
/// </summary>
public partial class PinSetupPage : ContentPage
{
    private readonly PinSetupViewModel _viewModel;
    private readonly Entry[] _pinEntries;
    private readonly Entry[] _confirmPinEntries;

    public PinSetupPage(PinSetupViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialize entry arrays for easier management
        _pinEntries = new[] { PinDigit1, PinDigit2, PinDigit3, PinDigit4 };
        _confirmPinEntries = new[] { ConfirmPinDigit1, ConfirmPinDigit2, ConfirmPinDigit3, ConfirmPinDigit4 };

        // Set up entry behavior
        SetupPinEntries();
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

            entry.Focused += (s, e) => entry.SelectAll();
            entry.Unfocused += (s, e) => entry.CursorPosition = entry.Text?.Length ?? 0;
        }

        // Configure confirm PIN entries
        for (int i = 0; i < _confirmPinEntries.Length; i++)
        {
            var entry = _confirmPinEntries[i];
            var index = i;

            entry.Focused += (s, e) => entry.SelectAll();
            entry.Unfocused += (s, e) => entry.CursorPosition = entry.Text?.Length ?? 0;
        }

        // Focus first PIN entry when page appears
        Loaded += (s, e) => PinDigit1.Focus();
    }

    /// <summary>
    /// Handles PIN digit text changes and auto-advances to next field.
    /// </summary>
    private void OnPinDigitChanged(object sender, TextChangedEventArgs e)
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
                // Move to confirm PIN fields
                ConfirmPinDigit1.Focus();
            }
        }
    }

    /// <summary>
    /// Handles confirm PIN digit text changes and auto-advances to next field.
    /// </summary>
    private void OnConfirmPinDigitChanged(object sender, TextChangedEventArgs e)
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
        UpdateConfirmPinFromEntries();

        // Auto-advance to next field
        if (!string.IsNullOrEmpty(text))
        {
            var currentIndex = Array.IndexOf(_confirmPinEntries, entry);
            if (currentIndex >= 0 && currentIndex < _confirmPinEntries.Length - 1)
            {
                _confirmPinEntries[currentIndex + 1].Focus();
            }
        }
    }

    /// <summary>
    /// Updates the view model PIN property from entry fields.
    /// </summary>
    private void UpdatePinFromEntries()
    {
        var pin = string.Concat(_pinEntries.Select(e => e.Text ?? ""));
        _viewModel.Pin = pin.PadRight(4, ' ').Substring(0, 4).Replace(' ', '\0');
    }

    /// <summary>
    /// Updates the view model confirm PIN property from entry fields.
    /// </summary>
    private void UpdateConfirmPinFromEntries()
    {
        var confirmPin = string.Concat(_confirmPinEntries.Select(e => e.Text ?? ""));
        _viewModel.ConfirmPin = confirmPin.PadRight(4, ' ').Substring(0, 4).Replace(' ', '\0');
    }

    /// <summary>
    /// Handles hardware back button to prevent accidental exits.
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        // Show confirmation dialog for back navigation
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await DisplayAlert(
                "Cancel Setup",
                "Are you sure you want to cancel PIN setup? Parental controls will remain disabled.",
                "Yes, Cancel",
                "Continue Setup");

            if (result)
            {
                await Shell.Current.GoToAsync("..");
            }
        });

        return true; // Prevent default back behavior
    }
}