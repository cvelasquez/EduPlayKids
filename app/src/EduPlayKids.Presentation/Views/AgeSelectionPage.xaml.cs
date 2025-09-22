using EduPlayKids.App.ViewModels;

namespace EduPlayKids.App.Views;

/// <summary>
/// Age selection page for creating child profiles.
/// Allows parents to set their child's age which determines curriculum and difficulty level.
/// </summary>
public partial class AgeSelectionPage : ContentPage
{
    private readonly AgeSelectionViewModel _viewModel;

    public AgeSelectionPage(AgeSelectionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearingAsync();

        // Focus on the name entry when the page appears
        await Task.Delay(500); // Small delay for smooth animation
        ChildNameEntry.Focus();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.OnDisappearingAsync();
    }
}