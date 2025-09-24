using EduPlayKids.Application.Models.Audio;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.App.Services;
using EduPlayKids.App.ViewModels;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using QuestionAnswer = EduPlayKids.Application.Services.QuestionAnswer;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Presentation.Models;
using EduPlayKids.Presentation.Services;
using EduPlayKids.Presentation.ViewModels.Questions;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.ViewModels;

/// <summary>
/// Enhanced activity view model with complete educational content delivery system.
/// Integrates question rendering, answer validation, progress tracking, and audio feedback.
/// </summary>
public partial class EnhancedActivityViewModel : AudioAwareBaseViewModel
{
    private readonly IActivityDeliveryService _activityDeliveryService;
    private readonly IAnswerValidationService _answerValidationService;
    private readonly IContentProgressionService _progressionService;
    private readonly QuestionModelFactory _questionModelFactory;
    private readonly IChildSafeNavigationService _navigationService;
    private readonly new ILogger<EnhancedActivityViewModel> _logger;

    [ObservableProperty]
    private Activity? _currentActivity;

    [ObservableProperty]
    private List<ActivityQuestion> _questions = new();

    [ObservableProperty]
    private int _currentQuestionIndex;

    [ObservableProperty]
    private BaseQuestionViewModel? _currentQuestionViewModel;

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

    // Context properties
    public int ChildId { get; set; }
    public int ActivityId { get; set; }
    public string Language { get; set; } = "en";

    // Timing and analytics
    private DateTime _activityStartTime;
    private DateTime _questionStartTime;
    private readonly List<QuestionAnswer> _questionAnswers = new();

    public EnhancedActivityViewModel(
        IAudioService audioService,
        IActivityDeliveryService activityDeliveryService,
        IAnswerValidationService answerValidationService,
        IContentProgressionService progressionService,
        QuestionModelFactory questionModelFactory,
        IChildSafeNavigationService navigationService,
        ILogger<EnhancedActivityViewModel> logger)
        : base(logger, audioService)
    {
        _activityDeliveryService = activityDeliveryService;
        _answerValidationService = answerValidationService;
        _progressionService = progressionService;
        _questionModelFactory = questionModelFactory;
        _navigationService = navigationService;
        _logger = logger;

        // Initialize commands
        NextQuestionCommand = new AsyncRelayCommand(NextQuestionAsync, () => CanProceedToNext);
        PlayInstructionsCommand = new AsyncRelayCommand(PlayInstructionsAsync);
        ShowHintCommand = new AsyncRelayCommand(ShowCurrentQuestionHintAsync);
        CompleteActivityCommand = new AsyncRelayCommand(CompleteActivityAsync, () => IsActivityCompleted);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        RestartActivityCommand = new AsyncRelayCommand(RestartActivityAsync);
    }

    #region Commands

    public IAsyncRelayCommand NextQuestionCommand { get; }
    public IAsyncRelayCommand PlayInstructionsCommand { get; }
    public IAsyncRelayCommand ShowHintCommand { get; }
    public IAsyncRelayCommand CompleteActivityCommand { get; }
    public IAsyncRelayCommand GoBackCommand { get; }
    public IAsyncRelayCommand RestartActivityCommand { get; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the activity with comprehensive content loading.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity to load</param>
    /// <param name="language">Language preference</param>
    public async Task InitializeAsync(int childId, int activityId, string language = "en")
    {
        try
        {
            _logger.LogInformation("Initializing enhanced activity for child {ChildId}, activity {ActivityId}", childId, activityId);

            ChildId = childId;
            ActivityId = activityId;
            Language = language;
            _activityStartTime = DateTime.UtcNow;

            await ExecuteAsync(async () =>
            {
                // Load activity with questions
                CurrentActivity = await _activityDeliveryService.LoadActivityForChildAsync(activityId, childId, language);
                if (CurrentActivity == null)
                {
                    throw new InvalidOperationException($"Activity {activityId} not found or not available for child {childId}");
                }

                // Load questions for this activity
                var activityQuestions = await _activityDeliveryService.GetActivityQuestionsAsync(activityId, childId, language);
                Questions = activityQuestions.ToList();
                TotalQuestions = Questions.Count;

                // Set up activity display properties
                ActivityTitle = CurrentActivity.GetLocalizedTitle(language);
                UpdateProgressDisplay();

                // Initialize first question
                await LoadCurrentQuestionAsync();

                // Start background music for the subject
                if (CurrentActivity.Subject != null)
                {
                    await StartBackgroundMusicAsync(CurrentActivity.Subject.NameEn.ToLower());
                }

                _logger.LogInformation("Activity initialized: {Title} with {QuestionCount} questions", ActivityTitle, TotalQuestions);

            }, Language == "es" ? "Cargando tu actividad..." : "Loading your activity...");

            // Play welcome audio
            await PlayActivityWelcomeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing enhanced activity");
            await _navigationService.ShowErrorAsync(
                Language == "es" ? "Error" : "Error",
                Language == "es" ? "No se pudo cargar la actividad" : "Could not load activity");
        }
    }

    /// <summary>
    /// Loads and sets up the current question view model.
    /// </summary>
    private async Task LoadCurrentQuestionAsync()
    {
        try
        {
            if (CurrentQuestionIndex >= Questions.Count)
            {
                await CompleteActivityAsync();
                return;
            }

            var question = Questions[CurrentQuestionIndex];
            _questionStartTime = DateTime.UtcNow;

            // Get support features for this child
            var supportFeatures = await _activityDeliveryService.GetSupportFeaturesForChildAsync(
                ChildId, question.QuestionType, CancellationToken.None);

            // Create question model
            var childAge = await GetChildAgeAsync();
            var questionModel = _questionModelFactory.CreateQuestionModel(question, childAge, Language, supportFeatures);

            if (questionModel == null)
            {
                _logger.LogWarning("Could not create question model for question {QuestionId}", question.Id);
                await NextQuestionAsync(); // Skip problematic question
                return;
            }

            // Create appropriate view model based on question type
            CurrentQuestionViewModel = await CreateQuestionViewModelAsync(questionModel);
            if (CurrentQuestionViewModel != null)
            {
                // Subscribe to question completion events
                CurrentQuestionViewModel.QuestionCompleted += OnQuestionCompleted;
                CurrentQuestionViewModel.QuestionReset += OnQuestionReset;

                // Initialize the question view model
                await CurrentQuestionViewModel.InitializeAsync(questionModel, ChildId, ActivityId, Language);
            }

            // Update UI state
            CanProceedToNext = false;
            ShowHelpButton = questionModel.HintsEnabled;
            UpdateProgressDisplay();

            _logger.LogInformation("Loaded question {QuestionIndex}: {QuestionType}", CurrentQuestionIndex + 1, question.QuestionType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current question");
            await NextQuestionAsync(); // Try to proceed to next question
        }
    }

    /// <summary>
    /// Creates the appropriate question view model based on question type.
    /// </summary>
    private async Task<BaseQuestionViewModel?> CreateQuestionViewModelAsync(QuestionModelBase questionModel)
    {
        return questionModel switch
        {
            MultipleChoiceQuestionModel => new MultipleChoiceQuestionViewModel(
                _audioService, _answerValidationService, _progressionService, null),
            // Add other question types here as they're implemented
            // DragDropQuestionModel => new DragDropQuestionViewModel(...),
            // MatchingQuestionModel => new MatchingQuestionViewModel(...),
            // TracingQuestionModel => new TracingQuestionViewModel(...),
            _ => null
        };
    }

    #endregion

    #region Question Flow Management

    /// <summary>
    /// Proceeds to the next question in the activity.
    /// </summary>
    private async Task NextQuestionAsync()
    {
        try
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                await LoadCurrentQuestionAsync();

                // Play transition audio
                await PlayAudioAsync("audio/transitions/next_question.mp3", AudioType.UIInteraction);
            }
            else
            {
                await CompleteActivityAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proceeding to next question");
        }
    }

    /// <summary>
    /// Shows hint for the current question.
    /// </summary>
    private async Task ShowCurrentQuestionHintAsync()
    {
        try
        {
            if (CurrentQuestionViewModel != null)
            {
                await CurrentQuestionViewModel.ShowHintCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing hint");
        }
    }

    /// <summary>
    /// Handles question completion events.
    /// </summary>
    private async void OnQuestionCompleted(object? sender, QuestionCompletedEventArgs e)
    {
        try
        {
            var validationResult = e.ValidationResult;
            var timeSpent = (int)(DateTime.UtcNow - _questionStartTime).TotalSeconds;

            // Record the answer
            _questionAnswers.Add(new QuestionAnswer
            {
                QuestionId = validationResult.PointsEarned > 0 ? Questions[CurrentQuestionIndex].Id : 0,
                UserAnswer = CurrentQuestionViewModel?.GetUserAnswer() ?? new object(),
                TimeSpentSeconds = timeSpent,
                AttemptNumber = validationResult.AttemptNumber
            });

            // Update progress tracking
            if (validationResult.IsCorrect)
            {
                CorrectAnswers++;
            }

            // Play appropriate feedback
            if (validationResult.IsCorrect)
            {
                await PlayCorrectAnswerFeedbackAsync(CorrectAnswers, ChildId);
                CanProceedToNext = true;

                // Auto-advance after a delay
                _ = Task.Delay(2000).ContinueWith(_ => NextQuestionAsync());
            }
            else
            {
                await PlayIncorrectAnswerFeedbackAsync(validationResult.AttemptNumber, ChildId);
            }

            UpdateProgressDisplay();
            _logger.LogInformation("Question completed: {IsCorrect}, attempt {AttemptNumber}", validationResult.IsCorrect, validationResult.AttemptNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling question completion");
        }
    }

    /// <summary>
    /// Handles question reset events.
    /// </summary>
    private void OnQuestionReset(object? sender, EventArgs e)
    {
        CanProceedToNext = false;
        _questionStartTime = DateTime.UtcNow; // Reset timer
    }

    #endregion

    #region Activity Completion

    /// <summary>
    /// Completes the activity and processes results.
    /// </summary>
    private async Task CompleteActivityAsync()
    {
        try
        {
            _logger.LogInformation("Completing activity {ActivityId} for child {ChildId}", ActivityId, ChildId);

            await ExecuteAsync(async () =>
            {
                var totalTimeSeconds = (int)(DateTime.UtcNow - _activityStartTime).TotalSeconds;

                // Validate all answers and get activity results
                var activityResults = await _answerValidationService.ValidateActivityAnswersAsync(
                    ActivityId, ChildId, _questionAnswers, totalTimeSeconds, CancellationToken.None);

                // Update progress and unlock new content
                var completionData = new ActivityCompletionData
                {
                    ActivityId = ActivityId,
                    StarsEarned = activityResults.StarsEarned,
                    CorrectAnswers = activityResults.CorrectAnswers,
                    TotalQuestions = activityResults.TotalQuestions,
                    TimeSpentSeconds = totalTimeSeconds,
                    IsFirstCompletion = activityResults.IsFirstCompletion,
                    CompletionTime = DateTime.UtcNow
                };

                var progressionResult = await _progressionService.ProcessActivityCompletionAsync(
                    ChildId, ActivityId, completionData, CancellationToken.None);

                // Update UI with results
                IsActivityCompleted = true;
                StarsEarned = activityResults.StarsEarned;
                CompletionMessage = activityResults.CompletionMessage;
                NewAchievements = progressionResult.NewAchievements;

                // Check for new achievements
                var newAchievements = await _answerValidationService.CheckForAchievementsAsync(
                    ChildId, activityResults, CancellationToken.None);
                NewAchievements = newAchievements.Select(a => new Achievement
                {
                    NameEn = a.NameEn ?? "",
                    DescriptionEn = a.DescriptionEn,
                    BadgeIcon = a.BadgeIcon
                }).ToList();

                _logger.LogInformation("Activity completed: {Stars} stars, {NewAchievements} new achievements, {UnlockedActivities} unlocked",
                    StarsEarned, NewAchievements.Count, progressionResult.NewlyUnlockedActivities.Count);

            }, Language == "es" ? "Guardando tu progreso..." : "Saving your progress...");

            // Show celebration
            await ShowCompletionCelebrationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing activity");
        }
    }

    /// <summary>
    /// Shows completion celebration with stars and achievements.
    /// </summary>
    private async Task ShowCompletionCelebrationAsync()
    {
        try
        {
            ShowCelebration = true;

            // Play completion audio based on performance
            await PlayCompletionFeedbackAsync(StarsEarned, CurrentActivity?.Subject?.NameEn ?? "activity");

            // Play achievement audio if any new achievements
            if (NewAchievements.Any())
            {
                await Task.Delay(2000);
                await PlayAudioAsync("audio/achievements/new_badge.mp3", AudioType.Achievement);
            }

            // Generate motivational content
            var motivationalContent = await _progressionService.GenerateMotivationalContentAsync(
                ChildId, new ActivityCompletionData
                {
                    ActivityId = ActivityId,
                    StarsEarned = StarsEarned,
                    CorrectAnswers = CorrectAnswers,
                    TotalQuestions = TotalQuestions,
                    IsFirstCompletion = true
                }, CancellationToken.None);

            if (motivationalContent.ShouldShowImmediately)
            {
                await _navigationService.ShowCelebrationAsync(motivationalContent.Title, motivationalContent.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing completion celebration");
        }
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Navigates back to the previous screen.
    /// </summary>
    private async Task GoBackAsync()
    {
        try
        {
            await StopBackgroundMusicAsync();
            await PlayBackNavigationAudioAsync();
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating back");
        }
    }

    /// <summary>
    /// Restarts the current activity.
    /// </summary>
    private async Task RestartActivityAsync()
    {
        try
        {
            _logger.LogInformation("Restarting activity {ActivityId}", ActivityId);

            // Reset all state
            CurrentQuestionIndex = 0;
            CorrectAnswers = 0;
            IsActivityCompleted = false;
            ShowCelebration = false;
            _questionAnswers.Clear();

            // Reload the activity
            await InitializeAsync(ChildId, ActivityId, Language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restarting activity");
        }
    }

    #endregion

    #region Audio Integration

    /// <summary>
    /// Plays activity welcome audio.
    /// </summary>
    private async Task PlayActivityWelcomeAsync()
    {
        try
        {
            if (CurrentActivity != null)
            {
                var audioPath = CurrentActivity.GetLocalizedAudioPath(Language);
                if (!string.IsNullOrEmpty(audioPath))
                {
                    await PlayAudioAsync(audioPath, AudioType.Instruction);
                }
                else
                {
                    // Fallback to generic welcome
                    await PlayActivityIntroductionAsync(CurrentActivity.Subject?.NameEn ?? "activity", ChildId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing welcome audio");
        }
    }

    /// <summary>
    /// Plays activity instructions.
    /// </summary>
    private async Task PlayInstructionsAsync()
    {
        try
        {
            if (CurrentActivity != null)
            {
                var instructions = CurrentActivity.GetLocalizedInstruction(Language);
                if (!string.IsNullOrEmpty(instructions))
                {
                    await PlayInstructionAsync($"activity_{ActivityId}_instructions", ChildId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing instructions");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates the progress display properties.
    /// </summary>
    private void UpdateProgressDisplay()
    {
        if (TotalQuestions > 0)
        {
            ProgressPercentage = (double)(CurrentQuestionIndex + 1) / TotalQuestions * 100;
            ProgressText = Language == "es"
                ? $"Pregunta {CurrentQuestionIndex + 1} de {TotalQuestions}"
                : $"Question {CurrentQuestionIndex + 1} of {TotalQuestions}";
        }
        else
        {
            ProgressPercentage = 0;
            ProgressText = string.Empty;
        }
    }

    /// <summary>
    /// Gets the child's age for age-appropriate content adaptation.
    /// </summary>
    private async Task<int> GetChildAgeAsync()
    {
        // TODO: Implement actual child age retrieval
        return 5; // Placeholder
    }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets the current question number for display.
    /// </summary>
    public int CurrentQuestionNumber => CurrentQuestionIndex + 1;

    /// <summary>
    /// Indicates if there are more questions to complete.
    /// </summary>
    public bool HasMoreQuestions => CurrentQuestionIndex < Questions.Count - 1;

    /// <summary>
    /// Gets the star display text.
    /// </summary>
    public string StarDisplayText => StarsEarned switch
    {
        3 => "🌟🌟🌟",
        2 => "🌟🌟⭐",
        1 => "🌟⭐⭐",
        _ => "⭐⭐⭐"
    };

    #endregion

    #region Cleanup

    protected virtual void OnDisposing()
    {
        if (CurrentQuestionViewModel != null)
        {
            CurrentQuestionViewModel.QuestionCompleted -= OnQuestionCompleted;
            CurrentQuestionViewModel.QuestionReset -= OnQuestionReset;
            CurrentQuestionViewModel?.Dispose();
        }

        base.OnDisposing();
    }

    #endregion
}