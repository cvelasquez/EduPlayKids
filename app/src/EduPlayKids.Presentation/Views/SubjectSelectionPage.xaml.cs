using EduPlayKids.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.Views;

/// <summary>
/// Subject selection page where children choose what they want to learn.
/// Displays age-appropriate subjects with engaging visuals and descriptions.
/// </summary>
[QueryProperty(nameof(ChildId), "childId")]
[QueryProperty(nameof(ChildAge), "childAge")]
[QueryProperty(nameof(ChildName), "childName")]
public partial class SubjectSelectionPage : ContentPage
{
    private readonly SubjectSelectionViewModel _viewModel;
    private readonly ILogger<SubjectSelectionPage> _logger;

    public SubjectSelectionPage(SubjectSelectionViewModel viewModel, ILogger<SubjectSelectionPage> logger)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        BindingContext = _viewModel;
    }

    /// <summary>
    /// Child ID passed from navigation parameters.
    /// </summary>
    public string ChildId
    {
        set
        {
            if (int.TryParse(value, out var childId))
            {
                _viewModel.ChildId = childId;
                _logger.LogDebug("Child ID set to {ChildId}", childId);
            }
        }
    }

    /// <summary>
    /// Child age passed from navigation parameters.
    /// </summary>
    public string ChildAge
    {
        set
        {
            if (int.TryParse(value, out var childAge))
            {
                _viewModel.ChildAge = childAge;
                _logger.LogDebug("Child age set to {ChildAge}", childAge);
            }
        }
    }

    /// <summary>
    /// Child name passed from navigation parameters.
    /// </summary>
    public string ChildName
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _viewModel.ChildName = Uri.UnescapeDataString(value);
                _logger.LogDebug("Child name set to {ChildName}", _viewModel.ChildName);
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _logger.LogInformation("Subject selection page appearing for child {ChildId}, age {ChildAge}, name {ChildName}",
            _viewModel.ChildId, _viewModel.ChildAge, _viewModel.ChildName);

        await _viewModel.OnAppearingAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.OnDisappearingAsync();
    }
}