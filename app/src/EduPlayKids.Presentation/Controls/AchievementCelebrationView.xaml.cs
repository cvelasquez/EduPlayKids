using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EduPlayKids.App.Controls;

/// <summary>
/// Achievement celebration view designed for children ages 3-8.
/// Features confetti animations, positive reinforcement, and joyful visual feedback.
/// Celebrates learning milestones with age-appropriate encouragement and motivational content.
/// </summary>
public partial class AchievementCelebrationView : ContentView
{
    private readonly IAudioService? _audioService;
    private readonly ILogger<AchievementCelebrationView>? _logger;
    private bool _isAnimating = false;
    private bool _isVisible = false;

    #region Bindable Properties

    /// <summary>
    /// Main celebration title.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(AchievementCelebrationView),
            "🎉 Congratulations!");

    /// <summary>
    /// Celebration icon (emoji).
    /// </summary>
    public static readonly BindableProperty CelebrationIconProperty =
        BindableProperty.Create(
            nameof(CelebrationIcon),
            typeof(string),
            typeof(AchievementCelebrationView),
            "🎉");

    /// <summary>
    /// Number of stars earned (0-3).
    /// </summary>
    public static readonly BindableProperty StarsEarnedProperty =
        BindableProperty.Create(
            nameof(StarsEarned),
            typeof(int),
            typeof(AchievementCelebrationView),
            0,
            propertyChanged: OnStarsEarnedChanged);

    /// <summary>
    /// Text explaining stars earned.
    /// </summary>
    public static readonly BindableProperty StarsEarnedTextProperty =
        BindableProperty.Create(
            nameof(StarsEarnedText),
            typeof(string),
            typeof(AchievementCelebrationView),
            "You earned:");

    /// <summary>
    /// Performance feedback message.
    /// </summary>
    public static readonly BindableProperty PerformanceMessageProperty =
        BindableProperty.Create(
            nameof(PerformanceMessage),
            typeof(string),
            typeof(AchievementCelebrationView),
            "Amazing work!");

    /// <summary>
    /// Encouraging message for motivation.
    /// </summary>
    public static readonly BindableProperty EncouragingMessageProperty =
        BindableProperty.Create(
            nameof(EncouragingMessage),
            typeof(string),
            typeof(AchievementCelebrationView),
            "You're doing fantastic!");

    /// <summary>
    /// Additional motivational message.
    /// </summary>
    public static readonly BindableProperty MotivationalMessageProperty =
        BindableProperty.Create(
            nameof(MotivationalMessage),
            typeof(string),
            typeof(AchievementCelebrationView),
            "Keep up the great learning!");

    /// <summary>
    /// Achievement title for badges section.
    /// </summary>
    public static readonly BindableProperty AchievementTitleProperty =
        BindableProperty.Create(
            nameof(AchievementTitle),
            typeof(string),
            typeof(AchievementCelebrationView),
            "🏆 New Achievements!");

    /// <summary>
    /// Achievement description.
    /// </summary>
    public static readonly BindableProperty AchievementDescriptionProperty =
        BindableProperty.Create(
            nameof(AchievementDescription),
            typeof(string),
            typeof(AchievementCelebrationView),
            "You've unlocked special badges!");

    /// <summary>
    /// Progress section title.
    /// </summary>
    public static readonly BindableProperty ProgressTitleProperty =
        BindableProperty.Create(
            nameof(ProgressTitle),
            typeof(string),
            typeof(AchievementCelebrationView),
            "Learning Progress");

    /// <summary>
    /// Progress text description.
    /// </summary>
    public static readonly BindableProperty ProgressTextProperty =
        BindableProperty.Create(
            nameof(ProgressText),
            typeof(string),
            typeof(AchievementCelebrationView),
            "Keep going!");

    /// <summary>
    /// Progress value (0.0 to 1.0).
    /// </summary>
    public static readonly BindableProperty ProgressValueProperty =
        BindableProperty.Create(
            nameof(ProgressValue),
            typeof(double),
            typeof(AchievementCelebrationView),
            0.0);

    /// <summary>
    /// Continue button text.
    /// </summary>
    public static readonly BindableProperty ContinueButtonTextProperty =
        BindableProperty.Create(
            nameof(ContinueButtonText),
            typeof(string),
            typeof(AchievementCelebrationView),
            "🚀 Continue Learning");

    /// <summary>
    /// Share button text.
    /// </summary>
    public static readonly BindableProperty ShareButtonTextProperty =
        BindableProperty.Create(
            nameof(ShareButtonText),
            typeof(string),
            typeof(AchievementCelebrationView),
            "🎉 Celebrate More");

    /// <summary>
    /// Whether to show achievements section.
    /// </summary>
    public static readonly BindableProperty ShowAchievementsProperty =
        BindableProperty.Create(
            nameof(ShowAchievements),
            typeof(bool),
            typeof(AchievementCelebrationView),
            false);

    /// <summary>
    /// Whether to show progress section.
    /// </summary>
    public static readonly BindableProperty ShowProgressProperty =
        BindableProperty.Create(
            nameof(ShowProgress),
            typeof(bool),
            typeof(AchievementCelebrationView),
            false);

    /// <summary>
    /// Whether to show share button.
    /// </summary>
    public static readonly BindableProperty ShowShareButtonProperty =
        BindableProperty.Create(
            nameof(ShowShareButton),
            typeof(bool),
            typeof(AchievementCelebrationView),
            true);

    /// <summary>
    /// Whether to allow tap to dismiss.
    /// </summary>
    public static readonly BindableProperty AllowTapToDismissProperty =
        BindableProperty.Create(
            nameof(AllowTapToDismiss),
            typeof(bool),
            typeof(AchievementCelebrationView),
            true);

    /// <summary>
    /// Language for localization.
    /// </summary>
    public static readonly BindableProperty LanguageProperty =
        BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(AchievementCelebrationView),
            "en");

    /// <summary>
    /// Animation duration multiplier.
    /// </summary>
    public static readonly BindableProperty AnimationSpeedProperty =
        BindableProperty.Create(
            nameof(AnimationSpeed),
            typeof(double),
            typeof(AchievementCelebrationView),
            1.0);

    // Command Properties
    public static readonly BindableProperty ContinueCommandProperty =
        BindableProperty.Create(
            nameof(ContinueCommand),
            typeof(ICommand),
            typeof(AchievementCelebrationView),
            null);

    public static readonly BindableProperty ShareCommandProperty =
        BindableProperty.Create(
            nameof(ShareCommand),
            typeof(ICommand),
            typeof(AchievementCelebrationView),
            null);

    #region Property Accessors

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string CelebrationIcon
    {
        get => (string)GetValue(CelebrationIconProperty);
        set => SetValue(CelebrationIconProperty, value);
    }

    public int StarsEarned
    {
        get => (int)GetValue(StarsEarnedProperty);
        set => SetValue(StarsEarnedProperty, value);
    }

    public string StarsEarnedText
    {
        get => (string)GetValue(StarsEarnedTextProperty);
        set => SetValue(StarsEarnedTextProperty, value);
    }

    public string PerformanceMessage
    {
        get => (string)GetValue(PerformanceMessageProperty);
        set => SetValue(PerformanceMessageProperty, value);
    }

    public string EncouragingMessage
    {
        get => (string)GetValue(EncouragingMessageProperty);
        set => SetValue(EncouragingMessageProperty, value);
    }

    public string MotivationalMessage
    {
        get => (string)GetValue(MotivationalMessageProperty);
        set => SetValue(MotivationalMessageProperty, value);
    }

    public string AchievementTitle
    {
        get => (string)GetValue(AchievementTitleProperty);
        set => SetValue(AchievementTitleProperty, value);
    }

    public string AchievementDescription
    {
        get => (string)GetValue(AchievementDescriptionProperty);
        set => SetValue(AchievementDescriptionProperty, value);
    }

    public string ProgressTitle
    {
        get => (string)GetValue(ProgressTitleProperty);
        set => SetValue(ProgressTitleProperty, value);
    }

    public string ProgressText
    {
        get => (string)GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }

    public double ProgressValue
    {
        get => (double)GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    public string ContinueButtonText
    {
        get => (string)GetValue(ContinueButtonTextProperty);
        set => SetValue(ContinueButtonTextProperty, value);
    }

    public string ShareButtonText
    {
        get => (string)GetValue(ShareButtonTextProperty);
        set => SetValue(ShareButtonTextProperty, value);
    }

    public bool ShowAchievements
    {
        get => (bool)GetValue(ShowAchievementsProperty);
        set => SetValue(ShowAchievementsProperty, value);
    }

    public bool ShowProgress
    {
        get => (bool)GetValue(ShowProgressProperty);
        set => SetValue(ShowProgressProperty, value);
    }

    public bool ShowShareButton
    {
        get => (bool)GetValue(ShowShareButtonProperty);
        set => SetValue(ShowShareButtonProperty, value);
    }

    public bool AllowTapToDismiss
    {
        get => (bool)GetValue(AllowTapToDismissProperty);
        set => SetValue(AllowTapToDismissProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public double AnimationSpeed
    {
        get => (double)GetValue(AnimationSpeedProperty);
        set => SetValue(AnimationSpeedProperty, value);
    }

    public ICommand ContinueCommand
    {
        get => (ICommand)GetValue(ContinueCommandProperty);
        set => SetValue(ContinueCommandProperty, value);
    }

    public ICommand ShareCommand
    {
        get => (ICommand)GetValue(ShareCommandProperty);
        set => SetValue(ShareCommandProperty, value);
    }

    #endregion

    #endregion

    #region Events

    /// <summary>
    /// Fired when celebration animation starts.
    /// </summary>
    public event EventHandler<CelebrationEventArgs>? CelebrationStarted;

    /// <summary>
    /// Fired when celebration animation completes.
    /// </summary>
    public event EventHandler<CelebrationEventArgs>? CelebrationCompleted;

    /// <summary>
    /// Fired when celebration is dismissed.
    /// </summary>
    public event EventHandler<CelebrationEventArgs>? CelebrationDismissed;

    /// <summary>
    /// Fired when continue button is pressed.
    /// </summary>
    public event EventHandler<CelebrationEventArgs>? ContinueRequested;

    /// <summary>
    /// Fired when share button is pressed.
    /// </summary>
    public event EventHandler<CelebrationEventArgs>? ShareRequested;

    #endregion

    #region Constructor

    public AchievementCelebrationView()
    {
        InitializeComponent();

        // Try to get services from dependency injection
        try
        {
            _audioService = ServiceHelper.GetService<IAudioService>();
            _logger = ServiceHelper.GetService<ILogger<AchievementCelebrationView>>();
        }
        catch
        {
            // Services not available, will work without audio feedback
        }

        InitializeCelebration();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the celebration view with default settings.
    /// </summary>
    private void InitializeCelebration()
    {
        // Initially hidden
        CelebrationOverlay.IsVisible = false;

        // Set up default commands if none provided
        if (ContinueCommand == null)
        {
            ContinueCommand = new Command(OnContinueClicked);
        }

        if (ShareCommand == null)
        {
            ShareCommand = new Command(OnShareClicked);
        }

        _logger?.LogDebug("AchievementCelebrationView initialized");
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to the StarsEarned property.
    /// </summary>
    private static void OnStarsEarnedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AchievementCelebrationView control && newValue is int stars)
        {
            control.UpdateStarDisplay(stars);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Shows the celebration with full animation sequence.
    /// </summary>
    /// <param name="starsEarned">Number of stars earned (0-3)</param>
    /// <param name="celebrationType">Type of celebration to show</param>
    /// <param name="achievements">List of achievements earned</param>
    public async Task ShowCelebrationAsync(int starsEarned, CelebrationType celebrationType = CelebrationType.Activity, List<string>? achievements = null)
    {
        if (_isAnimating || _isVisible) return;

        try
        {
            _isAnimating = true;
            _isVisible = true;

            _logger?.LogInformation("Starting celebration animation with {Stars} stars, type: {Type}", starsEarned, celebrationType);

            // Configure celebration based on type
            ConfigureCelebration(starsEarned, celebrationType);

            // Add achievements if provided
            if (achievements?.Any() == true)
            {
                AddAchievementBadges(achievements);
                ShowAchievements = true;
            }

            // Fire event
            CelebrationStarted?.Invoke(this, new CelebrationEventArgs
            {
                StarsEarned = starsEarned,
                CelebrationType = celebrationType,
                Language = Language
            });

            // Make visible and reset initial state
            CelebrationOverlay.IsVisible = true;
            await ResetAnimationState();

            // Play celebration audio
            if (_audioService != null)
            {
                await PlayCelebrationAudioAsync(starsEarned, celebrationType);
            }

            // Start main animation sequence
            await PlayMainCelebrationAnimationAsync();

            // Start confetti
            _ = PlayConfettiAnimationAsync();

            // Start continuous sparkles
            _ = StartContinuousSparkleAsync();

            // Fire completion event
            CelebrationCompleted?.Invoke(this, new CelebrationEventArgs
            {
                StarsEarned = starsEarned,
                CelebrationType = celebrationType,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in celebration animation");
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <summary>
    /// Hides the celebration with fade out animation.
    /// </summary>
    public async Task HideCelebrationAsync()
    {
        if (!_isVisible) return;

        try
        {
            _logger?.LogInformation("Hiding celebration");

            await CelebrationContainer.FadeTo(0, 300);
            await CelebrationContainer.ScaleTo(0.8, 200);

            CelebrationOverlay.IsVisible = false;
            _isVisible = false;

            CelebrationDismissed?.Invoke(this, new CelebrationEventArgs
            {
                StarsEarned = StarsEarned,
                CelebrationType = CelebrationType.Activity,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error hiding celebration");
        }
    }

    /// <summary>
    /// Configures celebration for specific achievement scenarios.
    /// </summary>
    /// <param name="childAge">Child's age for age-appropriate messaging</param>
    /// <param name="subjectName">Subject name for context</param>
    /// <param name="starsEarned">Number of stars earned</param>
    /// <param name="language">Language preference</param>
    public void ConfigureForEducationalContext(int childAge, string subjectName, int starsEarned, string language = "en")
    {
        Language = language;
        StarsEarned = starsEarned;

        if (language == "es")
        {
            ConfigureSpanishMessages(childAge, subjectName, starsEarned);
        }
        else
        {
            ConfigureEnglishMessages(childAge, subjectName, starsEarned);
        }

        _logger?.LogInformation("Celebration configured for age {Age}, subject {Subject}, stars {Stars}, language {Language}",
            childAge, subjectName, starsEarned, language);
    }

    /// <summary>
    /// Adds achievement badges to the celebration.
    /// </summary>
    /// <param name="achievementIcons">List of achievement icons/badges</param>
    public void AddAchievementBadges(List<string> achievementIcons)
    {
        try
        {
            AchievementIcons.Children.Clear();

            foreach (var icon in achievementIcons.Take(5)) // Limit to 5 achievements for display
            {
                var achievementLabel = new Label
                {
                    Text = icon,
                    FontSize = 32,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                AchievementIcons.Children.Add(achievementLabel);
            }

            ShowAchievements = achievementIcons.Any();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding achievement badges");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Configures celebration content based on type and stars.
    /// </summary>
    private void ConfigureCelebration(int starsEarned, CelebrationType celebrationType)
    {
        StarsEarned = starsEarned;

        // Set celebration icon based on performance
        CelebrationIcon = starsEarned switch
        {
            3 => "🎉",
            2 => "🎊",
            1 => "👏",
            _ => "🌟"
        };

        // Configure based on celebration type
        switch (celebrationType)
        {
            case CelebrationType.Activity:
                Title = Language == "es" ? "¡Actividad Completada!" : "Activity Completed!";
                break;
            case CelebrationType.Achievement:
                Title = Language == "es" ? "¡Nuevo Logro!" : "New Achievement!";
                break;
            case CelebrationType.LevelUp:
                Title = Language == "es" ? "¡Nivel Completado!" : "Level Up!";
                break;
            case CelebrationType.CrownChallenge:
                Title = Language == "es" ? "¡Desafío Corona!" : "Crown Challenge!";
                CelebrationIcon = "👑";
                break;
        }
    }

    /// <summary>
    /// Configures English celebration messages.
    /// </summary>
    private void ConfigureEnglishMessages(int childAge, string subjectName, int starsEarned)
    {
        var ageGroup = childAge switch
        {
            <= 4 => "preschool",
            5 => "kindergarten",
            _ => "elementary"
        };

        StarsEarnedText = "You earned:";

        PerformanceMessage = starsEarned switch
        {
            3 => "Perfect! You're a star!",
            2 => "Great job! Almost perfect!",
            1 => "Good work! Keep trying!",
            _ => "Nice effort! Try again!"
        };

        EncouragingMessage = ageGroup switch
        {
            "preschool" => "You're learning so much!",
            "kindergarten" => "You're getting really good at this!",
            _ => "You're becoming an expert!"
        };

        MotivationalMessage = $"Keep exploring {subjectName}!";
        ContinueButtonText = "🚀 Keep Learning";
        ShareButtonText = "🎉 Celebrate More";
    }

    /// <summary>
    /// Configures Spanish celebration messages.
    /// </summary>
    private void ConfigureSpanishMessages(int childAge, string subjectName, int starsEarned)
    {
        var ageGroup = childAge switch
        {
            <= 4 => "preescolar",
            5 => "kindergarten",
            _ => "primaria"
        };

        StarsEarnedText = "Ganaste:";

        PerformanceMessage = starsEarned switch
        {
            3 => "¡Perfecto! ¡Eres una estrella!",
            2 => "¡Muy bien! ¡Casi perfecto!",
            1 => "¡Buen trabajo! ¡Sigue intentando!",
            _ => "¡Buen esfuerzo! ¡Inténtalo otra vez!"
        };

        EncouragingMessage = ageGroup switch
        {
            "preescolar" => "¡Estás aprendiendo mucho!",
            "kindergarten" => "¡Te estás volviendo muy bueno en esto!",
            _ => "¡Te estás convirtiendo en un experto!"
        };

        MotivationalMessage = $"¡Sigue explorando {subjectName}!";
        ContinueButtonText = "🚀 Seguir Aprendiendo";
        ShareButtonText = "🎉 Celebrar Más";
    }

    /// <summary>
    /// Updates the star display based on earned count.
    /// </summary>
    private void UpdateStarDisplay(int starsEarned)
    {
        var clampedStars = Math.Max(0, Math.Min(3, starsEarned));

        Star1.Text = clampedStars >= 1 ? "🌟" : "⭐";
        Star1.TextColor = clampedStars >= 1 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");

        Star2.Text = clampedStars >= 2 ? "🌟" : "⭐";
        Star2.TextColor = clampedStars >= 2 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");

        Star3.Text = clampedStars >= 3 ? "🌟" : "⭐";
        Star3.TextColor = clampedStars >= 3 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");
    }

    /// <summary>
    /// Resets all animation elements to initial state.
    /// </summary>
    private async Task ResetAnimationState()
    {
        // Reset main container
        CelebrationContainer.Scale = 0;
        CelebrationContainer.Opacity = 0;
        CelebrationContainer.TranslationY = 0;

        // Reset title
        CelebrationTitle.TranslationY = -100;

        // Reset star container
        StarContainer.Scale = 0;

        // Reset confetti
        var confettiElements = new[] { Confetti1, Confetti2, Confetti3, Confetti4, Confetti5, Confetti6 };
        foreach (var confetti in confettiElements)
        {
            confetti.TranslationY = 0;
            confetti.Opacity = 0;
            confetti.Rotation = 0;
        }

        await Task.Delay(50); // Small delay to ensure state is set
    }

    /// <summary>
    /// Plays the main celebration animation sequence.
    /// </summary>
    private async Task PlayMainCelebrationAnimationAsync()
    {
        try
        {
            var speed = AnimationSpeed;

            // Container appearance
            await Task.WhenAll(
                CelebrationContainer.ScaleTo(1.0, (uint)(600 * speed), Easing.BounceOut),
                CelebrationContainer.FadeTo(1.0, (uint)(400 * speed))
            );

            // Title slide in
            await CelebrationTitle.TranslateTo(0, 0, (uint)(800 * speed), Easing.SpringOut);

            // Star container reveal
            await StarContainer.ScaleTo(1.0, (uint)(1000 * speed), Easing.BounceOut);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in main celebration animation");
        }
    }

    /// <summary>
    /// Plays confetti animation.
    /// </summary>
    private async Task PlayConfettiAnimationAsync()
    {
        try
        {
            var confettiElements = new[] { Confetti1, Confetti2, Confetti3, Confetti4, Confetti5, Confetti6 };
            var random = new Random();

            var tasks = new List<Task>();

            for (int i = 0; i < confettiElements.Length; i++)
            {
                var confetti = confettiElements[i];
                var delay = i * 200; // Stagger the confetti
                var duration = 2000 + random.Next(-200, 200);
                var fallDistance = 200 + random.Next(-20, 20);
                var rotation = random.Next(360, 720);

                tasks.Add(AnimateConfettiParticle(confetti, delay, duration, fallDistance, rotation));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in confetti animation");
        }
    }

    /// <summary>
    /// Animates a single confetti particle.
    /// </summary>
    private async Task AnimateConfettiParticle(Label confetti, int delay, int duration, double fallDistance, double rotation)
    {
        try
        {
            await Task.Delay(delay);

            // Make visible and start animation
            confetti.Opacity = 1;

            await Task.WhenAll(
                confetti.TranslateTo(confetti.TranslationX, fallDistance, (uint)duration, Easing.CubicIn),
                confetti.RotateTo(rotation, (uint)duration),
                confetti.FadeTo(0, (uint)duration)
            );
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error animating confetti particle");
        }
    }

    /// <summary>
    /// Starts continuous sparkle animation.
    /// </summary>
    private async Task StartContinuousSparkleAsync()
    {
        try
        {
            while (_isVisible)
            {
                await Task.WhenAll(
                    Sparkle1.ScaleTo(1.2, 1000, Easing.SinInOut),
                    Sparkle2.ScaleTo(1.3, 1200, Easing.SinInOut)
                );

                await Task.WhenAll(
                    Sparkle1.ScaleTo(0.8, 1000, Easing.SinInOut),
                    Sparkle2.ScaleTo(0.7, 1200, Easing.SinInOut)
                );

                await Task.Delay(2000);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in continuous sparkle animation");
        }
    }

    /// <summary>
    /// Plays celebration audio based on performance.
    /// </summary>
    private async Task PlayCelebrationAudioAsync(int starsEarned, CelebrationType celebrationType)
    {
        if (_audioService == null) return;

        try
        {
            var audioPath = celebrationType switch
            {
                CelebrationType.CrownChallenge => "audio/achievements/crown_celebration.mp3",
                CelebrationType.Achievement => "audio/achievements/badge_earned.mp3",
                CelebrationType.LevelUp => "audio/achievements/level_complete.mp3",
                _ => starsEarned switch
                {
                    3 => "audio/achievements/perfect_score.mp3",
                    2 => "audio/achievements/great_job.mp3",
                    1 => "audio/achievements/good_work.mp3",
                    _ => "audio/achievements/encouragement.mp3"
                }
            };

            await _audioService.PlayAudioAsync(audioPath, AudioType.Achievement);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error playing celebration audio");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles dismiss button click.
    /// </summary>
    private async void OnDismissClicked(object sender, EventArgs e)
    {
        await HideCelebrationAsync();
    }

    /// <summary>
    /// Handles continue button click.
    /// </summary>
    private async void OnContinueClicked()
    {
        try
        {
            // Play confirmation sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/continue_learning.mp3", AudioType.UIInteraction);
            }

            ContinueRequested?.Invoke(this, new CelebrationEventArgs
            {
                StarsEarned = StarsEarned,
                CelebrationType = CelebrationType.Activity,
                Language = Language
            });

            await HideCelebrationAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling continue click");
        }
    }

    /// <summary>
    /// Handles share button click.
    /// </summary>
    private async void OnShareClicked()
    {
        try
        {
            // Play share sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/share_celebration.mp3", AudioType.UIInteraction);
            }

            ShareRequested?.Invoke(this, new CelebrationEventArgs
            {
                StarsEarned = StarsEarned,
                CelebrationType = CelebrationType.Activity,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling share click");
        }
    }

    #endregion
}

#region Event Args and Enums

/// <summary>
/// Types of celebrations that can be shown.
/// </summary>
public enum CelebrationType
{
    Activity,
    Achievement,
    LevelUp,
    CrownChallenge
}

/// <summary>
/// Event arguments for celebration events.
/// </summary>
public class CelebrationEventArgs : EventArgs
{
    public int StarsEarned { get; set; }
    public CelebrationType CelebrationType { get; set; }
    public string Language { get; set; } = "en";
    public List<string> Achievements { get; set; } = new();
    public double Progress { get; set; }
}

#endregion