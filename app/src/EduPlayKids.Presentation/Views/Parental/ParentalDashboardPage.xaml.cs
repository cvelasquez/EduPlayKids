using EduPlayKids.App.ViewModels.Parental;

namespace EduPlayKids.App.Views.Parental;

/// <summary>
/// Code-behind for parental dashboard page.
/// Displays comprehensive child progress analytics and parental controls.
/// </summary>
public partial class ParentalDashboardPage : ContentPage
{
    private readonly ParentalDashboardViewModel _viewModel;

    public ParentalDashboardPage(ParentalDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Initialize data when page appears.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            // Log error and show user-friendly message
            await DisplayAlert("Error", "Unable to load dashboard data. Please try again.", "OK");
        }
    }

    /// <summary>
    /// Clean up resources when page disappears.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop any running timers or updates
        _viewModel.StopAutoRefresh();
    }

    /// <summary>
    /// Handle hardware back button to prevent accidental exits from parental area.
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        // Show confirmation dialog for back navigation
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await DisplayAlert(
                "Exit Parental Controls",
                "Are you sure you want to exit parental controls? You'll need to enter your PIN again to return.",
                "Yes, Exit",
                "Stay Here");

            if (result)
            {
                await Shell.Current.GoToAsync("//ageselection");
            }
        });

        return true; // Prevent default back behavior
    }
}