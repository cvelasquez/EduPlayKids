using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.App.Services;
using EduPlayKids.App.ViewModels;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Services;
using QuestionAnswer = EduPlayKids.Application.Services.QuestionAnswer;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Infrastructure.Services.Audio;
using EduPlayKids.Presentation.Models;
using EduPlayKids.Presentation.Services;
using EduPlayKids.Presentation.ViewModels.Questions;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.ViewModels;

/// <summary>
/// Audio-enhanced activity view model with comprehensive bilingual narration,
/// age-appropriate pacing, progress encouragement, and celebration audio.
/// Designed for children aged 3-8 with full accessibility support.
/// </summary>
public partial class AudioEnhancedActivityViewModel : AudioAwareBaseViewModel
{
    #region Private Fields

    private readonly IActivityDeliveryService _activityDeliveryService;
    private readonly IAnswerValidationService _answerValidationService;
    private readonly IContentProgressionService _progressionService;
    private readonly IEducationalAudioService _educationalAudioService;
    private readonly QuestionModelFactory _questionModelFactory;
    private readonly IChildSafeNavigationService _navigationService;
    private readonly new ILogger<AudioEnhancedActivityViewModel> _logger;

    // Audio integration fields
    private readonly List<string> _audioPlaybackQueue;
    private readonly EducationalContext _educationalContext;
    private DateTime _activityStartTime;
    private DateTime _questionStartTime;
    private readonly List<QuestionAnswer> _questionAnswers = new();
    private bool _hasPlayedActivityIntro;
    private int _lastEncouragementProgress;

    #endregion

    #region Observable Properties

    [ObservableProperty]
    private Activity? _currentActivity;

    [ObservableProperty]
    private List<ActivityQuestion> _questions = new();

    [ObservableProperty]
    private int _currentQuestionIndex;

    [ObservableProperty]
    private AudioEnhancedBaseQuestionViewModel? _currentQuestionViewModel;

    [ObservableProperty]
    private bool _isActivityCompleted;

    [ObservableProperty]
    private int _correctAnswers;

    [ObservableProperty]
    private int _totalQuestions;

    [ObservableProperty]
    private int _starsEarned;

    [ObservableProperty]
    private double _progressPercentage;

    [ObservableProperty]
    private string _progressText = string.Empty;

    [ObservableProperty]
    private string _activityTitle = string.Empty;

    [ObservableProperty]
    private string _completionMessage = string.Empty;

    [ObservableProperty]
    private bool _showCelebration;

    [ObservableProperty]
    private bool _canProceedToNext;

    [ObservableProperty]
    private bool _showHelpButton;

    [ObservableProperty]
    private List<Achievement> _newAchievements = new();

    // Audio-specific properties
    [ObservableProperty]
    private bool _isPlayingActivityAudio;

    [ObservableProperty]
    private string _currentAudioDescription = string.Empty;

    [ObservableProperty]
    private bool _audioAutoPlayEnabled = true;

    [ObservableProperty]
    private bool _backgroundMusicEnabled = true;

    [ObservableProperty]
    private float _progressEncouragementThreshold = 0.25f; // 25% intervals

    [ObservableProperty]
    private bool _showAudioControls = true;

    [ObservableProperty]
    private List<string> _recentAudioEvents = new();

    #endregion

    #region Context Properties

    public int ChildId { get; set; }
    public int ActivityId { get; set; }
    public int ChildAge { get; set; } = 6;
    public string Language { get; set; } = "en";

    #endregion

    #region Constructor

    public AudioEnhancedActivityViewModel(
        IAudioService audioService,
        IEducationalAudioService educationalAudioService,
        IActivityDeliveryService activityDeliveryService,
        IAnswerValidationService answerValidationService,
        IContentProgressionService progressionService,
        QuestionModelFactory questionModelFactory,
        IChildSafeNavigationService navigationService,
        ILogger<AudioEnhancedActivityViewModel> logger)
        : base(logger, audioService)
    {
        _educationalAudioService = educationalAudioService;
        _activityDeliveryService = activityDeliveryService;
        _answerValidationService = answerValidationService;
        _progressionService = progressionService;
        _questionModelFactory = questionModelFactory;
        _navigationService = navigationService;
        _logger = logger;

        _audioPlaybackQueue = new List<string>();
        _educationalContext = new EducationalContext();
        _lastEncouragementProgress = 0;

        InitializeCommands();
        SubscribeToEducationalAudioEvents();
    }

    #endregion

    #region Commands

    public IAsyncRelayCommand NextQuestionCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayActivityIntroCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayInstructionsCommand { get; private set; } = null!;
    public IAsyncRelayCommand ShowHintCommand { get; private set; } = null!;
    public IAsyncRelayCommand CompleteActivityCommand { get; private set; } = null!;
    public IAsyncRelayCommand GoBackCommand { get; private set; } = null!;
    public IAsyncRelayCommand RestartActivityCommand { get; private set; } = null!;
    public IAsyncRelayCommand ToggleBackgroundMusicCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayProgressEncouragementCommand { get; private set; } = null!;
    public IRelayCommand ToggleAudioAutoPlayCommand { get; private set; } = null!;
    public IAsyncRelayCommand PlayActivitySummaryCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        NextQuestionCommand = new AsyncRelayCommand(NextQuestionAsync, () => CanProceedToNext);
        PlayActivityIntroCommand = new AsyncRelayCommand(PlayActivityIntroductionAsync);
        PlayInstructionsCommand = new AsyncRelayCommand(PlayInstructionsAsync);
        ShowHintCommand = new AsyncRelayCommand(ShowCurrentQuestionHintAsync);
        CompleteActivityCommand = new AsyncRelayCommand(CompleteActivityAsync, () => IsActivityCompleted);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        RestartActivityCommand = new AsyncRelayCommand(RestartActivityAsync);
        ToggleBackgroundMusicCommand = new AsyncRelayCommand(ToggleBackgroundMusicAsync);
        PlayProgressEncouragementCommand = new AsyncRelayCommand(PlayProgressEncouragementAsync);
        ToggleAudioAutoPlayCommand = new RelayCommand(() => AudioAutoPlayEnabled = !AudioAutoPlayEnabled);
        PlayActivitySummaryCommand = new AsyncRelayCommand(PlayActivitySummaryAsync);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the activity with comprehensive audio support.
    /// </summary>
    public async Task InitializeAsync(int childId, int activityId, int childAge, string language = "en")
    {
        try
        {
            _logger.LogInformation("Initializing audio-enhanced activity for child {ChildId}, activity {ActivityId}, age {ChildAge}",
                childId, activityId, childAge);

            // Set context
            ChildId = childId;
            ActivityId = activityId;
            ChildAge = childAge;
            Language = language;
            _activityStartTime = DateTime.UtcNow;
            _hasPlayedActivityIntro = false;

            await ExecuteAsync(async () =>
            {
                // Load activity with questions
                CurrentActivity = await _activityDeliveryService.LoadActivityForChildAsync(activityId, childId, language);
                if (CurrentActivity == null)
                {
                    throw new InvalidOperationException($"Activity {activityId} not found or not available for child {childId}");
                }

                // Set up educational context
                _educationalContext.Subject = CurrentActivity.Subject?.Name ?? "general";
                _educationalContext.ActivityType = CurrentActivity.ActivityType ?? "general";
                _educationalContext.DifficultyLevel = CurrentActivity.DifficultyLevel ?? "medium";
                _educationalContext.Session.ChildId = childId;
                _educationalContext.Session.SessionStart = _activityStartTime;
                _educationalContext.Session.SessionId = Guid.NewGuid().ToString();

                // Set educational context in audio service
                _educationalAudioService.SetEducationalContext(_educationalContext);

                // Load questions
                var activityQuestions = await _activityDeliveryService.GetActivityQuestionsAsync(activityId, childId, language);
                Questions = activityQuestions.ToList();
                TotalQuestions = Questions.Count;

                // Set up activity properties
                ActivityTitle = GetLocalizedActivityTitle();
                ShowHelpButton = ChildAge <= 5; // Show help for younger children
                ProgressText = GetLocalizedText("progress.starting", "Starting activity...");

                // Start background music if enabled
                if (BackgroundMusicEnabled)
                {
                    await StartBackgroundMusicAsync();
                }

                // Load first question
                if (Questions.Any())
                {
                    await LoadQuestionAsync(0);
                }

                // Play activity introduction if auto-play enabled
                if (AudioAutoPlayEnabled)
                {
                    await PlayActivityIntroductionAsync();
                }

                _logger.LogInformation("Audio-enhanced activity initialized successfully with {QuestionCount} questions", Questions.Count);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing audio-enhanced activity");
            await ShowErrorWithAudioAsync("activity.init_error", "Failed to load activity. Please try again.");
        }
    }

    #endregion

    #region Activity Audio Methods

    /// <summary>
    /// Plays the activity introduction with age-appropriate narration.
    /// </summary>
    private async Task PlayActivityIntroductionAsync()
    {
        try
        {
            if (_hasPlayedActivityIntro || CurrentActivity == null) return;

            IsPlayingActivityAudio = true;
            CurrentAudioDescription = GetLocalizedText("audio.playing_intro", "Playing activity introduction...");
            AddAudioEvent("Activity introduction started");

            var activityType = CurrentActivity.Subject?.Name ?? "general";
            var difficultyLevel = CurrentActivity.DifficultyLevel ?? "medium";

            await _educationalAudioService.PlayActivityIntroductionAsync(activityType, difficultyLevel, ChildAge);

            _hasPlayedActivityIntro = true;
            CurrentAudioDescription = GetLocalizedText("audio.intro_complete", "Introduction complete");
            AddAudioEvent("Activity introduction completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing activity introduction");
            CurrentAudioDescription = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingActivityAudio = false;
        }
    }

    /// <summary>
    /// Plays step-by-step instructions for the current activity.
    /// </summary>
    private async Task PlayInstructionsAsync()
    {
        try
        {
            if (CurrentActivity == null) return;

            IsPlayingActivityAudio = true;
            CurrentAudioDescription = GetLocalizedText("audio.playing_instructions", "Playing instructions...");
            AddAudioEvent("Instructions started");

            var instructions = GenerateStepByStepInstructions();
            await _educationalAudioService.PlayStepByStepGuidanceAsync(instructions, stepDelay: 2000, ChildAge);

            CurrentAudioDescription = GetLocalizedText("audio.instructions_complete", "Instructions complete");
            AddAudioEvent("Instructions completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing instructions");
            CurrentAudioDescription = GetLocalizedText("audio.error", "Audio error");
        }
        finally
        {
            IsPlayingActivityAudio = false;
        }
    }

    /// <summary>
    /// Plays background music appropriate for the activity.
    /// </summary>
    private async Task StartBackgroundMusicAsync()
    {
        try
        {
            if (!BackgroundMusicEnabled || CurrentActivity == null) return;

            var activityType = CurrentActivity.Subject?.Name ?? "general";
            var volume = ChildAge <= 4 ? 0.2f : 0.3f; // Lower volume for younger children

            await _audioService.PlayBackgroundMusicAsync(activityType, volume, loop: true);
            AddAudioEvent("Background music started");
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Background music not available: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Toggles background music on/off.
    /// </summary>
    private async Task ToggleBackgroundMusicAsync()
    {
        try
        {
            BackgroundMusicEnabled = !BackgroundMusicEnabled;

            if (BackgroundMusicEnabled)
            {
                await StartBackgroundMusicAsync();
            }
            else
            {
                await _audioService.StopAudioAsync(AudioType.BackgroundMusic);
                AddAudioEvent("Background music stopped");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling background music");
        }
    }

    /// <summary>
    /// Plays progress encouragement based on current progress.
    /// </summary>
    private async Task PlayProgressEncouragementAsync()
    {
        try
        {
            var progressPercent = (int)(ProgressPercentage * 100);

            // Only play encouragement at certain thresholds and not too frequently
            if (progressPercent > _lastEncouragementProgress + 25)
            {
                IsPlayingActivityAudio = true;
                CurrentAudioDescription = GetLocalizedText("audio.playing_encouragement", "Playing encouragement...");

                await _educationalAudioService.PlayProgressEncouragementAsync(progressPercent, ChildAge);

                _lastEncouragementProgress = progressPercent;
                AddAudioEvent($"Progress encouragement played at {progressPercent}%");
                CurrentAudioDescription = GetLocalizedText("audio.encouragement_complete", "Encouragement complete");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing progress encouragement");
        }
        finally
        {
            IsPlayingActivityAudio = false;
        }
    }

    /// <summary>
    /// Plays activity summary at completion.
    /// </summary>
    private async Task PlayActivitySummaryAsync()
    {
        try
        {
            if (!IsActivityCompleted) return;

            IsPlayingActivityAudio = true;
            CurrentAudioDescription = GetLocalizedText("audio.playing_summary", "Playing activity summary...");

            // Play completion celebration first
            await _audioService.PlayActivityCompletionAsync(StarsEarned, CurrentActivity?.Subject?.Name);

            // Add delay before summary
            await Task.Delay(1500);

            // Generate and play summary
            var summaryText = GenerateActivitySummary();
            await _audioService.PlayQuestionAudioAsync(summaryText, "activity_summary");

            AddAudioEvent("Activity summary played");
            CurrentAudioDescription = GetLocalizedText("audio.summary_complete", "Summary complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing activity summary");
        }
        finally
        {
            IsPlayingActivityAudio = false;
        }
    }

    #endregion

    #region Question Management

    /// <summary>
    /// Loads a question with audio integration.
    /// </summary>
    private async Task LoadQuestionAsync(int questionIndex)
    {
        try
        {
            if (questionIndex < 0 || questionIndex >= Questions.Count) return;

            CurrentQuestionIndex = questionIndex;
            var question = Questions[questionIndex];
            _questionStartTime = DateTime.UtcNow;

            // Create question model
            var questionModel = await _questionModelFactory.CreateQuestionModelAsync(question, ChildAge, Language, new Dictionary<string, bool>());

            // Create audio-enhanced question view model
            CurrentQuestionViewModel = CreateAudioEnhancedQuestionViewModel(questionModel);

            if (CurrentQuestionViewModel != null)
            {
                // Initialize with audio support
                await CurrentQuestionViewModel.InitializeAsync(questionModel, ChildId, ActivityId, ChildAge, Language);

                // Subscribe to completion events
                CurrentQuestionViewModel.QuestionCompleted += OnQuestionCompleted;
                CurrentQuestionViewModel.QuestionReset += OnQuestionReset;

                // Update progress
                UpdateProgressDisplay();

                // Play progress encouragement if threshold reached
                if (ProgressPercentage >= _lastEncouragementProgress + ProgressEncouragementThreshold)
                {
                    await PlayProgressEncouragementAsync();
                }

                AddAudioEvent($"Question {questionIndex + 1} loaded");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading question {QuestionIndex}", questionIndex);
        }
    }

    /// <summary>
    /// Creates an audio-enhanced question view model based on question type.
    /// </summary>
    private AudioEnhancedBaseQuestionViewModel? CreateAudioEnhancedQuestionViewModel(QuestionModelBase questionModel)
    {
        // In a real implementation, this would create specific view models for different question types
        // For now, return null to indicate not implemented
        _logger.LogDebug("Creating audio-enhanced question view model for type: {QuestionType}", questionModel.QuestionType);
        return null;
    }

    /// <summary>
    /// Proceeds to the next question with audio transition.
    /// </summary>
    private async Task NextQuestionAsync()
    {
        try
        {
            if (CurrentQuestionIndex >= Questions.Count - 1)
            {
                await CompleteActivityAsync();
                return;
            }

            // Play transition audio
            await _audioService.PlayUIFeedbackAsync(UIInteractionType.PageTransition);

            // Brief pause
            await Task.Delay(500);

            // Load next question
            await LoadQuestionAsync(CurrentQuestionIndex + 1);

            CanProceedToNext = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proceeding to next question");
        }
    }

    /// <summary>
    /// Shows hint for current question with audio.
    /// </summary>
    private async Task ShowCurrentQuestionHintAsync()
    {
        try
        {
            if (CurrentQuestionViewModel?.AudioIntegration != null)
            {
                await CurrentQuestionViewModel.AudioIntegration.PlayHintAudioAsync();
                AddAudioEvent("Hint played for current question");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing hint for current question");
        }
    }

    #endregion

    #region Activity Completion

    /// <summary>
    /// Completes the activity with celebration audio.
    /// </summary>
    private async Task CompleteActivityAsync()
    {
        try
        {
            IsActivityCompleted = true;
            CanProceedToNext = false;

            // Calculate stars and performance
            CalculateActivityResults();

            // Stop background music
            await _audioService.StopAudioAsync(AudioType.BackgroundMusic);

            // Play completion celebration
            await PlayCompletionCelebrationAsync();

            // Check for achievements
            await CheckForAchievementsAsync();

            // Update progress in services
            await UpdateActivityProgressAsync();

            ShowCelebration = true;
            CompletionMessage = GenerateCompletionMessage();

            AddAudioEvent($"Activity completed with {StarsEarned} stars");
            _logger.LogInformation("Activity {ActivityId} completed by child {ChildId} with {Stars} stars",
                ActivityId, ChildId, StarsEarned);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing activity");
        }
    }

    /// <summary>
    /// Plays celebration audio based on performance.
    /// </summary>
    private async Task PlayCompletionCelebrationAsync()
    {
        try
        {
            IsPlayingActivityAudio = true;
            CurrentAudioDescription = GetLocalizedText("audio.playing_celebration", "Playing celebration...");

            // Play star-based celebration
            await _audioService.PlayActivityCompletionAsync(StarsEarned, CurrentActivity?.Subject?.Name);

            // Play milestone achievement if applicable
            if (NewAchievements.Any())
            {
                await Task.Delay(1000);
                foreach (var achievement in NewAchievements)
                {
                    // Convert level string to number (Common=1, Rare=2, Epic=3, Legendary=4)
                    var levelNumber = achievement.Level.ToLower() switch
                    {
                        "common" => 1,
                        "rare" => 2,
                        "epic" => 3,
                        "legendary" => 4,
                        _ => 1
                    };
                    await _educationalAudioService.PlayMilestoneAchievementAsync(
                        achievement.Type, levelNumber, null);
                }
            }

            AddAudioEvent($"Completion celebration played for {StarsEarned} stars");
            CurrentAudioDescription = GetLocalizedText("audio.celebration_complete", "Celebration complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing completion celebration");
        }
        finally
        {
            IsPlayingActivityAudio = false;
        }
    }

    #endregion

    #region Helper Methods

    private void UpdateProgressDisplay()
    {
        ProgressPercentage = TotalQuestions > 0 ? (double)CurrentQuestionIndex / TotalQuestions : 0;
        ProgressText = GetLocalizedText("progress.format", $"Question {CurrentQuestionIndex + 1} of {TotalQuestions}");
    }

    private void CalculateActivityResults()
    {
        var accuracy = TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions : 0;

        StarsEarned = accuracy switch
        {
            >= 0.9 => 3,
            >= 0.7 => 2,
            >= 0.5 => 1,
            _ => 0
        };

        _educationalContext.Performance.AccuracyPercentage = (int)(accuracy * 100);
        _educationalContext.Performance.CorrectAnswers = CorrectAnswers;
        _educationalContext.Performance.TotalQuestions = TotalQuestions;
    }

    private List<string> GenerateStepByStepInstructions()
    {
        var instructions = new List<string>();

        if (CurrentActivity == null) return instructions;

        var baseKey = $"instructions.{CurrentActivity.ActivityType}";

        instructions.Add(GetLocalizedText($"{baseKey}.step1", "Look at the question carefully."));
        instructions.Add(GetLocalizedText($"{baseKey}.step2", "Think about the correct answer."));
        instructions.Add(GetLocalizedText($"{baseKey}.step3", "Select or complete your answer."));
        instructions.Add(GetLocalizedText($"{baseKey}.step4", "Check your work before submitting."));

        return instructions;
    }

    private string GenerateActivitySummary()
    {
        var summaryTemplate = GetLocalizedText("summary.template",
            "Great job! You completed {0} questions and got {1} correct. You earned {2} stars!");

        return string.Format(summaryTemplate, TotalQuestions, CorrectAnswers, StarsEarned);
    }

    private string GenerateCompletionMessage()
    {
        return StarsEarned switch
        {
            3 => GetLocalizedText("completion.excellent", "Excellent work! You're a star learner!"),
            2 => GetLocalizedText("completion.great", "Great job! You're doing wonderful!"),
            1 => GetLocalizedText("completion.good", "Good effort! Keep practicing!"),
            _ => GetLocalizedText("completion.keep_trying", "Keep trying! You're learning!")
        };
    }

    private string GetLocalizedActivityTitle()
    {
        if (CurrentActivity?.Title == null) return string.Empty;

        // In a real implementation, this would use a localization service
        return CurrentActivity.Title;
    }

    private string GetLocalizedText(string key, string fallback)
    {
        // In a real implementation, this would use a localization service
        return fallback;
    }

    private void AddAudioEvent(string eventDescription)
    {
        RecentAudioEvents.Insert(0, $"{DateTime.Now:HH:mm:ss} - {eventDescription}");

        // Keep only last 10 events
        if (RecentAudioEvents.Count > 10)
        {
            RecentAudioEvents.RemoveAt(RecentAudioEvents.Count - 1);
        }
    }

    private async Task ShowErrorWithAudioAsync(string errorKey, string errorMessage)
    {
        await _audioService.PlayUIFeedbackAsync(UIInteractionType.ModalDialog);
        _logger.LogError("Activity error: {ErrorMessage}", errorMessage);
    }

    private async Task CheckForAchievementsAsync()
    {
        try
        {
            // Check for achievements (placeholder implementation)
            NewAchievements.Clear();

            // In a real implementation, this would check against achievement criteria
            if (StarsEarned == 3)
            {
                NewAchievements.Add(new Achievement { Type = "perfect_score", Level = "1" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for achievements");
        }
    }

    private async Task UpdateActivityProgressAsync()
    {
        try
        {
            // Update progress in data services (placeholder implementation)
            await Task.Delay(1); // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity progress");
        }
    }

    #endregion

    #region Navigation Methods

    private async Task GoBackAsync()
    {
        try
        {
            await _audioService.StopAudioAsync();
            await _navigationService.NavigateBackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating back");
        }
    }

    private async Task RestartActivityAsync()
    {
        try
        {
            await _audioService.StopAudioAsync();
            await InitializeAsync(ChildId, ActivityId, ChildAge, Language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restarting activity");
        }
    }

    #endregion

    #region Event Handlers

    private void OnQuestionCompleted(object? sender, QuestionCompletedEventArgs e)
    {
        try
        {
            if (e.ValidationResult.IsCorrect)
            {
                CorrectAnswers++;
            }

            CanProceedToNext = true;
            UpdateProgressDisplay();

            // Record answer for analytics
            var questionAnswer = new QuestionAnswer
            {
                QuestionId = CurrentQuestionViewModel?.QuestionId ?? 0,
                IsCorrect = e.ValidationResult.IsCorrect,
                AttemptCount = e.ValidationResult.AttemptNumber,
                TimeSpentSeconds = (int)(DateTime.UtcNow - _questionStartTime).TotalSeconds
            };

            _questionAnswers.Add(questionAnswer);

            AddAudioEvent($"Question completed: {(e.ValidationResult.IsCorrect ? "Correct" : "Incorrect")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling question completion");
        }
    }

    private void OnQuestionReset(object? sender, EventArgs e)
    {
        CanProceedToNext = false;
        AddAudioEvent("Question reset for retry");
    }

    private void SubscribeToEducationalAudioEvents()
    {
        _educationalAudioService.EducationalAudioStarted += OnEducationalAudioStarted;
        _educationalAudioService.EducationalAudioCompleted += OnEducationalAudioCompleted;
        _educationalAudioService.ActivityAudioEvent += OnActivityAudioEvent;
    }

    private void OnEducationalAudioStarted(object? sender, EducationalAudioEventArgs e)
    {
        IsPlayingActivityAudio = true;
        AddAudioEvent($"Educational audio started: {e.EventType}");
    }

    private void OnEducationalAudioCompleted(object? sender, EducationalAudioEventArgs e)
    {
        IsPlayingActivityAudio = false;
        AddAudioEvent($"Educational audio completed: {e.EventType}");
    }

    private void OnActivityAudioEvent(object? sender, ActivityAudioEventArgs e)
    {
        AddAudioEvent($"Activity audio event: {e.ActivityEventType}");
    }

    #endregion

    #region Cleanup

    protected virtual void OnDisposing()
    {
        if (CurrentQuestionViewModel != null)
        {
            CurrentQuestionViewModel.QuestionCompleted -= OnQuestionCompleted;
            CurrentQuestionViewModel.QuestionReset -= OnQuestionReset;
        }

        _educationalAudioService.EducationalAudioStarted -= OnEducationalAudioStarted;
        _educationalAudioService.EducationalAudioCompleted -= OnEducationalAudioCompleted;
        _educationalAudioService.ActivityAudioEvent -= OnActivityAudioEvent;

        base.OnDisposing();
    }

    #endregion
}