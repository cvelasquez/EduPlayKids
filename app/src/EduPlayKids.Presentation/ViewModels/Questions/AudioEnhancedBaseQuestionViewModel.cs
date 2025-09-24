using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.App.ViewModels;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Services;
using EduPlayKids.Presentation.Models;
using EduPlayKids.Presentation.Services;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.ViewModels.Questions;

/// <summary>
/// Enhanced base view model for all question types with comprehensive audio integration.
/// Provides bilingual narration, age-appropriate pacing, accessibility features,
/// and comprehensive educational audio workflows for children aged 3-8.
/// </summary>
public abstract partial class AudioEnhancedBaseQuestionViewModel : AudioAwareBaseViewModel
{
    #region Private Fields

    protected readonly IAnswerValidationService _answerValidationService;
    protected readonly IContentProgressionService _progressionService;
    protected readonly AudioQuestionIntegration _audioIntegration;
    protected readonly new ILogger<AudioEnhancedBaseQuestionViewModel> _logger;

    private DateTime _questionStartTime;
    private int _attemptCount;
    private bool _hasPlayedWelcomeAudio;

    #endregion

    #region Observable Properties

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
    private bool _canRepeatQuestion;

    [ObservableProperty]
    private bool _canPlayDescription;

    [ObservableProperty]
    private bool _isPlayingAudio;

    [ObservableProperty]
    private string _audioStatusText = string.Empty;

    [ObservableProperty]
    private View? _interactiveContent;

    // Audio-specific properties
    [ObservableProperty]
    private int _repetitionCount;

    [ObservableProperty]
    private int _currentHintLevel;

    [ObservableProperty]
    private bool _audioAutoPlayEnabled = true;

    [ObservableProperty]
    private bool _accessibilityModeEnabled;

    #endregion

    #region Context Properties

    public int ChildId { get; set; }
    public int ActivityId { get; set; }
    public int QuestionId { get; set; }
    public string Language { get; set; } = "en";
    public int ChildAge { get; set; } = 6;

    #endregion

    #region Constructor

    protected AudioEnhancedBaseQuestionViewModel(
        IAudioService audioService,
        IAnswerValidationService answerValidationService,
        IContentProgressionService progressionService,
        ILogger<AudioEnhancedBaseQuestionViewModel> logger)
        : base(logger, audioService)
    {
        _answerValidationService = answerValidationService;
        _progressionService = progressionService;
        _logger = logger;

        // Initialize audio integration
        _audioIntegration = new AudioQuestionIntegration(audioService,
            null);

        InitializeCommands();
        SubscribeToAudioEvents();
    }

    #endregion

    #region Commands

    public IAsyncRelayCommand PlayQuestionAudioCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayChoicesAudioCommand { get; private set; } = null!;
    public IAsyncRelayCommand RepeatQuestionCommand { get; private set; } = null!;
    public IAsyncRelayCommand ShowHintCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayDescriptionCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayInstructionsCommand { get; private set; } = null!;
    public IRelayCommand TryAgainCommand { get; private set; } = null!;
    public IAsyncRelayCommand SubmitAnswerCommand { get; private set; } = null!;
    public IRelayCommand ToggleAudioAutoPlayCommand { get; private set; } = null!;
    public IRelayCommand EnableAccessibilityModeCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        PlayQuestionAudioCommand = new AsyncRelayCommand(PlayQuestionAudioAsync);
        PlayChoicesAudioCommand = new AsyncRelayCommand(PlayChoicesAudioAsync);
        RepeatQuestionCommand = new AsyncRelayCommand(RepeatQuestionAsync);
        ShowHintCommand = new AsyncRelayCommand(ShowHintAsync);
        PlayDescriptionCommand = new AsyncRelayCommand(PlayDescriptionAsync);
        PlayInstructionsCommand = new AsyncRelayCommand(PlayInstructionsAsync);
        TryAgainCommand = new RelayCommand(ResetQuestion);
        SubmitAnswerCommand = new AsyncRelayCommand(SubmitAnswerAsync);
        ToggleAudioAutoPlayCommand = new RelayCommand(() => AudioAutoPlayEnabled = !AudioAutoPlayEnabled);
        EnableAccessibilityModeCommand = new RelayCommand(() => AccessibilityModeEnabled = !AccessibilityModeEnabled);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the question view model with comprehensive audio support.
    /// </summary>
    public virtual async Task InitializeAsync(
        QuestionModelBase questionModel,
        int childId,
        int activityId,
        int childAge,
        string language = "en")
    {
        try
        {
            _logger.LogInformation("Initializing audio-enhanced question view model for question {QuestionId}", questionModel.QuestionId);

            // Set context
            QuestionModel = questionModel;
            ChildId = childId;
            ActivityId = activityId;
            QuestionId = questionModel.QuestionId;
            Language = language;
            ChildAge = childAge;
            _questionStartTime = DateTime.UtcNow;
            _attemptCount = 0;
            _hasPlayedWelcomeAudio = false;

            // Initialize audio integration
            await _audioIntegration.InitializeQuestionAudioAsync(questionModel, childAge, language);

            // Set up UI properties
            await SetupUIPropertiesAsync();

            // Create interactive content
            InteractiveContent = await CreateInteractiveContentAsync();

            // Subscribe to question model changes
            if (QuestionModel != null)
            {
                QuestionModel.PropertyChanged += OnQuestionModelPropertyChanged;
            }

            // Auto-play welcome audio sequence if enabled
            if (AudioAutoPlayEnabled && HasAudio)
            {
                await PlayWelcomeAudioSequenceAsync();
            }

            _logger.LogInformation("Audio-enhanced question view model initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing audio-enhanced question view model");
        }
    }

    private async Task SetupUIPropertiesAsync()
    {
        if (QuestionModel == null) return;

        // Set instruction text with bilingual support
        InstructionText = GetLocalizedInstructionText();
        HasInstructions = !string.IsNullOrEmpty(InstructionText);

        // Audio capabilities
        HasAudio = !string.IsNullOrEmpty(QuestionModel.AudioPath) || _audioIntegration.CurrentQuestionAudio != null;
        HasImage = !string.IsNullOrEmpty(QuestionModel.ImagePath);

        // Audio control properties
        CanRepeatQuestion = HasAudio && _audioIntegration.CurrentQuestionAudio?.SupportsRepetition == true;
        CanPlayDescription = HasImage && AccessibilityModeEnabled;
        CanShowHints = QuestionModel.HintsEnabled && !string.IsNullOrEmpty(QuestionModel.HintText);

        // Interaction state
        CanSubmitAnswer = true;
        CanTryAgain = false;
        ShowProgress = QuestionModel.MaxAttempts > 0;

        // Audio status
        AudioStatusText = GetAudioStatusText();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates the interactive content view for the specific question type.
    /// Must be implemented by derived classes.
    /// </summary>
    protected abstract Task<View?> CreateInteractiveContentAsync();

    #endregion

    #region Audio Playback Methods

    /// <summary>
    /// Plays the complete welcome audio sequence for the question.
    /// </summary>
    private async Task PlayWelcomeAudioSequenceAsync()
    {
        try
        {
            if (_hasPlayedWelcomeAudio) return;

            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_welcome", "Playing welcome message...");

            // Play the complete question sequence
            await _audioIntegration.PlayQuestionSequenceAsync(includeChoices: true);

            _hasPlayedWelcomeAudio = true;
            AudioStatusText = GetLocalizedText("audio.ready", "Audio ready");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing welcome audio sequence");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Plays the question text audio.
    /// </summary>
    private async Task PlayQuestionAudioAsync()
    {
        try
        {
            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_question", "Playing question...");

            await _audioIntegration.PlayQuestionTextAsync();

            AudioStatusText = GetLocalizedText("audio.question_complete", "Question audio complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing question audio");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Plays the answer choices audio.
    /// </summary>
    private async Task PlayChoicesAudioAsync()
    {
        try
        {
            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_choices", "Playing answer choices...");

            await _audioIntegration.PlayAnswerChoicesSequenceAsync();

            AudioStatusText = GetLocalizedText("audio.choices_complete", "Choices audio complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing choices audio");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Repeats the question for accessibility.
    /// </summary>
    private async Task RepeatQuestionAsync()
    {
        try
        {
            IsPlayingAudio = true;

            var speed = ChildAge <= 4 ? "slower" : "normal";
            await _audioIntegration.RepeatQuestionAsync(speed);

            RepetitionCount = _audioIntegration.RepetitionCount;
            AudioStatusText = GetLocalizedText("audio.repeated", $"Question repeated ({RepetitionCount} times)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error repeating question");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Shows hint with audio support.
    /// </summary>
    private async Task ShowHintAsync()
    {
        try
        {
            if (QuestionModel == null) return;

            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_hint", "Playing hint...");

            // Show hint in UI
            QuestionModel.ShowHints = true;
            CanShowHints = false;

            // Play hint audio
            await _audioIntegration.PlayHintAudioAsync();

            CurrentHintLevel = _audioIntegration.CurrentHintLevel;
            AudioStatusText = GetLocalizedText("audio.hint_complete", $"Hint {CurrentHintLevel} played");

            _logger.LogInformation("Hint shown for question {QuestionId}, level {HintLevel}", QuestionId, CurrentHintLevel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing hint");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Plays visual description for accessibility.
    /// </summary>
    private async Task PlayDescriptionAsync()
    {
        try
        {
            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_description", "Playing visual description...");

            await _audioIntegration.PlayVisualDescriptionAsync();

            AudioStatusText = GetLocalizedText("audio.description_complete", "Description complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing description");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    /// <summary>
    /// Plays interaction instructions.
    /// </summary>
    private async Task PlayInstructionsAsync()
    {
        try
        {
            IsPlayingAudio = true;
            AudioStatusText = GetLocalizedText("audio.playing_instructions", "Playing instructions...");

            await _audioIntegration.PlayInteractionInstructionsAsync();

            AudioStatusText = GetLocalizedText("audio.instructions_complete", "Instructions complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing instructions");
            AudioStatusText = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    #endregion

    #region Answer Processing

    /// <summary>
    /// Submits the user's answer with audio feedback.
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
                // Play "please select an answer" audio
                await _audioService.PlayLocalizedAudioAsync("feedback.please_select", AudioType.Instruction, Language);
                return;
            }

            // Disable submission during processing
            CanSubmitAnswer = false;
            _attemptCount++;

            // Validate the answer
            var timeSpent = (int)(DateTime.UtcNow - _questionStartTime).TotalSeconds;
            var validationResult = await _answerValidationService.ValidateAnswerAsync(
                QuestionId, ChildId, userAnswer, timeSpent, CancellationToken.None);

            // Process result with audio feedback
            await ProcessValidationResultWithAudioAsync(validationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting answer for question {QuestionId}", QuestionId);
            CanSubmitAnswer = true; // Re-enable submission on error
        }
    }

    /// <summary>
    /// Processes validation result with comprehensive audio feedback.
    /// </summary>
    protected virtual async Task ProcessValidationResultWithAudioAsync(AnswerValidationResult validationResult)
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
                await HandleCorrectAnswerAsync(validationResult);
            }
            else
            {
                await HandleIncorrectAnswerAsync(validationResult);
            }

            // Fire completion event
            OnQuestionCompleted(validationResult);

            _logger.LogInformation("Answer processed with audio: {IsCorrect}, attempt {AttemptNumber}",
                validationResult.IsCorrect, validationResult.AttemptNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing validation result with audio");
        }
    }

    private async Task HandleCorrectAnswerAsync(AnswerValidationResult validationResult)
    {
        SuccessMessage = validationResult.FeedbackMessage;
        ShowExplanation = !string.IsNullOrEmpty(validationResult.ExplanationMessage);

        // Determine celebration intensity based on attempts
        var intensity = _attemptCount == 1 ? FeedbackIntensity.High : FeedbackIntensity.Medium;

        // Play success audio
        await _audioIntegration.PlaySuccessFeedbackAsync(intensity);

        // Disable further attempts
        CanTryAgain = false;
        CanSubmitAnswer = false;
    }

    private async Task HandleIncorrectAnswerAsync(AnswerValidationResult validationResult)
    {
        EncouragementMessage = validationResult.FeedbackMessage;

        // Play encouraging feedback
        var encouragementLevel = _attemptCount <= 2 ? FeedbackIntensity.Soft : FeedbackIntensity.Medium;
        await _audioIntegration.PlayEncouragementFeedbackAsync(encouragementLevel);

        // Check if can try again
        var hasAttemptsRemaining = QuestionModel?.MaxAttempts == 0 ||
                                 QuestionModel?.AttemptCount < QuestionModel?.MaxAttempts;

        CanTryAgain = hasAttemptsRemaining;
        CanSubmitAnswer = hasAttemptsRemaining;

        // Show hints if threshold reached
        if (QuestionModel != null && QuestionModel.ShouldShowHints())
        {
            CanShowHints = true;
        }
    }

    /// <summary>
    /// Extracts the user's answer from the UI.
    /// Must be implemented by derived question types.
    /// </summary>
    protected abstract object? ExtractUserAnswer();

    #endregion

    #region Question Reset

    /// <summary>
    /// Resets the question for another attempt.
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

            // Update audio status
            AudioStatusText = GetLocalizedText("audio.ready_retry", "Ready for retry");

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

    #region Helper Methods

    private string GetLocalizedInstructionText()
    {
        if (QuestionModel == null) return string.Empty;

        return QuestionModel.QuestionType.ToLower() switch
        {
            "multiplechoice" => GetLocalizedText("instructions.multiple_choice",
                Language == "es" ? "Selecciona la respuesta correcta:" : "Choose the correct answer:"),
            "draganddrop" => GetLocalizedText("instructions.drag_drop",
                Language == "es" ? "Arrastra cada elemento a su lugar correcto:" : "Drag each item to the correct place:"),
            "matching" => GetLocalizedText("instructions.matching",
                Language == "es" ? "Conecta los elementos que van juntos:" : "Match the items that go together:"),
            "tracing" => GetLocalizedText("instructions.tracing",
                Language == "es" ? "Traza siguiendo la línea:" : "Trace along the line:"),
            _ => GetLocalizedText("instructions.general",
                Language == "es" ? "Completa esta actividad:" : "Complete this activity:")
        };
    }

    private string GetAudioStatusText()
    {
        if (!HasAudio) return GetLocalizedText("audio.not_available", "Audio not available");
        if (IsPlayingAudio) return GetLocalizedText("audio.playing", "Playing audio...");
        return GetLocalizedText("audio.ready", "Audio ready");
    }

    private string GetLocalizedText(string key, string fallback)
    {
        // In a real implementation, this would use a localization service
        // For now, return the fallback text
        return fallback;
    }

    private void SubscribeToAudioEvents()
    {
        _audioIntegration.QuestionAudioEvent += OnQuestionAudioEvent;
        _audioIntegration.AccessibilityAudioEvent += OnAccessibilityAudioEvent;
    }

    #endregion

    #region Event Handlers

    private void OnQuestionAudioEvent(object? sender, QuestionAudioEventArgs e)
    {
        // Update UI based on audio events
        switch (e.QuestionEventType)
        {
            case QuestionAudioEventType.QuestionNarrationStarted:
                IsPlayingAudio = true;
                break;
            case QuestionAudioEventType.QuestionNarrationCompleted:
                IsPlayingAudio = false;
                break;
            case QuestionAudioEventType.QuestionRepeated:
                RepetitionCount = e.RepetitionCount;
                break;
            case QuestionAudioEventType.HintAudioPlayed:
                CurrentHintLevel = e.HintLevel ?? 1;
                break;
        }
    }

    private void OnAccessibilityAudioEvent(object? sender, AccessibilityAudioEventArgs e)
    {
        // Handle accessibility audio events
        _logger.LogDebug("Accessibility audio event: {EventType}", e.AccessibilityEventType);
    }

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

    #region Public Properties

    /// <summary>
    /// Gets whether the question is incorrect.
    /// </summary>
    public bool IsIncorrect => IsAnswered && !IsCorrect;

    /// <summary>
    /// Gets the current audio integration component.
    /// </summary>
    public AudioQuestionIntegration AudioIntegration => _audioIntegration;

    #endregion

    #region Cleanup

    protected virtual void OnDisposing()
    {
        if (QuestionModel != null)
        {
            QuestionModel.PropertyChanged -= OnQuestionModelPropertyChanged;
        }

        _audioIntegration?.Dispose();
        base.OnDisposing();
    }

    #endregion
}