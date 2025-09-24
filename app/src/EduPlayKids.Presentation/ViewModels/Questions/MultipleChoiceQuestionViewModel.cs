using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Services;
using EduPlayKids.Presentation.Models;
using EduPlayKids.Presentation.Views.Questions;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.ViewModels.Questions;

/// <summary>
/// View model for multiple choice questions with interactive option selection.
/// Supports single and multiple selection modes with audio feedback.
/// </summary>
public partial class MultipleChoiceQuestionViewModel : BaseQuestionViewModel
{
    [ObservableProperty]
    private MultipleChoiceQuestionModel? _multipleChoiceModel;

    [ObservableProperty]
    private string _selectionStatusText = string.Empty;

    [ObservableProperty]
    private bool _allowMultipleSelection;

    private readonly ILogger<MultipleChoiceQuestionViewModel> _multipleChoiceLogger;

    public MultipleChoiceQuestionViewModel(
        IAudioService audioService,
        IAnswerValidationService answerValidationService,
        IContentProgressionService progressionService,
        ILogger<MultipleChoiceQuestionViewModel> logger)
        : base(audioService, answerValidationService, progressionService, logger)
    {
        _multipleChoiceLogger = logger;

        // Initialize commands
        SelectOptionCommand = new RelayCommand<int>(SelectOption);
        PlayOptionAudioCommand = new AsyncRelayCommand<int>(PlayOptionAudioAsync);
    }

    #region Commands

    public IRelayCommand<int> SelectOptionCommand { get; }
    public IAsyncRelayCommand<int> PlayOptionAudioCommand { get; }

    #endregion

    #region Initialization

    public override async Task InitializeAsync(QuestionModelBase questionModel, int childId, int activityId, string language = "en")
    {
        if (questionModel is not MultipleChoiceQuestionModel mcModel)
        {
            throw new ArgumentException("Question model must be MultipleChoiceQuestionModel", nameof(questionModel));
        }

        await base.InitializeAsync(questionModel, childId, activityId, language);

        MultipleChoiceModel = mcModel;
        AllowMultipleSelection = mcModel.AllowMultipleSelection;

        // Set up option state tracking
        foreach (var option in mcModel.Options)
        {
            option.PropertyChanged += OnOptionPropertyChanged;
        }

        // Update selection status
        UpdateSelectionStatus();

        _multipleChoiceLogger.LogInformation("Multiple choice question initialized with {OptionCount} options, multiple selection: {AllowMultiple}",
            mcModel.Options.Count, AllowMultipleSelection);
    }

    protected override async Task<View?> CreateInteractiveContentAsync()
    {
        try
        {
            var view = new MultipleChoiceQuestionView();
            view.ViewModel = this;
            return view;
        }
        catch (Exception ex)
        {
            _multipleChoiceLogger.LogError(ex, "Error creating multiple choice interactive content");
            return null;
        }
    }

    #endregion

    #region Option Selection

    /// <summary>
    /// Handles option selection with single or multiple selection logic.
    /// </summary>
    /// <param name="optionIndex">Index of the selected option</param>
    private void SelectOption(int optionIndex)
    {
        try
        {
            if (MultipleChoiceModel == null || IsAnswered) return;

            _multipleChoiceLogger.LogInformation("Option {OptionIndex} selected", optionIndex);

            if (AllowMultipleSelection)
            {
                // Toggle selection for multiple selection mode
                MultipleChoiceModel.ToggleOption(optionIndex);
            }
            else
            {
                // Single selection mode
                MultipleChoiceModel.SelectedOptionIndex = optionIndex;
            }

            UpdateSelectionStatus();

            // Play selection audio feedback
            _ = PlayAudioAsync("audio/ui/option_selected.mp3", AudioType.UIInteraction);

            // Auto-submit for single selection if configured
            if (!AllowMultipleSelection && MultipleChoiceModel.SelectedOptionIndex >= 0)
            {
                // Small delay for audio feedback, then auto-submit
                _ = Task.Delay(500).ContinueWith(_ => SubmitAnswerAsync());
            }
        }
        catch (Exception ex)
        {
            _multipleChoiceLogger.LogError(ex, "Error selecting option {OptionIndex}", optionIndex);
        }
    }

    /// <summary>
    /// Plays audio for a specific option.
    /// </summary>
    /// <param name="optionIndex">Index of the option to play audio for</param>
    private async Task PlayOptionAudioAsync(int optionIndex)
    {
        try
        {
            if (MultipleChoiceModel == null || optionIndex < 0 || optionIndex >= MultipleChoiceModel.Options.Count)
                return;

            var option = MultipleChoiceModel.Options[optionIndex];
            if (!string.IsNullOrEmpty(option.AudioPath))
            {
                await PlayAudioAsync(option.AudioPath, AudioType.Instruction);
                _multipleChoiceLogger.LogInformation("Played audio for option {OptionIndex}", optionIndex);
            }
        }
        catch (Exception ex)
        {
            _multipleChoiceLogger.LogError(ex, "Error playing option audio for index {OptionIndex}", optionIndex);
        }
    }

    #endregion

    #region Answer Extraction

    protected override object? ExtractUserAnswer()
    {
        if (MultipleChoiceModel == null) return null;

        if (AllowMultipleSelection)
        {
            var selectedIndexes = MultipleChoiceModel.SelectedOptionIndexes;
            return selectedIndexes.Any() ? selectedIndexes : null;
        }
        else
        {
            return MultipleChoiceModel.SelectedOptionIndex >= 0
                ? MultipleChoiceModel.SelectedOptionIndex
                : null;
        }
    }

    #endregion

    #region Answer Processing

    protected override async Task ProcessValidationResultAsync(AnswerValidationResult validationResult)
    {
        await base.ProcessValidationResultAsync(validationResult);

        if (MultipleChoiceModel == null) return;

        // Update option visual states to show correct/incorrect
        for (int i = 0; i < MultipleChoiceModel.Options.Count; i++)
        {
            var option = MultipleChoiceModel.Options[i];
            var isCorrectAnswer = MultipleChoiceModel.CorrectAnswerIndexes.Contains(i);
            var wasSelected = AllowMultipleSelection
                ? MultipleChoiceModel.SelectedOptionIndexes.Contains(i)
                : MultipleChoiceModel.SelectedOptionIndex == i;

            // Set visual state for feedback
            option.IsCorrectAnswer = isCorrectAnswer;
            option.WasSelected = wasSelected;
        }

        // Play appropriate audio feedback
        if (validationResult.IsCorrect)
        {
            await PlayCorrectAnswerAudioAsync();
        }
        else
        {
            await PlayIncorrectAnswerAudioAsync();
        }
    }

    #endregion

    #region Audio Feedback

    /// <summary>
    /// Plays audio feedback for correct answer.
    /// </summary>
    private async Task PlayCorrectAnswerAudioAsync()
    {
        try
        {
            var audioFiles = new[]
            {
                "audio/feedback/excellent.mp3",
                "audio/feedback/perfect.mp3",
                "audio/feedback/great_job.mp3"
            };

            var randomFile = audioFiles[Random.Shared.Next(audioFiles.Length)];
            await PlayAudioAsync(randomFile, AudioType.SuccessFeedback);
        }
        catch (Exception ex)
        {
            _multipleChoiceLogger.LogError(ex, "Error playing correct answer audio");
        }
    }

    /// <summary>
    /// Plays audio feedback for incorrect answer.
    /// </summary>
    private async Task PlayIncorrectAnswerAudioAsync()
    {
        try
        {
            var audioFiles = new[]
            {
                "audio/feedback/try_again.mp3",
                "audio/feedback/almost_there.mp3",
                "audio/feedback/keep_trying.mp3"
            };

            var randomFile = audioFiles[Random.Shared.Next(audioFiles.Length)];
            await PlayAudioAsync(randomFile, AudioType.ErrorFeedback);
        }
        catch (Exception ex)
        {
            _multipleChoiceLogger.LogError(ex, "Error playing incorrect answer audio");
        }
    }

    #endregion

    #region State Management

    /// <summary>
    /// Updates the selection status text for multiple selection mode.
    /// </summary>
    private void UpdateSelectionStatus()
    {
        if (MultipleChoiceModel == null)
        {
            SelectionStatusText = string.Empty;
            return;
        }

        if (AllowMultipleSelection)
        {
            var selectedCount = MultipleChoiceModel.SelectedOptionIndexes.Count;
            var totalRequired = MultipleChoiceModel.CorrectAnswerIndexes.Count;

            SelectionStatusText = Language == "es"
                ? $"{selectedCount} de {totalRequired} respuestas seleccionadas"
                : $"{selectedCount} of {totalRequired} answers selected";

            // Enable submit when enough options are selected
            CanSubmitAnswer = selectedCount > 0 && !IsAnswered;
        }
        else
        {
            SelectionStatusText = string.Empty;
            CanSubmitAnswer = MultipleChoiceModel.SelectedOptionIndex >= 0 && !IsAnswered;
        }
    }

    protected override void ResetQuestionSpecificState()
    {
        if (MultipleChoiceModel == null) return;

        // Reset selection state
        MultipleChoiceModel.SelectedOptionIndex = -1;
        MultipleChoiceModel.SelectedOptionIndexes = new List<int>();

        // Reset option states
        foreach (var option in MultipleChoiceModel.Options)
        {
            option.IsSelected = false;
        }

        UpdateSelectionStatus();
    }

    #endregion

    #region Event Handlers

    private void OnOptionPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MultipleChoiceOption.IsSelected))
        {
            UpdateSelectionStatus();
        }
    }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets the current options for binding.
    /// </summary>
    public IEnumerable<MultipleChoiceOption> Options => MultipleChoiceModel?.Options ?? Enumerable.Empty<MultipleChoiceOption>();

    /// <summary>
    /// Indicates if any options are selected.
    /// </summary>
    public bool HasSelection => AllowMultipleSelection
        ? MultipleChoiceModel?.SelectedOptionIndexes.Any() ?? false
        : MultipleChoiceModel?.SelectedOptionIndex >= 0;

    #endregion

    #region Cleanup

    protected override void OnDisposing()
    {
        if (MultipleChoiceModel != null)
        {
            foreach (var option in MultipleChoiceModel.Options)
            {
                option.PropertyChanged -= OnOptionPropertyChanged;
            }
        }

        base.OnDisposing();
    }

    #endregion
}