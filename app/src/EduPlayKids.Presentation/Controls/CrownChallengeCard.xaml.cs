using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace EduPlayKids.App.Controls;

/// <summary>
/// Crown Challenge card for presenting advanced activities to high-performing children.
/// Features royal theming, advanced content previews, and motivational rewards display.
/// Designed to celebrate excellence and encourage continued learning growth.
/// </summary>
public partial class CrownChallengeCard : ContentView
{
    private readonly IAudioService? _audioService;
    private readonly ILogger<CrownChallengeCard>? _logger;
    private bool _isAnimating = false;

    #region Bindable Properties

    /// <summary>
    /// Main title of the crown challenge.
    /// </summary>
    public static readonly BindableProperty ChallengeTitleProperty =
        BindableProperty.Create(
            nameof(ChallengeTitle),
            typeof(string),
            typeof(CrownChallengeCard),
            "Crown Challenge");

    /// <summary>
    /// Subtitle providing additional context.
    /// </summary>
    public static readonly BindableProperty ChallengeSubtitleProperty =
        BindableProperty.Create(
            nameof(ChallengeSubtitle),
            typeof(string),
            typeof(CrownChallengeCard),
            "Advanced Learning");

    /// <summary>
    /// Achievement title explaining what the child accomplished.
    /// </summary>
    public static readonly BindableProperty AchievementTitleProperty =
        BindableProperty.Create(
            nameof(AchievementTitle),
            typeof(string),
            typeof(CrownChallengeCard),
            "🎉 Outstanding Performance!");

    /// <summary>
    /// Achievement description with encouraging details.
    /// </summary>
    public static readonly BindableProperty AchievementDescriptionProperty =
        BindableProperty.Create(
            nameof(AchievementDescription),
            typeof(string),
            typeof(CrownChallengeCard),
            "You've shown excellent learning skills and earned this special challenge!");

    /// <summary>
    /// Title for the challenge preview section.
    /// </summary>
    public static readonly BindableProperty PreviewTitleProperty =
        BindableProperty.Create(
            nameof(PreviewTitle),
            typeof(string),
            typeof(CrownChallengeCard),
            "Challenge Preview");

    /// <summary>
    /// Preview content to show challenge difficulty/type.
    /// </summary>
    public static readonly BindableProperty PreviewContentProperty =
        BindableProperty.Create(
            nameof(PreviewContent),
            typeof(View),
            typeof(CrownChallengeCard),
            null);

    /// <summary>
    /// Hint text for the preview content.
    /// </summary>
    public static readonly BindableProperty PreviewHintProperty =
        BindableProperty.Create(
            nameof(PreviewHint),
            typeof(string),
            typeof(CrownChallengeCard),
            "This challenge is designed just for advanced learners like you!");

    /// <summary>
    /// Title for the reward section.
    /// </summary>
    public static readonly BindableProperty RewardTitleProperty =
        BindableProperty.Create(
            nameof(RewardTitle),
            typeof(string),
            typeof(CrownChallengeCard),
            "🏆 Special Rewards");

    /// <summary>
    /// First reward icon.
    /// </summary>
    public static readonly BindableProperty RewardIcon1Property =
        BindableProperty.Create(
            nameof(RewardIcon1),
            typeof(string),
            typeof(CrownChallengeCard),
            "👑");

    /// <summary>
    /// Second reward icon.
    /// </summary>
    public static readonly BindableProperty RewardIcon2Property =
        BindableProperty.Create(
            nameof(RewardIcon2),
            typeof(string),
            typeof(CrownChallengeCard),
            "🌟");

    /// <summary>
    /// Third reward icon.
    /// </summary>
    public static readonly BindableProperty RewardIcon3Property =
        BindableProperty.Create(
            nameof(RewardIcon3),
            typeof(string),
            typeof(CrownChallengeCard),
            "🎖️");

    /// <summary>
    /// Description of rewards earned.
    /// </summary>
    public static readonly BindableProperty RewardDescriptionProperty =
        BindableProperty.Create(
            nameof(RewardDescription),
            typeof(string),
            typeof(CrownChallengeCard),
            "Earn exclusive badges and unlock new content!");

    /// <summary>
    /// Text for the difficulty badge.
    /// </summary>
    public static readonly BindableProperty DifficultyTextProperty =
        BindableProperty.Create(
            nameof(DifficultyText),
            typeof(string),
            typeof(CrownChallengeCard),
            "EXPERT");

    /// <summary>
    /// Text explaining required stars.
    /// </summary>
    public static readonly BindableProperty RequiredStarsTextProperty =
        BindableProperty.Create(
            nameof(RequiredStarsText),
            typeof(string),
            typeof(CrownChallengeCard),
            "Perfect Score!");

    /// <summary>
    /// Text for the "Later" button.
    /// </summary>
    public static readonly BindableProperty LaterButtonTextProperty =
        BindableProperty.Create(
            nameof(LaterButtonText),
            typeof(string),
            typeof(CrownChallengeCard),
            "Later");

    /// <summary>
    /// Text for the "Start Challenge" button.
    /// </summary>
    public static readonly BindableProperty StartButtonTextProperty =
        BindableProperty.Create(
            nameof(StartButtonText),
            typeof(string),
            typeof(CrownChallengeCard),
            "👑 Start Crown Challenge");

    /// <summary>
    /// Loading text displayed during operations.
    /// </summary>
    public static readonly BindableProperty LoadingTextProperty =
        BindableProperty.Create(
            nameof(LoadingText),
            typeof(string),
            typeof(CrownChallengeCard),
            "Preparing your challenge...");

    /// <summary>
    /// Whether to show the difficulty badge.
    /// </summary>
    public static readonly BindableProperty ShowDifficultyBadgeProperty =
        BindableProperty.Create(
            nameof(ShowDifficultyBadge),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    /// <summary>
    /// Whether to show required stars section.
    /// </summary>
    public static readonly BindableProperty ShowRequiredStarsProperty =
        BindableProperty.Create(
            nameof(ShowRequiredStars),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    /// <summary>
    /// Whether to show help button.
    /// </summary>
    public static readonly BindableProperty ShowHelpButtonProperty =
        BindableProperty.Create(
            nameof(ShowHelpButton),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    /// <summary>
    /// Whether to show reward icons.
    /// </summary>
    public static readonly BindableProperty ShowRewardIcon1Property =
        BindableProperty.Create(
            nameof(ShowRewardIcon1),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    public static readonly BindableProperty ShowRewardIcon2Property =
        BindableProperty.Create(
            nameof(ShowRewardIcon2),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    public static readonly BindableProperty ShowRewardIcon3Property =
        BindableProperty.Create(
            nameof(ShowRewardIcon3),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    /// <summary>
    /// Whether the challenge can be started.
    /// </summary>
    public static readonly BindableProperty CanStartChallengeProperty =
        BindableProperty.Create(
            nameof(CanStartChallenge),
            typeof(bool),
            typeof(CrownChallengeCard),
            true);

    /// <summary>
    /// Whether the card is in loading state.
    /// </summary>
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(
            nameof(IsLoading),
            typeof(bool),
            typeof(CrownChallengeCard),
            false);

    /// <summary>
    /// Language for localization.
    /// </summary>
    public static readonly BindableProperty LanguageProperty =
        BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(CrownChallengeCard),
            "en");

    // Command Properties
    public static readonly BindableProperty StartChallengeCommandProperty =
        BindableProperty.Create(
            nameof(StartChallengeCommand),
            typeof(ICommand),
            typeof(CrownChallengeCard),
            null);

    public static readonly BindableProperty LaterCommandProperty =
        BindableProperty.Create(
            nameof(LaterCommand),
            typeof(ICommand),
            typeof(CrownChallengeCard),
            null);

    public static readonly BindableProperty HelpCommandProperty =
        BindableProperty.Create(
            nameof(HelpCommand),
            typeof(ICommand),
            typeof(CrownChallengeCard),
            null);

    #region Property Accessors

    public string ChallengeTitle
    {
        get => (string)GetValue(ChallengeTitleProperty);
        set => SetValue(ChallengeTitleProperty, value);
    }

    public string ChallengeSubtitle
    {
        get => (string)GetValue(ChallengeSubtitleProperty);
        set => SetValue(ChallengeSubtitleProperty, value);
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

    public string PreviewTitle
    {
        get => (string)GetValue(PreviewTitleProperty);
        set => SetValue(PreviewTitleProperty, value);
    }

    public View PreviewContent
    {
        get => (View)GetValue(PreviewContentProperty);
        set => SetValue(PreviewContentProperty, value);
    }

    public string PreviewHint
    {
        get => (string)GetValue(PreviewHintProperty);
        set => SetValue(PreviewHintProperty, value);
    }

    public string RewardTitle
    {
        get => (string)GetValue(RewardTitleProperty);
        set => SetValue(RewardTitleProperty, value);
    }

    public string RewardIcon1
    {
        get => (string)GetValue(RewardIcon1Property);
        set => SetValue(RewardIcon1Property, value);
    }

    public string RewardIcon2
    {
        get => (string)GetValue(RewardIcon2Property);
        set => SetValue(RewardIcon2Property, value);
    }

    public string RewardIcon3
    {
        get => (string)GetValue(RewardIcon3Property);
        set => SetValue(RewardIcon3Property, value);
    }

    public string RewardDescription
    {
        get => (string)GetValue(RewardDescriptionProperty);
        set => SetValue(RewardDescriptionProperty, value);
    }

    public string DifficultyText
    {
        get => (string)GetValue(DifficultyTextProperty);
        set => SetValue(DifficultyTextProperty, value);
    }

    public string RequiredStarsText
    {
        get => (string)GetValue(RequiredStarsTextProperty);
        set => SetValue(RequiredStarsTextProperty, value);
    }

    public string LaterButtonText
    {
        get => (string)GetValue(LaterButtonTextProperty);
        set => SetValue(LaterButtonTextProperty, value);
    }

    public string StartButtonText
    {
        get => (string)GetValue(StartButtonTextProperty);
        set => SetValue(StartButtonTextProperty, value);
    }

    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public bool ShowDifficultyBadge
    {
        get => (bool)GetValue(ShowDifficultyBadgeProperty);
        set => SetValue(ShowDifficultyBadgeProperty, value);
    }

    public bool ShowRequiredStars
    {
        get => (bool)GetValue(ShowRequiredStarsProperty);
        set => SetValue(ShowRequiredStarsProperty, value);
    }

    public bool ShowHelpButton
    {
        get => (bool)GetValue(ShowHelpButtonProperty);
        set => SetValue(ShowHelpButtonProperty, value);
    }

    public bool ShowRewardIcon1
    {
        get => (bool)GetValue(ShowRewardIcon1Property);
        set => SetValue(ShowRewardIcon1Property, value);
    }

    public bool ShowRewardIcon2
    {
        get => (bool)GetValue(ShowRewardIcon2Property);
        set => SetValue(ShowRewardIcon2Property, value);
    }

    public bool ShowRewardIcon3
    {
        get => (bool)GetValue(ShowRewardIcon3Property);
        set => SetValue(ShowRewardIcon3Property, value);
    }

    public bool CanStartChallenge
    {
        get => (bool)GetValue(CanStartChallengeProperty);
        set => SetValue(CanStartChallengeProperty, value);
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public ICommand StartChallengeCommand
    {
        get => (ICommand)GetValue(StartChallengeCommandProperty);
        set => SetValue(StartChallengeCommandProperty, value);
    }

    public ICommand LaterCommand
    {
        get => (ICommand)GetValue(LaterCommandProperty);
        set => SetValue(LaterCommandProperty, value);
    }

    public ICommand HelpCommand
    {
        get => (ICommand)GetValue(HelpCommandProperty);
        set => SetValue(HelpCommandProperty, value);
    }

    #endregion

    #endregion

    #region Events

    /// <summary>
    /// Fired when the crown challenge is started.
    /// </summary>
    public event EventHandler<CrownChallengeEventArgs>? ChallengeStarted;

    /// <summary>
    /// Fired when the crown challenge is postponed.
    /// </summary>
    public event EventHandler<CrownChallengeEventArgs>? ChallengePostponed;

    /// <summary>
    /// Fired when help is requested.
    /// </summary>
    public event EventHandler<CrownChallengeEventArgs>? HelpRequested;

    /// <summary>
    /// Fired when the appearance animation completes.
    /// </summary>
    public event EventHandler<EventArgs>? AppearanceAnimationCompleted;

    #endregion

    #region Constructor

    public CrownChallengeCard()
    {
        InitializeComponent();

        // Try to get services from dependency injection
        try
        {
            _audioService = ServiceHelper.GetService<IAudioService>();
            _logger = ServiceHelper.GetService<ILogger<CrownChallengeCard>>();
        }
        catch
        {
            // Services not available, will work without audio feedback
        }

        InitializeCard();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the crown challenge card with default settings.
    /// </summary>
    private void InitializeCard()
    {
        // Set up default commands if none provided
        if (StartChallengeCommand == null)
        {
            StartChallengeCommand = new Command(OnStartChallengeClicked);
        }

        if (LaterCommand == null)
        {
            LaterCommand = new Command(OnLaterClicked);
        }

        if (HelpCommand == null)
        {
            HelpCommand = new Command(OnHelpClicked);
        }

        _logger?.LogDebug("CrownChallengeCard initialized");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Plays the crown challenge appearance animation.
    /// </summary>
    public async Task PlayAppearanceAnimationAsync()
    {
        if (_isAnimating) return;

        try
        {
            _isAnimating = true;
            _logger?.LogInformation("Playing crown challenge appearance animation");

            // Play royal fanfare audio
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/achievements/crown_unlock.mp3", AudioType.Achievement);
            }

            // Reset initial state
            CrownContainer.Scale = 0;
            CrownContainer.Opacity = 0;
            CrownIcon.Rotation = -180;

            // Make visible
            MainContainer.IsVisible = true;

            // Animate appearance
            await Task.WhenAll(
                CrownContainer.ScaleTo(1.0, 800, Easing.BounceOut),
                CrownContainer.FadeTo(1.0, 600),
                CrownIcon.RotateTo(0, 1000, Easing.SpringOut)
            );

            // Start shimmer effect
            _ = StartShimmerAnimationAsync();

            // Start crown pulse
            _ = StartPulseAnimationAsync();

            AppearanceAnimationCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in crown challenge appearance animation");
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <summary>
    /// Sets up the challenge for a specific educational context.
    /// </summary>
    /// <param name="subjectName">Subject name for theming</param>
    /// <param name="activityType">Type of advanced activity</param>
    /// <param name="difficultyLevel">Difficulty level display</param>
    /// <param name="language">Language for localization</param>
    public void ConfigureChallenge(string subjectName, string activityType, string difficultyLevel, string language = "en")
    {
        Language = language;

        if (language == "es")
        {
            ConfigureSpanishText(subjectName, activityType, difficultyLevel);
        }
        else
        {
            ConfigureEnglishText(subjectName, activityType, difficultyLevel);
        }

        DifficultyText = difficultyLevel.ToUpper();
        _logger?.LogInformation("Crown challenge configured for {Subject} {Activity} at {Difficulty} level",
            subjectName, activityType, difficultyLevel);
    }

    /// <summary>
    /// Updates the preview content with educational material.
    /// </summary>
    /// <param name="previewView">View to display as preview</param>
    /// <param name="hint">Hint text for the preview</param>
    public void SetPreviewContent(View previewView, string hint = "")
    {
        PreviewContent = previewView;
        if (!string.IsNullOrEmpty(hint))
        {
            PreviewHint = hint;
        }
    }

    /// <summary>
    /// Updates reward display for specific achievements.
    /// </summary>
    /// <param name="icon1">First reward icon</param>
    /// <param name="icon2">Second reward icon</param>
    /// <param name="icon3">Third reward icon</param>
    /// <param name="description">Reward description</param>
    public void SetRewards(string icon1, string icon2, string icon3, string description)
    {
        RewardIcon1 = icon1;
        RewardIcon2 = icon2;
        RewardIcon3 = icon3;
        RewardDescription = description;
    }

    /// <summary>
    /// Hides the crown challenge with animation.
    /// </summary>
    public async Task HideAsync()
    {
        try
        {
            await Task.WhenAll(
                CrownContainer.ScaleTo(0, 400, Easing.CubicIn),
                CrownContainer.FadeTo(0, 300)
            );

            MainContainer.IsVisible = false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error hiding crown challenge");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Configures English text content.
    /// </summary>
    private void ConfigureEnglishText(string subjectName, string activityType, string difficultyLevel)
    {
        ChallengeTitle = $"{subjectName} Crown Challenge";
        ChallengeSubtitle = $"Advanced {activityType}";
        AchievementTitle = "🎉 Outstanding Performance!";
        AchievementDescription = $"You've mastered {subjectName} basics and earned this special {activityType} challenge!";
        PreviewTitle = "Challenge Preview";
        PreviewHint = "This advanced activity is designed just for excellent learners like you!";
        RewardTitle = "🏆 Special Rewards";
        RewardDescription = "Earn exclusive crown badges and unlock expert content!";
        RequiredStarsText = "Perfect Score!";
        LaterButtonText = "Later";
        StartButtonText = "👑 Start Challenge";
        LoadingText = "Preparing your royal challenge...";
    }

    /// <summary>
    /// Configures Spanish text content.
    /// </summary>
    private void ConfigureSpanishText(string subjectName, string activityType, string difficultyLevel)
    {
        ChallengeTitle = $"Desafío Corona {subjectName}";
        ChallengeSubtitle = $"{activityType} Avanzado";
        AchievementTitle = "🎉 ¡Rendimiento Excepcional!";
        AchievementDescription = $"¡Has dominado lo básico de {subjectName} y ganaste este desafío especial de {activityType}!";
        PreviewTitle = "Vista Previa del Desafío";
        PreviewHint = "¡Esta actividad avanzada está diseñada especialmente para estudiantes excelentes como tú!";
        RewardTitle = "🏆 Premios Especiales";
        RewardDescription = "¡Gana insignias de corona exclusivas y desbloquea contenido experto!";
        RequiredStarsText = "¡Puntaje Perfecto!";
        LaterButtonText = "Después";
        StartButtonText = "👑 Empezar Desafío";
        LoadingText = "Preparando tu desafío real...";
    }

    /// <summary>
    /// Starts the shimmer animation effect.
    /// </summary>
    private async Task StartShimmerAnimationAsync()
    {
        try
        {
            while (MainContainer.IsVisible && !_isAnimating)
            {
                await ShimmerOverlay.FadeTo(0.7, 1000, Easing.SinInOut);
                await ShimmerOverlay.FadeTo(0.2, 1000, Easing.SinInOut);
                await Task.Delay(2000); // Pause between shimmer cycles
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in shimmer animation");
        }
    }

    /// <summary>
    /// Starts the crown icon pulse animation.
    /// </summary>
    private async Task StartPulseAnimationAsync()
    {
        try
        {
            while (MainContainer.IsVisible && !_isAnimating)
            {
                await CrownIcon.ScaleTo(1.1, 500, Easing.SinInOut);
                await CrownIcon.ScaleTo(1.0, 500, Easing.SinInOut);
                await Task.Delay(3000); // Pause between pulses
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in pulse animation");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles start challenge button click.
    /// </summary>
    private async void OnStartChallengeClicked()
    {
        try
        {
            _logger?.LogInformation("Crown challenge start requested");

            // Play royal confirmation sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/royal_confirm.mp3", AudioType.UIInteraction);
            }

            // Provide haptic feedback
            try
            {
#if ANDROID || IOS
#if false
                HapticFeedback.Perform(HapticFeedbackType.Click);
#endif
#endif
            }
            catch
            {
                // Haptic feedback not available
            }

            ChallengeStarted?.Invoke(this, new CrownChallengeEventArgs
            {
                Action = CrownChallengeAction.Started,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling start challenge click");
        }
    }

    /// <summary>
    /// Handles later button click.
    /// </summary>
    private async void OnLaterClicked()
    {
        try
        {
            _logger?.LogInformation("Crown challenge postponed");

            // Play neutral sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/gentle_confirm.mp3", AudioType.UIInteraction);
            }

            ChallengePostponed?.Invoke(this, new CrownChallengeEventArgs
            {
                Action = CrownChallengeAction.Postponed,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling later click");
        }
    }

    /// <summary>
    /// Handles help button click.
    /// </summary>
    private async void OnHelpClicked()
    {
        try
        {
            _logger?.LogInformation("Crown challenge help requested");

            // Play helpful sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/help_sound.mp3", AudioType.UIInteraction);
            }

            HelpRequested?.Invoke(this, new CrownChallengeEventArgs
            {
                Action = CrownChallengeAction.HelpRequested,
                Language = Language
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling help click");
        }
    }

    #endregion
}

#region Event Args and Enums

/// <summary>
/// Actions that can be taken on a crown challenge.
/// </summary>
public enum CrownChallengeAction
{
    Started,
    Postponed,
    HelpRequested
}

/// <summary>
/// Event arguments for crown challenge events.
/// </summary>
public class CrownChallengeEventArgs : EventArgs
{
    public CrownChallengeAction Action { get; set; }
    public string Language { get; set; } = "en";
    public string ChallengeId { get; set; } = string.Empty;
    public int ChildId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
}

#endregion