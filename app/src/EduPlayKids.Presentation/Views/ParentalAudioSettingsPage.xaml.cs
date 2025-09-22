using EduPlayKids.App.ViewModels;

namespace EduPlayKids.App.Views;

/// <summary>
/// Parental audio settings page for configuring child-safe audio preferences.
/// Provides comprehensive controls for volume management, language selection, and safety features.
/// </summary>
public partial class ParentalAudioSettingsPage : ContentPage
{
    public ParentalAudioSettingsPage(ParentalAudioSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}