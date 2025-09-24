using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EduPlayKids.App.Controls;

/// <summary>
/// Child-friendly progress dashboard control for ages 3-8.
/// Features visual progress tracking, achievements display, and motivational elements.
/// Designed with large touch targets, positive reinforcement, and age-appropriate visuals.
/// </summary>
public partial class ProgressDashboardControl : ContentView
{
    private readonly IAudioService? _audioService;
    private readonly ILogger<ProgressDashboardControl>? _logger;
    private bool _isAnimating = false;

    #region Bindable Properties

    /// <summary>
    /// Child's avatar emoji for personalization.
    /// </summary>
    public static readonly BindableProperty ChildAvatarProperty =
        BindableProperty.Create(
            nameof(ChildAvatar),
            typeof(string),
            typeof(ProgressDashboardControl),
            "🧑");

    /// <summary>
    /// Welcome message displayed to the child.
    /// </summary>
    public static readonly BindableProperty WelcomeMessageProperty =
        BindableProperty.Create(
            nameof(WelcomeMessage),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Motivational message to encourage continued learning.
    /// </summary>
    public static readonly BindableProperty MotivationalMessageProperty =
        BindableProperty.Create(
            nameof(MotivationalMessage),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Overall progress percentage (0.0 to 1.0).
    /// </summary>
    public static readonly BindableProperty OverallProgressPercentageProperty =
        BindableProperty.Create(
            nameof(OverallProgressPercentage),
            typeof(double),
            typeof(ProgressDashboardControl),
            0.0,
            propertyChanged: OnProgressChanged);

    /// <summary>
    /// Description of current progress level.
    /// </summary>
    public static readonly BindableProperty ProgressDescriptionProperty =
        BindableProperty.Create(
            nameof(ProgressDescription),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Current level text (e.g., "Level 3").
    /// </summary>
    public static readonly BindableProperty CurrentLevelTextProperty =
        BindableProperty.Create(
            nameof(CurrentLevelText),
            typeof(string),
            typeof(ProgressDashboardControl),
            "Level 1");

    /// <summary>
    /// Points text display (e.g., "250 Stars").
    /// </summary>
    public static readonly BindableProperty PointsTextProperty =
        BindableProperty.Create(
            nameof(PointsText),
            typeof(string),
            typeof(ProgressDashboardControl),
            "0 Stars");

    // Subject Progress Properties
    public static readonly BindableProperty MathProgressProperty =
        BindableProperty.Create(nameof(MathProgress), typeof(double), typeof(ProgressDashboardControl), 0.0);

    public static readonly BindableProperty ReadingProgressProperty =
        BindableProperty.Create(nameof(ReadingProgress), typeof(double), typeof(ProgressDashboardControl), 0.0);

    public static readonly BindableProperty ScienceProgressProperty =
        BindableProperty.Create(nameof(ScienceProgress), typeof(double), typeof(ProgressDashboardControl), 0.0);

    public static readonly BindableProperty LogicProgressProperty =
        BindableProperty.Create(nameof(LogicProgress), typeof(double), typeof(ProgressDashboardControl), 0.0);

    public static readonly BindableProperty ConceptsProgressProperty =
        BindableProperty.Create(nameof(ConceptsProgress), typeof(double), typeof(ProgressDashboardControl), 0.0);

    // Subject Progress Text Properties
    public static readonly BindableProperty MathProgressTextProperty =
        BindableProperty.Create(nameof(MathProgressText), typeof(string), typeof(ProgressDashboardControl), "0%");

    public static readonly BindableProperty ReadingProgressTextProperty =
        BindableProperty.Create(nameof(ReadingProgressText), typeof(string), typeof(ProgressDashboardControl), "0%");

    public static readonly BindableProperty ScienceProgressTextProperty =
        BindableProperty.Create(nameof(ScienceProgressText), typeof(string), typeof(ProgressDashboardControl), "0%");

    public static readonly BindableProperty LogicProgressTextProperty =
        BindableProperty.Create(nameof(LogicProgressText), typeof(string), typeof(ProgressDashboardControl), "0%");

    public static readonly BindableProperty ConceptsProgressTextProperty =
        BindableProperty.Create(nameof(ConceptsProgressText), typeof(string), typeof(ProgressDashboardControl), "0%");

    /// <summary>
    /// Learning streak in days.
    /// </summary>
    public static readonly BindableProperty StreakDaysProperty =
        BindableProperty.Create(
            nameof(StreakDays),
            typeof(int),
            typeof(ProgressDashboardControl),
            0);

    /// <summary>
    /// Streak motivational message.
    /// </summary>
    public static readonly BindableProperty StreakMessageProperty =
        BindableProperty.Create(
            nameof(StreakMessage),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Weekly streak calendar display.
    /// </summary>
    public static readonly BindableProperty WeeklyStreakTextProperty =
        BindableProperty.Create(
            nameof(WeeklyStreakText),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Collection of recent achievements.
    /// </summary>
    public static readonly BindableProperty RecentAchievementsProperty =
        BindableProperty.Create(
            nameof(RecentAchievements),
            typeof(ObservableCollection<AchievementDisplayModel>),
            typeof(ProgressDashboardControl),
            new ObservableCollection<AchievementDisplayModel>());

    /// <summary>
    /// Next challenge icon.
    /// </summary>
    public static readonly BindableProperty NextChallengeIconProperty =
        BindableProperty.Create(
            nameof(NextChallengeIcon),
            typeof(string),
            typeof(ProgressDashboardControl),
            "🎯");

    /// <summary>
    /// Next challenge name.
    /// </summary>
    public static readonly BindableProperty NextChallengeNameProperty =
        BindableProperty.Create(
            nameof(NextChallengeName),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Next challenge description.
    /// </summary>
    public static readonly BindableProperty NextChallengeDescriptionProperty =
        BindableProperty.Create(
            nameof(NextChallengeDescription),
            typeof(string),
            typeof(ProgressDashboardControl),
            string.Empty);

    /// <summary>
    /// Command to start the next challenge.
    /// </summary>
    public static readonly BindableProperty StartNextChallengeCommandProperty =
        BindableProperty.Create(
            nameof(StartNextChallengeCommand),
            typeof(ICommand),
            typeof(ProgressDashboardControl),
            null);

    // Title Properties for Localization
    public static readonly BindableProperty ProgressTitleProperty =
        BindableProperty.Create(nameof(ProgressTitle), typeof(string), typeof(ProgressDashboardControl), "Your Progress");

    public static readonly BindableProperty SubjectsProgressTitleProperty =
        BindableProperty.Create(nameof(SubjectsProgressTitle), typeof(string), typeof(ProgressDashboardControl), "Subject Progress");

    public static readonly BindableProperty AchievementsTitleProperty =
        BindableProperty.Create(nameof(AchievementsTitle), typeof(string), typeof(ProgressDashboardControl), "Recent Achievements");

    public static readonly BindableProperty StreakTitleProperty =
        BindableProperty.Create(nameof(StreakTitle), typeof(string), typeof(ProgressDashboardControl), "Learning Streak");

    public static readonly BindableProperty NextChallengeTitleProperty =
        BindableProperty.Create(nameof(NextChallengeTitle), typeof(string), typeof(ProgressDashboardControl), "Next Challenge");

    public static readonly BindableProperty StartChallengeButtonTextProperty =
        BindableProperty.Create(nameof(StartChallengeButtonText), typeof(string), typeof(ProgressDashboardControl), "Start Learning!");

    // Visibility Properties
    public static readonly BindableProperty ShowAchievementsProperty =
        BindableProperty.Create(nameof(ShowAchievements), typeof(bool), typeof(ProgressDashboardControl), true);

    // Language Property
    public static readonly BindableProperty LanguageProperty =
        BindableProperty.Create(nameof(Language), typeof(string), typeof(ProgressDashboardControl), "en");

    #endregion

    #region Property Accessors

    public string ChildAvatar
    {
        get => (string)GetValue(ChildAvatarProperty);
        set => SetValue(ChildAvatarProperty, value);
    }

    public string WelcomeMessage
    {
        get => (string)GetValue(WelcomeMessageProperty);
        set => SetValue(WelcomeMessageProperty, value);
    }

    public string MotivationalMessage
    {
        get => (string)GetValue(MotivationalMessageProperty);
        set => SetValue(MotivationalMessageProperty, value);
    }

    public double OverallProgressPercentage
    {
        get => (double)GetValue(OverallProgressPercentageProperty);
        set => SetValue(OverallProgressPercentageProperty, value);
    }

    public string ProgressDescription
    {
        get => (string)GetValue(ProgressDescriptionProperty);
        set => SetValue(ProgressDescriptionProperty, value);
    }

    public string CurrentLevelText
    {
        get => (string)GetValue(CurrentLevelTextProperty);
        set => SetValue(CurrentLevelTextProperty, value);
    }

    public string PointsText
    {
        get => (string)GetValue(PointsTextProperty);
        set => SetValue(PointsTextProperty, value);
    }

    // Subject Progress Properties
    public double MathProgress
    {
        get => (double)GetValue(MathProgressProperty);
        set => SetValue(MathProgressProperty, value);
    }

    public double ReadingProgress
    {
        get => (double)GetValue(ReadingProgressProperty);
        set => SetValue(ReadingProgressProperty, value);
    }

    public double ScienceProgress
    {
        get => (double)GetValue(ScienceProgressProperty);
        set => SetValue(ScienceProgressProperty, value);
    }

    public double LogicProgress
    {
        get => (double)GetValue(LogicProgressProperty);
        set => SetValue(LogicProgressProperty, value);
    }

    public double ConceptsProgress
    {
        get => (double)GetValue(ConceptsProgressProperty);
        set => SetValue(ConceptsProgressProperty, value);
    }

    // Subject Progress Text Properties
    public string MathProgressText
    {
        get => (string)GetValue(MathProgressTextProperty);
        set => SetValue(MathProgressTextProperty, value);
    }

    public string ReadingProgressText
    {
        get => (string)GetValue(ReadingProgressTextProperty);
        set => SetValue(ReadingProgressTextProperty, value);
    }

    public string ScienceProgressText
    {
        get => (string)GetValue(ScienceProgressTextProperty);
        set => SetValue(ScienceProgressTextProperty, value);
    }

    public string LogicProgressText
    {
        get => (string)GetValue(LogicProgressTextProperty);
        set => SetValue(LogicProgressTextProperty, value);
    }

    public string ConceptsProgressText
    {
        get => (string)GetValue(ConceptsProgressTextProperty);
        set => SetValue(ConceptsProgressTextProperty, value);
    }

    public int StreakDays
    {
        get => (int)GetValue(StreakDaysProperty);
        set => SetValue(StreakDaysProperty, value);
    }

    public string StreakMessage
    {
        get => (string)GetValue(StreakMessageProperty);
        set => SetValue(StreakMessageProperty, value);
    }

    public string WeeklyStreakText
    {
        get => (string)GetValue(WeeklyStreakTextProperty);
        set => SetValue(WeeklyStreakTextProperty, value);
    }

    public ObservableCollection<AchievementDisplayModel> RecentAchievements
    {
        get => (ObservableCollection<AchievementDisplayModel>)GetValue(RecentAchievementsProperty);
        set => SetValue(RecentAchievementsProperty, value);
    }

    public string NextChallengeIcon
    {
        get => (string)GetValue(NextChallengeIconProperty);
        set => SetValue(NextChallengeIconProperty, value);
    }

    public string NextChallengeName
    {
        get => (string)GetValue(NextChallengeNameProperty);
        set => SetValue(NextChallengeNameProperty, value);
    }

    public string NextChallengeDescription
    {
        get => (string)GetValue(NextChallengeDescriptionProperty);
        set => SetValue(NextChallengeDescriptionProperty, value);
    }

    public ICommand StartNextChallengeCommand
    {
        get => (ICommand)GetValue(StartNextChallengeCommandProperty);
        set => SetValue(StartNextChallengeCommandProperty, value);
    }

    // Title Properties
    public string ProgressTitle
    {
        get => (string)GetValue(ProgressTitleProperty);
        set => SetValue(ProgressTitleProperty, value);
    }

    public string SubjectsProgressTitle
    {
        get => (string)GetValue(SubjectsProgressTitleProperty);
        set => SetValue(SubjectsProgressTitleProperty, value);
    }

    public string AchievementsTitle
    {
        get => (string)GetValue(AchievementsTitleProperty);
        set => SetValue(AchievementsTitleProperty, value);
    }

    public string StreakTitle
    {
        get => (string)GetValue(StreakTitleProperty);
        set => SetValue(StreakTitleProperty, value);
    }

    public string NextChallengeTitle
    {
        get => (string)GetValue(NextChallengeTitleProperty);
        set => SetValue(NextChallengeTitleProperty, value);
    }

    public string StartChallengeButtonText
    {
        get => (string)GetValue(StartChallengeButtonTextProperty);
        set => SetValue(StartChallengeButtonTextProperty, value);
    }

    public bool ShowAchievements
    {
        get => (bool)GetValue(ShowAchievementsProperty);
        set => SetValue(ShowAchievementsProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Fired when progress is updated.
    /// </summary>
    public event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

    /// <summary>
    /// Fired when a new achievement is unlocked.
    /// </summary>
    public event EventHandler<AchievementUnlockedEventArgs>? AchievementUnlocked;

    /// <summary>
    /// Fired when a level is completed.
    /// </summary>
    public event EventHandler<LevelCompletedEventArgs>? LevelCompleted;

    #endregion

    #region Constructor

    public ProgressDashboardControl()
    {
        InitializeComponent();

        // Try to get services from dependency injection if available
        try
        {
            _audioService = ServiceHelper.GetService<IAudioService>();
            _logger = ServiceHelper.GetService<ILogger<ProgressDashboardControl>>();
        }
        catch
        {
            // Services not available, will work without audio feedback
        }

        InitializeDefaults();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes default values and localizations.
    /// </summary>
    private void InitializeDefaults()
    {
        // Set default localized text
        UpdateLocalizedText();

        _logger?.LogDebug("ProgressDashboardControl initialized");
    }

    /// <summary>
    /// Updates localized text based on current language.
    /// </summary>
    private void UpdateLocalizedText()
    {
        if (Language == "es")
        {
            ProgressTitle = "Tu Progreso";
            SubjectsProgressTitle = "Progreso por Materia";
            AchievementsTitle = "Logros Recientes";
            StreakTitle = "Racha de Aprendizaje";
            NextChallengeTitle = "Próximo Desafío";
            StartChallengeButtonText = "¡Empezar a Aprender!";

            if (string.IsNullOrEmpty(WelcomeMessage))
                WelcomeMessage = "¡Hola, pequeño explorador!";

            if (string.IsNullOrEmpty(MotivationalMessage))
                MotivationalMessage = "¡Sigues aprendiendo increíble!";
        }
        else
        {
            ProgressTitle = "Your Progress";
            SubjectsProgressTitle = "Subject Progress";
            AchievementsTitle = "Recent Achievements";
            StreakTitle = "Learning Streak";
            NextChallengeTitle = "Next Challenge";
            StartChallengeButtonText = "Start Learning!";

            if (string.IsNullOrEmpty(WelcomeMessage))
                WelcomeMessage = "Hello, little explorer!";

            if (string.IsNullOrEmpty(MotivationalMessage))
                MotivationalMessage = "You're doing amazing!";
        }
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles overall progress changes with animation.
    /// </summary>
    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProgressDashboardControl control && newValue is double newProgress)
        {
            control.UpdateProgressWithAnimation(newProgress);
        }
    }

    #endregion

    #region Progress Management

    /// <summary>
    /// Updates progress display with celebration animations.
    /// </summary>
    /// <param name="newProgress">New progress value (0.0 to 1.0)</param>
    private async void UpdateProgressWithAnimation(double newProgress)
    {
        if (_isAnimating) return;

        var clampedProgress = Math.Max(0.0, Math.Min(1.0, newProgress));

        _logger?.LogInformation("Updating progress to {Progress:P0}", clampedProgress);

        try
        {
            _isAnimating = true;

            // Play progress update sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/progress_update.mp3", AudioType.UIInteraction);
            }

            // Animate progress container
            await ProgressContainer.ScaleTo(1.05, 300, Easing.BounceOut);
            await ProgressContainer.ScaleTo(1.0, 200, Easing.BounceOut);

            // Update progress description based on level
            UpdateProgressDescription(clampedProgress);

            // Check for level completion
            CheckLevelCompletion(clampedProgress);

            ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs
            {
                NewProgress = clampedProgress,
                ProgressDescription = ProgressDescription
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating progress display");
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <summary>
    /// Updates progress description based on current level.
    /// </summary>
    /// <param name="progress">Current progress (0.0 to 1.0)</param>
    private void UpdateProgressDescription(double progress)
    {
        var percentage = (int)(progress * 100);

        ProgressDescription = Language == "es"
            ? GetSpanishProgressDescription(percentage)
            : GetEnglishProgressDescription(percentage);
    }

    /// <summary>
    /// Gets English progress description based on percentage.
    /// </summary>
    private string GetEnglishProgressDescription(int percentage)
    {
        return percentage switch
        {
            >= 90 => "Almost there!",
            >= 75 => "Great progress!",
            >= 50 => "Halfway there!",
            >= 25 => "Good start!",
            > 0 => "Getting started!",
            _ => "Let's begin!"
        };
    }

    /// <summary>
    /// Gets Spanish progress description based on percentage.
    /// </summary>
    private string GetSpanishProgressDescription(int percentage)
    {
        return percentage switch
        {
            >= 90 => "¡Casi terminado!",
            >= 75 => "¡Excelente progreso!",
            >= 50 => "¡A la mitad!",
            >= 25 => "¡Buen comienzo!",
            > 0 => "¡Empezando!",
            _ => "¡Comencemos!"
        };
    }

    /// <summary>
    /// Checks for level completion and triggers celebration.
    /// </summary>
    /// <param name="progress">Current progress</param>
    private async void CheckLevelCompletion(double progress)
    {
        // Check if progress represents a level completion (multiple of 0.2 = 20%)
        if (progress > 0 && progress % 0.2 < 0.01) // Allow for floating point precision
        {
            await TriggerLevelCompletionCelebration();
        }
    }

    #endregion

    #region Subject Progress Management

    /// <summary>
    /// Updates all subject progress bars with animation.
    /// </summary>
    /// <param name="mathProgress">Math progress (0.0 to 1.0)</param>
    /// <param name="readingProgress">Reading progress (0.0 to 1.0)</param>
    /// <param name="scienceProgress">Science progress (0.0 to 1.0)</param>
    /// <param name="logicProgress">Logic progress (0.0 to 1.0)</param>
    /// <param name="conceptsProgress">Concepts progress (0.0 to 1.0)</param>
    public async Task UpdateSubjectProgressAsync(
        double mathProgress,
        double readingProgress,
        double scienceProgress,
        double logicProgress,
        double conceptsProgress)
    {
        // Update progress values
        MathProgress = Math.Max(0.0, Math.Min(1.0, mathProgress));
        ReadingProgress = Math.Max(0.0, Math.Min(1.0, readingProgress));
        ScienceProgress = Math.Max(0.0, Math.Min(1.0, scienceProgress));
        LogicProgress = Math.Max(0.0, Math.Min(1.0, logicProgress));
        ConceptsProgress = Math.Max(0.0, Math.Min(1.0, conceptsProgress));

        // Update text displays
        MathProgressText = $"{(int)(MathProgress * 100)}%";
        ReadingProgressText = $"{(int)(ReadingProgress * 100)}%";
        ScienceProgressText = $"{(int)(ScienceProgress * 100)}%";
        LogicProgressText = $"{(int)(LogicProgress * 100)}%";
        ConceptsProgressText = $"{(int)(ConceptsProgress * 100)}%";

        // Play progress update sound
        if (_audioService != null)
        {
            await _audioService.PlayAudioAsync("audio/ui/subject_progress_update.mp3", AudioType.UIInteraction);
        }

        _logger?.LogInformation("Updated subject progress: Math={Math:P0}, Reading={Reading:P0}, Science={Science:P0}, Logic={Logic:P0}, Concepts={Concepts:P0}",
            MathProgress, ReadingProgress, ScienceProgress, LogicProgress, ConceptsProgress);
    }

    #endregion

    #region Achievement Management

    /// <summary>
    /// Adds a new achievement with celebration animation.
    /// </summary>
    /// <param name="achievement">Achievement to add</param>
    public async Task AddAchievementAsync(AchievementDisplayModel achievement)
    {
        if (achievement == null) return;

        try
        {
            // Add to collection
            RecentAchievements.Insert(0, achievement);

            // Keep only recent achievements (max 5)
            while (RecentAchievements.Count > 5)
            {
                RecentAchievements.RemoveAt(RecentAchievements.Count - 1);
            }

            // Show new achievement animation
            await ShowNewAchievementAnimation(achievement);

            // Play achievement sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/achievements/new_achievement.mp3", AudioType.Achievement);
            }

            AchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs
            {
                Achievement = achievement
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding achievement");
        }
    }

    /// <summary>
    /// Shows new achievement unlock animation.
    /// </summary>
    private async Task ShowNewAchievementAnimation(AchievementDisplayModel achievement)
    {
        try
        {
            NewAchievementBadge.IsVisible = true;

            // Scale and fade in animation
            await Task.WhenAll(
                NewAchievementBadge.ScaleTo(1.0, 600, Easing.BounceOut),
                NewAchievementBadge.FadeTo(1.0, 400)
            );

            // Rotation animation
            await NewAchievementBadge.RotateTo(360, 800, Easing.SpringOut);
            NewAchievementBadge.Rotation = 0; // Reset rotation

            // Keep visible for 2 seconds
            await Task.Delay(2000);

            // Fade out
            await NewAchievementBadge.FadeTo(0, 500);
            NewAchievementBadge.IsVisible = false;
            NewAchievementBadge.Scale = 0; // Reset scale
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in new achievement animation");
        }
    }

    #endregion

    #region Level Completion

    /// <summary>
    /// Triggers level completion celebration.
    /// </summary>
    private async Task TriggerLevelCompletionCelebration()
    {
        try
        {
            // Play level completion sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/achievements/level_complete.mp3", AudioType.Achievement);
            }

            // Show confetti animation
            await ShowLevelUpConfetti();

            LevelCompleted?.Invoke(this, new LevelCompletedEventArgs
            {
                NewLevel = GetCurrentLevel(),
                ProgressPercentage = OverallProgressPercentage
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in level completion celebration");
        }
    }

    /// <summary>
    /// Shows level up confetti animation.
    /// </summary>
    private async Task ShowLevelUpConfetti()
    {
        try
        {
            LevelUpOverlay.IsVisible = true;

            // Reset confetti positions
            LevelConfetti1.Opacity = 1;
            LevelConfetti2.Opacity = 1;
            LevelConfetti3.Opacity = 1;
            LevelConfetti1.TranslationY = 0;
            LevelConfetti2.TranslationY = 0;
            LevelConfetti3.TranslationY = 0;

            // Animate confetti falling
            var tasks = new List<Task>
            {
                LevelConfetti1.TranslateTo(0, 150, 1500, Easing.CubicIn),
                LevelConfetti2.TranslateTo(0, 140, 1600, Easing.CubicIn),
                LevelConfetti3.TranslateTo(0, 160, 1400, Easing.CubicIn),
                LevelConfetti1.FadeTo(0, 1500),
                LevelConfetti2.FadeTo(0, 1600),
                LevelConfetti3.FadeTo(0, 1400)
            };

            await Task.WhenAll(tasks);

            // Hide overlay
            LevelUpOverlay.IsVisible = false;

            // Reset positions
            LevelConfetti1.TranslationY = 0;
            LevelConfetti2.TranslationY = 0;
            LevelConfetti3.TranslationY = 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in level up confetti animation");
        }
    }

    /// <summary>
    /// Gets current level based on progress.
    /// </summary>
    private int GetCurrentLevel()
    {
        return (int)(OverallProgressPercentage * 10) + 1; // 10 levels total
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates learning streak display.
    /// </summary>
    /// <param name="days">Number of consecutive days</param>
    /// <param name="weeklyPattern">Weekly learning pattern (e.g., "SMTWTFS")</param>
    public void UpdateLearningStreak(int days, string weeklyPattern = "")
    {
        StreakDays = Math.Max(0, days);

        StreakMessage = Language == "es"
            ? GetSpanishStreakMessage(days)
            : GetEnglishStreakMessage(days);

        WeeklyStreakText = string.IsNullOrEmpty(weeklyPattern)
            ? (Language == "es" ? "Esta semana" : "This week")
            : weeklyPattern;
    }

    /// <summary>
    /// Gets English streak message.
    /// </summary>
    private string GetEnglishStreakMessage(int days)
    {
        return days switch
        {
            0 => "Start your streak today!",
            1 => "Great start!",
            >= 7 => "Amazing streak!",
            >= 3 => "Keep it up!",
            _ => "Good job!"
        };
    }

    /// <summary>
    /// Gets Spanish streak message.
    /// </summary>
    private string GetSpanishStreakMessage(int days)
    {
        return days switch
        {
            0 => "¡Comienza tu racha hoy!",
            1 => "¡Excelente comienzo!",
            >= 7 => "¡Racha increíble!",
            >= 3 => "¡Sigue así!",
            _ => "¡Buen trabajo!"
        };
    }

    /// <summary>
    /// Updates next challenge information.
    /// </summary>
    /// <param name="icon">Challenge icon</param>
    /// <param name="name">Challenge name</param>
    /// <param name="description">Challenge description</param>
    public void UpdateNextChallenge(string icon, string name, string description)
    {
        NextChallengeIcon = icon ?? "🎯";
        NextChallengeName = name ?? "";
        NextChallengeDescription = description ?? "";
    }

    /// <summary>
    /// Resets all progress displays to initial state.
    /// </summary>
    public void Reset()
    {
        OverallProgressPercentage = 0.0;
        MathProgress = ReadingProgress = ScienceProgress = LogicProgress = ConceptsProgress = 0.0;
        StreakDays = 0;
        RecentAchievements.Clear();
        UpdateLocalizedText();
    }

    #endregion
}

#region Event Args and Models

/// <summary>
/// Event arguments for progress updated events.
/// </summary>
public class ProgressUpdatedEventArgs : EventArgs
{
    public double NewProgress { get; set; }
    public string ProgressDescription { get; set; } = string.Empty;
}

/// <summary>
/// Event arguments for achievement unlocked events.
/// </summary>
public class AchievementUnlockedEventArgs : EventArgs
{
    public AchievementDisplayModel Achievement { get; set; } = null!;
}

/// <summary>
/// Event arguments for level completed events.
/// </summary>
public class LevelCompletedEventArgs : EventArgs
{
    public int NewLevel { get; set; }
    public double ProgressPercentage { get; set; }
}

/// <summary>
/// Model for displaying achievements in the dashboard.
/// </summary>
public class AchievementDisplayModel
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

#endregion

