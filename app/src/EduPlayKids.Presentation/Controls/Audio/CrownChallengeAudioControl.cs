using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Infrastructure.Services.Audio;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.Controls.Audio;

/// <summary>
/// Audio-enhanced crown challenge unlock control that provides exciting celebration
/// for achieving crown challenges with age-appropriate encouragement and bilingual support.
/// Designed for children aged 3-8 to celebrate advanced learning achievements.
/// </summary>
public class CrownChallengeAudioControl : ContentView
{
    #region Private Fields

    private readonly IAudioService _audioService;
    private readonly IEducationalAudioService _educationalAudioService;
    private readonly ILogger<CrownChallengeAudioControl> _logger;

    private Grid _mainContainer;
    private StackLayout _crownDisplay;
    private Label _crownIcon;
    private Label _titleLabel;
    private Label _descriptionLabel;
    private Button _playAudioButton;
    private Button _startChallengeButton;
    private ProgressBar _unlockProgress;

    private string _challengeType = "general";
    private int _childAge = 6;
    private string _language = "en";
    private bool _isUnlocked;
    private bool _hasPlayedUnlockCelebration;
    private DateTime _unlockTime;
    private readonly List<string> _celebrationPhrases;

    #endregion

    #region Bindable Properties

    /// <summary>
    /// Type of crown challenge (math, reading, logic, etc.).
    /// </summary>
    public static readonly BindableProperty ChallengeTypeProperty = BindableProperty.Create(
        nameof(ChallengeType),
        typeof(string),
        typeof(CrownChallengeAudioControl),
        "general",
        propertyChanged: OnChallengeTypeChanged);

    /// <summary>
    /// Whether the crown challenge is unlocked.
    /// </summary>
    public static readonly BindableProperty IsUnlockedProperty = BindableProperty.Create(
        nameof(IsUnlocked),
        typeof(bool),
        typeof(CrownChallengeAudioControl),
        false,
        propertyChanged: OnIsUnlockedChanged);

    /// <summary>
    /// Child's age for age-appropriate celebrations.
    /// </summary>
    public static readonly BindableProperty ChildAgeProperty = BindableProperty.Create(
        nameof(ChildAge),
        typeof(int),
        typeof(CrownChallengeAudioControl),
        6,
        propertyChanged: OnChildAgeChanged);

    /// <summary>
    /// Language for bilingual audio support.
    /// </summary>
    public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
        nameof(Language),
        typeof(string),
        typeof(CrownChallengeAudioControl),
        "en",
        propertyChanged: OnLanguageChanged);

    /// <summary>
    /// Title of the crown challenge.
    /// </summary>
    public static readonly BindableProperty ChallengeTitleProperty = BindableProperty.Create(
        nameof(ChallengeTitle),
        typeof(string),
        typeof(CrownChallengeAudioControl),
        null,
        propertyChanged: OnChallengeTitleChanged);

    /// <summary>
    /// Description of the crown challenge.
    /// </summary>
    public static readonly BindableProperty ChallengeDescriptionProperty = BindableProperty.Create(
        nameof(ChallengeDescription),
        typeof(string),
        typeof(CrownChallengeAudioControl),
        null,
        propertyChanged: OnChallengeDescriptionChanged);

    /// <summary>
    /// Whether to auto-play unlock celebration.
    /// </summary>
    public static readonly BindableProperty AutoPlayUnlockCelebrationProperty = BindableProperty.Create(
        nameof(AutoPlayUnlockCelebration),
        typeof(bool),
        typeof(CrownChallengeAudioControl),
        true);

    /// <summary>
    /// Whether to show audio control buttons.
    /// </summary>
    public static readonly BindableProperty ShowAudioControlsProperty = BindableProperty.Create(
        nameof(ShowAudioControls),
        typeof(bool),
        typeof(CrownChallengeAudioControl),
        true,
        propertyChanged: OnShowAudioControlsChanged);

    /// <summary>
    /// Progress towards unlocking (0.0 to 1.0).
    /// </summary>
    public static readonly BindableProperty UnlockProgressProperty = BindableProperty.Create(
        nameof(UnlockProgress),
        typeof(double),
        typeof(CrownChallengeAudioControl),
        0.0,
        propertyChanged: OnUnlockProgressChanged);

    #endregion

    #region Public Properties

    public string ChallengeType
    {
        get => (string)GetValue(ChallengeTypeProperty);
        set => SetValue(ChallengeTypeProperty, value);
    }

    public bool IsUnlocked
    {
        get => (bool)GetValue(IsUnlockedProperty);
        set => SetValue(IsUnlockedProperty, value);
    }

    public int ChildAge
    {
        get => (int)GetValue(ChildAgeProperty);
        set => SetValue(ChildAgeProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public string? ChallengeTitle
    {
        get => (string?)GetValue(ChallengeTitleProperty);
        set => SetValue(ChallengeTitleProperty, value);
    }

    public string? ChallengeDescription
    {
        get => (string?)GetValue(ChallengeDescriptionProperty);
        set => SetValue(ChallengeDescriptionProperty, value);
    }

    public bool AutoPlayUnlockCelebration
    {
        get => (bool)GetValue(AutoPlayUnlockCelebrationProperty);
        set => SetValue(AutoPlayUnlockCelebrationProperty, value);
    }

    public bool ShowAudioControls
    {
        get => (bool)GetValue(ShowAudioControlsProperty);
        set => SetValue(ShowAudioControlsProperty, value);
    }

    public double UnlockProgress
    {
        get => (double)GetValue(UnlockProgressProperty);
        set => SetValue(UnlockProgressProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when crown unlock celebration starts.
    /// </summary>
    public event EventHandler<CrownChallengeAudioEventArgs>? UnlockCelebrationStarted;

    /// <summary>
    /// Event fired when crown unlock celebration completes.
    /// </summary>
    public event EventHandler<CrownChallengeAudioEventArgs>? UnlockCelebrationCompleted;

    /// <summary>
    /// Event fired when user wants to start the challenge.
    /// </summary>
    public event EventHandler<CrownChallengeAudioEventArgs>? ChallengeStartRequested;

    /// <summary>
    /// Event fired when user requests audio replay.
    /// </summary>
    public event EventHandler<CrownChallengeAudioEventArgs>? AudioReplayRequested;

    #endregion

    #region Constructor

    public CrownChallengeAudioControl(
        IAudioService audioService,
        IEducationalAudioService educationalAudioService,
        ILogger<CrownChallengeAudioControl> logger)
    {
        _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        _educationalAudioService = educationalAudioService ?? throw new ArgumentNullException(nameof(educationalAudioService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _celebrationPhrases = new List<string>();

        // Create main container
        _mainContainer = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(GridLength.Auto), // Crown display
                new RowDefinition(GridLength.Auto), // Title
                new RowDefinition(GridLength.Auto), // Description
                new RowDefinition(GridLength.Auto), // Progress
                new RowDefinition(GridLength.Auto)  // Buttons
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star)
            },
            Padding = 20,
            BackgroundColor = Colors.LightGoldenrodYellow,
            Margin = 10
        };

        BuildCrownDisplay();
        BuildTextElements();
        BuildProgressBar();
        BuildButtons();
        BuildLayout();
        InitializeCelebrationPhrases();
        UpdateDisplay();
    }

    #endregion

    #region Layout Construction

    private void BuildCrownDisplay()
    {
        _crownIcon = new Label
        {
            Text = "👑",
            FontSize = 60,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Opacity = 0.3 // Start dimmed until unlocked
        };

        _crownDisplay = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 5
        };

        _crownDisplay.Children.Add(_crownIcon);
    }

    private void BuildTextElements()
    {
        _titleLabel = new Label
        {
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.DarkGoldenrod,
            HorizontalTextAlignment = TextAlignment.Center
        };

        _descriptionLabel = new Label
        {
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.DarkSlateGray,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(10, 5)
        };
    }

    private void BuildProgressBar()
    {
        _unlockProgress = new ProgressBar
        {
            Progress = 0.0,
            ProgressColor = Colors.Gold,
            BackgroundColor = Colors.LightGray,
            HeightRequest = 10,
            Margin = new Thickness(20, 10)
        };
    }

    private void BuildButtons()
    {
        _playAudioButton = new Button
        {
            Text = "🔊 Play Celebration",
            FontSize = 16,
            BackgroundColor = Colors.Gold,
            TextColor = Colors.White,
            CornerRadius = 25,
            HeightRequest = 50,
            Margin = new Thickness(10, 5)
        };

        _startChallengeButton = new Button
        {
            Text = "🚀 Start Challenge",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Colors.DarkGoldenrod,
            TextColor = Colors.White,
            CornerRadius = 25,
            HeightRequest = 55,
            Margin = new Thickness(10, 5)
        };

        _playAudioButton.Clicked += OnPlayAudioButtonClicked;
        _startChallengeButton.Clicked += OnStartChallengeButtonClicked;
    }

    private void BuildLayout()
    {
        // Row 0: Crown display
        _mainContainer.SetRow(_crownDisplay, 0);
        _mainContainer.Children.Add(_crownDisplay);

        // Row 1: Title
        _mainContainer.SetRow(_titleLabel, 1);
        _mainContainer.Children.Add(_titleLabel);

        // Row 2: Description
        _mainContainer.SetRow(_descriptionLabel, 2);
        _mainContainer.Children.Add(_descriptionLabel);

        // Row 3: Progress bar
        _mainContainer.SetRow(_unlockProgress, 3);
        _mainContainer.Children.Add(_unlockProgress);

        // Row 4: Buttons stack
        var buttonStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Fill
        };

        buttonStack.Children.Add(_playAudioButton);
        buttonStack.Children.Add(_startChallengeButton);

        _mainContainer.SetRow(buttonStack, 4);
        _mainContainer.Children.Add(buttonStack);

        Content = _mainContainer;
    }

    #endregion

    #region Display Updates

    private void UpdateDisplay()
    {
        try
        {
            // Update crown appearance
            _crownIcon.Opacity = _isUnlocked ? 1.0 : 0.3;

            // Update colors based on unlock status
            if (_isUnlocked)
            {
                _mainContainer.BackgroundColor = Colors.LightGoldenrodYellow;
                _titleLabel.TextColor = Colors.DarkGoldenrod;
                _crownIcon.Text = "👑✨"; // Add sparkles when unlocked
            }
            else
            {
                _mainContainer.BackgroundColor = Colors.LightGray;
                _titleLabel.TextColor = Colors.Gray;
                _crownIcon.Text = "👑";
            }

            // Update button states
            _playAudioButton.IsVisible = ShowAudioControls && _isUnlocked;
            _startChallengeButton.IsEnabled = _isUnlocked;
            _startChallengeButton.Opacity = _isUnlocked ? 1.0 : 0.5;

            // Update progress bar
            _unlockProgress.Progress = UnlockProgress;
            _unlockProgress.IsVisible = !_isUnlocked; // Hide when unlocked

            // Update text
            UpdateLocalizedText();

            _logger.LogDebug("Crown challenge display updated: {ChallengeType}, Unlocked: {IsUnlocked}", _challengeType, _isUnlocked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating crown challenge display");
        }
    }

    private void UpdateLocalizedText()
    {
        // Update title
        if (string.IsNullOrEmpty(ChallengeTitle))
        {
            _titleLabel.Text = GetLocalizedChallengeTitle();
        }
        else
        {
            _titleLabel.Text = ChallengeTitle;
        }

        // Update description
        if (string.IsNullOrEmpty(ChallengeDescription))
        {
            _descriptionLabel.Text = GetLocalizedChallengeDescription();
        }
        else
        {
            _descriptionLabel.Text = ChallengeDescription;
        }

        // Update button text
        _playAudioButton.Text = _language == "es" ? "🔊 Reproducir Celebración" : "🔊 Play Celebration";
        _startChallengeButton.Text = _language == "es" ? "🚀 Comenzar Desafío" : "🚀 Start Challenge";
    }

    private string GetLocalizedChallengeTitle()
    {
        if (!_isUnlocked)
        {
            return _language == "es" ? "Desafío Corona Bloqueado" : "Crown Challenge Locked";
        }

        return _challengeType.ToLower() switch
        {
            "math" => _language == "es" ? "¡Desafío Corona de Matemáticas!" : "Math Crown Challenge!",
            "reading" => _language == "es" ? "¡Desafío Corona de Lectura!" : "Reading Crown Challenge!",
            "logic" => _language == "es" ? "¡Desafío Corona de Lógica!" : "Logic Crown Challenge!",
            "science" => _language == "es" ? "¡Desafío Corona de Ciencias!" : "Science Crown Challenge!",
            "concepts" => _language == "es" ? "¡Desafío Corona de Conceptos!" : "Concepts Crown Challenge!",
            _ => _language == "es" ? "¡Desafío Corona Especial!" : "Special Crown Challenge!"
        };
    }

    private string GetLocalizedChallengeDescription()
    {
        if (!_isUnlocked)
        {
            var progressPercent = (int)(UnlockProgress * 100);
            return _language == "es"
                ? $"Completa más actividades para desbloquear. Progreso: {progressPercent}%"
                : $"Complete more activities to unlock. Progress: {progressPercent}%";
        }

        return _language == "es"
            ? "¡Felicidades! Has desbloqueado un desafío especial para expertos."
            : "Congratulations! You've unlocked a special expert challenge.";
    }

    #endregion

    #region Audio Celebration

    /// <summary>
    /// Plays the complete crown unlock celebration with exciting audio.
    /// </summary>
    public async Task PlayUnlockCelebrationAsync()
    {
        try
        {
            if (!_isUnlocked) return;

            var eventArgs = new CrownChallengeAudioEventArgs
            {
                ChallengeType = _challengeType,
                ChildAge = _childAge,
                Language = _language,
                IsUnlocked = true,
                UnlockTime = _unlockTime
            };

            OnUnlockCelebrationStarted(eventArgs);

            // Play educational crown unlock celebration
            await _educationalAudioService.PlayCrownUnlockCelebrationAsync(_challengeType, _childAge);

            // Add sparkle animation during audio
            await AnimateCrownSparkleAsync();

            // Play additional celebration phrases
            await PlayCelebrationPhrasesAsync();

            _hasPlayedUnlockCelebration = true;
            OnUnlockCelebrationCompleted(eventArgs);

            _logger.LogInformation("Crown unlock celebration played for {ChallengeType}", _challengeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing crown unlock celebration for {ChallengeType}", _challengeType);
        }
    }

    private async Task PlayCelebrationPhrasesAsync()
    {
        try
        {
            if (!_celebrationPhrases.Any()) return;

            // Brief pause before additional phrases
            await Task.Delay(1500);

            // Select random celebration phrase
            var random = new Random();
            var phrase = _celebrationPhrases[random.Next(_celebrationPhrases.Count)];

            await _audioService.PlayQuestionAudioAsync(phrase, "crown_celebration");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing celebration phrases");
        }
    }

    /// <summary>
    /// Plays achievement motivation audio for progress.
    /// </summary>
    public async Task PlayProgressMotivationAsync()
    {
        try
        {
            var progressPercent = (int)(UnlockProgress * 100);

            if (progressPercent >= 75)
            {
                var message = _language == "es"
                    ? "¡Casi lo tienes! Sigue así para desbloquear el desafío corona."
                    : "You're almost there! Keep going to unlock the crown challenge.";

                await _audioService.PlayQuestionAudioAsync(message, "progress_motivation");
            }
            else if (progressPercent >= 50)
            {
                var message = _language == "es"
                    ? "¡Excelente progreso! Estás a medio camino del desafío corona."
                    : "Excellent progress! You're halfway to the crown challenge.";

                await _audioService.PlayQuestionAudioAsync(message, "progress_motivation");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing progress motivation");
        }
    }

    #endregion

    #region Animation

    /// <summary>
    /// Animates crown unlocking with visual and audio effects.
    /// </summary>
    public async Task AnimateCrownUnlockAsync()
    {
        try
        {
            // Start with locked appearance
            _crownIcon.Opacity = 0.3;
            _crownIcon.Scale = 1.0;

            // Update unlock status
            IsUnlocked = true;
            _unlockTime = DateTime.UtcNow;

            // Play unlock sound
            await _audioService.PlayUIFeedbackAsync(UIInteractionType.ItemSelection);

            // Animate crown appearing
            await Task.WhenAll(
                _crownIcon.FadeTo(1.0, 1000, Easing.CubicOut),
                _crownIcon.ScaleTo(1.3, 800, Easing.BounceOut)
            );

            // Settle to normal size
            await _crownIcon.ScaleTo(1.0, 400, Easing.BounceOut);

            // Play full celebration if auto-play enabled
            if (AutoPlayUnlockCelebration)
            {
                await Task.Delay(500);
                await PlayUnlockCelebrationAsync();
            }

            // Animate sparkle effect
            await AnimateCrownSparkleAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error animating crown unlock");
        }
    }

    private async Task AnimateCrownSparkleAsync()
    {
        try
        {
            var sparkleCount = _childAge <= 4 ? 3 : 5; // Fewer sparkles for younger children

            for (int i = 0; i < sparkleCount; i++)
            {
                // Sparkle animation
                await Task.WhenAll(
                    _crownIcon.ScaleTo(1.1, 150, Easing.CubicOut),
                    _crownIcon.RotateTo(10, 150, Easing.CubicOut)
                );

                await Task.WhenAll(
                    _crownIcon.ScaleTo(1.0, 150, Easing.CubicIn),
                    _crownIcon.RotateTo(-10, 150, Easing.CubicIn)
                );

                await _crownIcon.RotateTo(0, 150, Easing.CubicOut);

                // Brief pause between sparkles
                await Task.Delay(200);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error animating crown sparkle");
        }
    }

    /// <summary>
    /// Animates progress bar filling with audio feedback.
    /// </summary>
    public async Task AnimateProgressUpdateAsync(double newProgress, TimeSpan duration)
    {
        try
        {
            var oldProgress = UnlockProgress;
            var progressDiff = newProgress - oldProgress;

            if (progressDiff <= 0) return;

            // Animate progress bar
            var animation = new Animation(v => _unlockProgress.Progress = v, oldProgress, newProgress);
            animation.Commit(this, "ProgressAnimation", 16, (uint)duration.TotalMilliseconds, Easing.CubicOut);

            // Update property
            UnlockProgress = newProgress;

            // Play progress audio at certain thresholds
            if (newProgress >= 1.0 && oldProgress < 1.0)
            {
                // Crown unlocked!
                await AnimateCrownUnlockAsync();
            }
            else if (newProgress >= 0.75 && oldProgress < 0.75)
            {
                await PlayProgressMotivationAsync();
            }
            else if (newProgress >= 0.5 && oldProgress < 0.5)
            {
                await PlayProgressMotivationAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error animating progress update");
        }
    }

    #endregion

    #region Event Handlers

    private async void OnPlayAudioButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            var eventArgs = new CrownChallengeAudioEventArgs
            {
                ChallengeType = _challengeType,
                ChildAge = _childAge,
                Language = _language,
                IsReplay = true
            };

            OnAudioReplayRequested(eventArgs);
            await PlayUnlockCelebrationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling play audio button click");
        }
    }

    private void OnStartChallengeButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!_isUnlocked) return;

            var eventArgs = new CrownChallengeAudioEventArgs
            {
                ChallengeType = _challengeType,
                ChildAge = _childAge,
                Language = _language,
                IsUnlocked = true
            };

            OnChallengeStartRequested(eventArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling start challenge button click");
        }
    }

    #endregion

    #region Property Change Handlers

    private static void OnChallengeTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control && newValue is string challengeType)
        {
            control._challengeType = challengeType;
            control.UpdateDisplay();
        }
    }

    private static void OnIsUnlockedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control && newValue is bool isUnlocked)
        {
            control._isUnlocked = isUnlocked;
            if (isUnlocked && control._unlockTime == DateTime.MinValue)
            {
                control._unlockTime = DateTime.UtcNow;
            }
            control.UpdateDisplay();
        }
    }

    private static void OnChildAgeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control && newValue is int age)
        {
            control._childAge = age;
        }
    }

    private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control && newValue is string language)
        {
            control._language = language;
            control.InitializeCelebrationPhrases();
            control.UpdateDisplay();
        }
    }

    private static void OnChallengeTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control)
        {
            control.UpdateDisplay();
        }
    }

    private static void OnChallengeDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control)
        {
            control.UpdateDisplay();
        }
    }

    private static void OnShowAudioControlsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control)
        {
            control.UpdateDisplay();
        }
    }

    private static void OnUnlockProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CrownChallengeAudioControl control && newValue is double progress)
        {
            control._unlockProgress.Progress = progress;
        }
    }

    #endregion

    #region Initialization

    private void InitializeCelebrationPhrases()
    {
        _celebrationPhrases.Clear();

        if (_language == "es")
        {
            _celebrationPhrases.AddRange(new[]
            {
                "¡Eres increíble! Has desbloqueado un desafío especial.",
                "¡Fantástico trabajo! Ahora puedes intentar algo más difícil.",
                "¡Qué emocionante! Has ganado acceso a los desafíos de expertos.",
                "¡Bravo! Eres oficialmente un campeón de aprendizaje."
            });
        }
        else
        {
            _celebrationPhrases.AddRange(new[]
            {
                "You're amazing! You've unlocked a special challenge.",
                "Fantastic work! Now you can try something more challenging.",
                "How exciting! You've earned access to expert challenges.",
                "Bravo! You're officially a learning champion."
            });
        }
    }

    #endregion

    #region Event Invocation

    protected virtual void OnUnlockCelebrationStarted(CrownChallengeAudioEventArgs e)
    {
        UnlockCelebrationStarted?.Invoke(this, e);
    }

    protected virtual void OnUnlockCelebrationCompleted(CrownChallengeAudioEventArgs e)
    {
        UnlockCelebrationCompleted?.Invoke(this, e);
    }

    protected virtual void OnChallengeStartRequested(CrownChallengeAudioEventArgs e)
    {
        ChallengeStartRequested?.Invoke(this, e);
    }

    protected virtual void OnAudioReplayRequested(CrownChallengeAudioEventArgs e)
    {
        AudioReplayRequested?.Invoke(this, e);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets whether unlock celebration has been played.
    /// </summary>
    public bool HasPlayedUnlockCelebration => _hasPlayedUnlockCelebration;

    /// <summary>
    /// Gets the time when challenge was unlocked.
    /// </summary>
    public DateTime UnlockTime => _unlockTime;

    /// <summary>
    /// Resets the control state.
    /// </summary>
    public void Reset()
    {
        IsUnlocked = false;
        UnlockProgress = 0.0;
        _hasPlayedUnlockCelebration = false;
        _unlockTime = DateTime.MinValue;
        UpdateDisplay();
    }

    #endregion
}

/// <summary>
/// Event arguments for crown challenge audio events.
/// </summary>
public class CrownChallengeAudioEventArgs : EventArgs
{
    public string ChallengeType { get; set; } = "general";
    public int ChildAge { get; set; }
    public string Language { get; set; } = "en";
    public bool IsUnlocked { get; set; }
    public bool IsReplay { get; set; }
    public DateTime UnlockTime { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}