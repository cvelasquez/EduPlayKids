using EduPlayKids.Application.Models.Audio;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.App.ViewModels;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using EduPlayKids.Presentation.Models;
using EduPlayKids.Presentation.Services;
using EduPlayKids.Presentation.ViewModels;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.ViewModels.Questions;

/// <summary>
/// Base view model for all question types with common functionality.
/// Provides audio integration, answer validation, and progress tracking.
/// </summary>
public abstract partial class BaseQuestionViewModel : AudioAwareBaseViewModel, IDisposable
{
    protected readonly IAnswerValidationService _answerValidationService;
    protected readonly IContentProgressionService _progressionService;
    protected readonly new ILogger<BaseQuestionViewModel> _logger;

    [ObservableProperty]
    private QuestionModelBase? _questionModel;

    [ObservableProperty]
    private bool _isAnswered;

    [ObservableProperty]
    private bool _isCorrect;

    [ObservableProperty]
    private bool _showExplanation;

    [ObservableProperty]
    private bool _canTryAgain;

    [ObservableProperty]
    private bool _canSubmitAnswer;

    [ObservableProperty]
    private bool _showProgress;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private string _encouragementMessage = string.Empty;

    [ObservableProperty]
    private string _instructionText = string.Empty;

    [ObservableProperty]
    private bool _hasInstructions;

    [ObservableProperty]
    private bool _hasAudio;

    [ObservableProperty]
    private bool _hasImage;

    [ObservableProperty]
    private bool _canShowHints;

    [ObservableProperty]
    private View? _interactiveContent;

    // Child and activity context
    public int ChildId { get; set; }
    public int ActivityId { get; set; }
    public int QuestionId { get; set; }
    public string Language { get; set; } = "en";

    protected BaseQuestionViewModel(
        IAudioService audioService,
        IAnswerValidationService answerValidationService,
        IContentProgressionService progressionService,
        ILogger<BaseQuestionViewModel> logger)
        : base(logger, audioService)
    {
        _answerValidationService = answerValidationService;
        _progressionService = progressionService;
        _logger = logger;

        // Initialize commands
        PlayAudioCommand = new AsyncRelayCommand(PlayQuestionAudioAsync);
        ShowHintCommand = new AsyncRelayCommand(ShowHintAsync);
        TryAgainCommand = new RelayCommand(ResetQuestion);
        SubmitAnswerCommand = new AsyncRelayCommand(SubmitAnswerAsync);
    }

    #region Commands

    public IAsyncRelayCommand PlayAudioCommand { get; }
    public IAsyncRelayCommand ShowHintCommand { get; }
    public IRelayCommand TryAgainCommand { get; }
    public IAsyncRelayCommand SubmitAnswerCommand { get; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the question view model with a question model.
    /// Sets up UI properties and prepares for interaction.
    /// </summary>
    /// <param name="questionModel">The question model to display</param>
    /// <param name="childId">The child answering the question</param>
    /// <param name="activityId">The parent activity ID</param>
    /// <param name="language">Language preference</param>
    public virtual async Task InitializeAsync(QuestionModelBase questionModel, int childId, int activityId, string language = "en")
    {
        try
        {
            _logger.LogInformation("Initializing question view model for question {QuestionId}", questionModel.QuestionId);

            QuestionModel = questionModel;
            ChildId = childId;
            ActivityId = activityId;
            QuestionId = questionModel.QuestionId;
            Language = language;

            // Set up UI properties
            InstructionText = GetInstructionText();
            HasInstructions = !string.IsNullOrEmpty(InstructionText);
            HasAudio = !string.IsNullOrEmpty(questionModel.AudioPath);
            HasImage = !string.IsNullOrEmpty(questionModel.ImagePath);

            // Initialize interaction state
            CanSubmitAnswer = true;
            CanTryAgain = false;
            ShowProgress = questionModel.MaxAttempts > 0;
            CanShowHints = questionModel.HintsEnabled && !string.IsNullOrEmpty(questionModel.HintText);

            // Create interactive content
            InteractiveContent = await CreateInteractiveContentAsync();

            // Subscribe to question model changes
            if (QuestionModel != null)
            {
                QuestionModel.PropertyChanged += OnQuestionModelPropertyChanged;
            }

            // Play welcome audio if enabled
            if (HasAudio && AudioEnabled)
            {
                await PlayQuestionAudioAsync();
            }

            _logger.LogInformation("Question view model initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing question view model");
        }
    }

    /// <summary>
    /// Creates the interactive content view for the specific question type.
    /// Must be implemented by derived classes.
    /// </summary>
    protected abstract Task<View?> CreateInteractiveContentAsync();

    /// <summary>
    /// Gets instruction text for the specific question type.
    /// Can be overridden by derived classes for custom instructions.
    /// </summary>
    protected virtual string GetInstructionText()
    {
        return QuestionModel?.QuestionType.ToLower() switch
        {
            "multiplechoice" => Language == "es" ? "Selecciona la respuesta correcta:" : "Choose the correct answer:",
            "draganddrop" => Language == "es" ? "Arrastra cada elemento a su lugar correcto:" : "Drag each item to the correct place:",
            "matching" => Language == "es" ? "Conecta los elementos que van juntos:" : "Match the items that go together:",
            "tracing" => Language == "es" ? "Traza siguiendo la línea:" : "Trace along the line:",
            _ => Language == "es" ? "Completa esta actividad:" : "Complete this activity:"
        };
    }

    #endregion

    #region Audio Integration

    /// <summary>
    /// Plays the question audio instruction.
    /// </summary>
    private async Task PlayQuestionAudioAsync()
    {
        try
        {
            if (QuestionModel?.AudioPath != null)
            {
                await PlayAudioAsync(QuestionModel.AudioPath, AudioType.Instruction);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing question audio");
        }
    }

    /// <summary>
    /// Shows hints and plays hint audio if available.
    /// </summary>
    private async Task ShowHintAsync()
    {
        try
        {
            if (QuestionModel == null) return;

            QuestionModel.ShowHints = true;
            CanShowHints = false;

            // Play hint audio if available
            var hintAudioPath = Language == "es" ? "hint_audio_es_path" : "hint_audio_en_path";
            if (!string.IsNullOrEmpty(hintAudioPath))
            {
                await PlayAudioAsync(hintAudioPath, AudioType.Instruction);
            }

            _logger.LogInformation("Hint shown for question {QuestionId}", QuestionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing hint");
        }
    }

    #endregion

    #region Answer Processing

    /// <summary>
    /// Submits the user's answer for validation.
    /// Must be implemented by derived classes to extract the answer.
    /// </summary>
    protected virtual async Task SubmitAnswerAsync()
    {
        try
        {
            if (QuestionModel == null || IsAnswered) return;

            _logger.LogInformation("Submitting answer for question {QuestionId}", QuestionId);

            var userAnswer = ExtractUserAnswer();
            if (userAnswer == null)
            {
                await PlayAudioAsync("audio/feedback/please_select_answer.mp3", AudioType.SuccessFeedback);
                return;
            }

            // Disable submission during processing
            CanSubmitAnswer = false;

            // Validate the answer
            var validationResult = await _answerValidationService.ValidateAnswerAsync(
                QuestionId, ChildId, userAnswer, GetTimeSpent(), CancellationToken.None);

            // Update UI state
            await ProcessValidationResultAsync(validationResult);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting answer for question {QuestionId}", QuestionId);
            CanSubmitAnswer = true; // Re-enable submission on error
        }
    }

    /// <summary>
    /// Extracts the user's answer from the UI.
    /// Must be implemented by derived question types.
    /// </summary>
    protected abstract object? ExtractUserAnswer();

    public object? GetUserAnswer() => ExtractUserAnswer();

    /// <summary>
    /// Processes the answer validation result and updates UI accordingly.
    /// </summary>
    protected virtual async Task ProcessValidationResultAsync(AnswerValidationResult validationResult)
    {
        try
        {
            IsAnswered = true;
            IsCorrect = validationResult.IsCorrect;

            if (QuestionModel != null)
            {
                QuestionModel.IsAnswered = true;
                QuestionModel.IsCorrect = validationResult.IsCorrect;
                QuestionModel.AttemptCount = validationResult.AttemptNumber;
            }

            if (validationResult.IsCorrect)
            {
                SuccessMessage = validationResult.FeedbackMessage;
                ShowExplanation = !string.IsNullOrEmpty(validationResult.ExplanationMessage);

                // Play success audio
                await PlayAudioAsync("audio/feedback/correct_answer.mp3", AudioType.SuccessFeedback);

                // Show completion celebration
                CanTryAgain = false;
            }
            else
            {
                EncouragementMessage = validationResult.FeedbackMessage;

                // Play encouragement audio
                await PlayAudioAsync("audio/feedback/try_again.mp3", AudioType.SuccessFeedback);

                // Allow retry if not at max attempts
                var hasAttemptsRemaining = QuestionModel?.MaxAttempts == 0 ||
                                         QuestionModel?.AttemptCount < QuestionModel?.MaxAttempts;
                CanTryAgain = hasAttemptsRemaining;
                CanSubmitAnswer = hasAttemptsRemaining;

                // Show hints if threshold reached
                if (QuestionModel != null && QuestionModel.ShouldShowHints())
                {
                    QuestionModel.ShowHints = true;
                }
            }

            // Fire completion event for parent to handle
            OnQuestionCompleted(validationResult);

            _logger.LogInformation("Answer processed: {IsCorrect}, attempt {AttemptNumber}",
                validationResult.IsCorrect, validationResult.AttemptNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing validation result");
        }
    }

    #endregion

    #region Question Reset

    /// <summary>
    /// Resets the question to allow another attempt.
    /// </summary>
    private void ResetQuestion()
    {
        try
        {
            _logger.LogInformation("Resetting question {QuestionId} for retry", QuestionId);

            IsAnswered = false;
            IsCorrect = false;
            ShowExplanation = false;
            CanTryAgain = false;
            CanSubmitAnswer = true;
            SuccessMessage = string.Empty;
            EncouragementMessage = string.Empty;

            // Reset question-specific state
            ResetQuestionSpecificState();

            OnQuestionReset();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting question");
        }
    }

    /// <summary>
    /// Resets question-type specific state.
    /// Can be overridden by derived classes.
    /// </summary>
    protected virtual void ResetQuestionSpecificState()
    {
        // Override in derived classes for specific reset logic
    }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when a question is completed (correct or max attempts reached).
    /// </summary>
    public event EventHandler<QuestionCompletedEventArgs>? QuestionCompleted;

    /// <summary>
    /// Event fired when a question is reset for retry.
    /// </summary>
    public event EventHandler? QuestionReset;

    protected virtual void OnQuestionCompleted(AnswerValidationResult validationResult)
    {
        QuestionCompleted?.Invoke(this, new QuestionCompletedEventArgs(validationResult));
    }

    protected virtual void OnQuestionReset()
    {
        QuestionReset?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Property Change Handling

    private void OnQuestionModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Update derived properties when question model changes
        switch (e.PropertyName)
        {
            case nameof(QuestionModelBase.IsAnswered):
                IsAnswered = QuestionModel?.IsAnswered ?? false;
                break;
            case nameof(QuestionModelBase.IsCorrect):
                IsCorrect = QuestionModel?.IsCorrect ?? false;
                break;
            case nameof(QuestionModelBase.ShowExplanation):
                ShowExplanation = QuestionModel?.ShowExplanation ?? false;
                break;
        }
    }

    #endregion

    #region Computed Properties

    public bool IsIncorrect => IsAnswered && !IsCorrect;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the time spent on this question in seconds.
    /// </summary>
    protected virtual int GetTimeSpent()
    {
        // TODO: Implement actual time tracking
        return 30; // Placeholder
    }

    #endregion

    #region Cleanup

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                OnDisposing();
            }
            _disposed = true;
        }
    }

    protected virtual void OnDisposing()
    {
        if (QuestionModel != null)
        {
            QuestionModel.PropertyChanged -= OnQuestionModelPropertyChanged;
        }

        // base.OnDisposing(); // Method not available in base class
    }

    #endregion
}

/// <summary>
/// Event arguments for question completion.
/// </summary>
public class QuestionCompletedEventArgs : EventArgs
{
    public AnswerValidationResult ValidationResult { get; }

    public QuestionCompletedEventArgs(AnswerValidationResult validationResult)
    {
        ValidationResult = validationResult;
    }
}