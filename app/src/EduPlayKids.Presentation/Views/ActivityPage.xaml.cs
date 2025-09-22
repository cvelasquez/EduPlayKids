using EduPlayKids.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.Views;

/// <summary>
/// Activity page where children complete educational exercises.
/// This is the core learning experience of the EduPlayKids app.
/// </summary>
[QueryProperty(nameof(ChildId), "childId")]
[QueryProperty(nameof(ActivityId), "activityId")]
[QueryProperty(nameof(SubjectName), "subjectName")]
public partial class ActivityPage : ContentPage
{
    private readonly ActivityViewModel _viewModel;
    private readonly ILogger<ActivityPage> _logger;

    public ActivityPage(ActivityViewModel viewModel, ILogger<ActivityPage> logger)
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
                _logger.LogDebug("Child ID set to {ChildId}", childId);
                _viewModel.ChildId = childId;
                UpdateViewModelIfReady();
            }
        }
    }

    /// <summary>
    /// Activity ID passed from navigation parameters.
    /// </summary>
    public string ActivityId
    {
        set
        {
            if (int.TryParse(value, out var activityId))
            {
                _logger.LogDebug("Activity ID set to {ActivityId}", activityId);
                _viewModel.ActivityId = activityId;
                UpdateViewModelIfReady();
            }
        }
    }

    /// <summary>
    /// Subject name passed from navigation parameters.
    /// </summary>
    public string SubjectName
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var subjectName = Uri.UnescapeDataString(value);
                _logger.LogDebug("Subject name set to {SubjectName}", subjectName);
                _viewModel.SubjectName = subjectName;
                UpdateViewModelIfReady();
            }
        }
    }

    /// <summary>
    /// Updates the ViewModel when all required parameters are available.
    /// </summary>
    private void UpdateViewModelIfReady()
    {
        if (_viewModel.ChildId > 0 && _viewModel.ActivityId > 0 && !string.IsNullOrEmpty(_viewModel.SubjectName))
        {
            _viewModel.Initialize(_viewModel.ChildId, _viewModel.ActivityId, _viewModel.SubjectName);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _logger.LogInformation("Activity page appearing: Child={ChildId}, Activity={ActivityId}, Subject={SubjectName}",
            _viewModel.ChildId, _viewModel.ActivityId, _viewModel.SubjectName);

        await _viewModel.OnAppearingAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.OnDisappearingAsync();
    }
}