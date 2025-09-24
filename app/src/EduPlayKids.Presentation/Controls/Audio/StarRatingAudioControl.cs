using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.Presentation.Controls.Audio;

/// <summary>
/// Audio-enhanced star rating control that provides comprehensive audio feedback
/// for activity completion with age-appropriate celebrations and encouragement.
/// Designed for children aged 3-8 with bilingual support and accessibility features.
/// </summary>
public class StarRatingAudioControl : ContentView
{
    #region Private Fields

    private readonly IAudioService _audioService;
    private readonly ILogger<StarRatingAudioControl> _logger;
    private readonly Grid _starContainer;
    private readonly Label _ratingText;
    private readonly Button _playAudioButton;
    private readonly List<StarView> _stars;

    private int _currentRating;
    private int _childAge = 6;
    private string _language = "en";
    private string _activityType = "general";
    private bool _hasPlayedCelebration;
    private DateTime _ratingSetTime;

    #endregion

    #region Bindable Properties

    /// <summary>
    /// Star rating value (0-3 stars).
    /// </summary>
    public static readonly BindableProperty RatingProperty = BindableProperty.Create(
        nameof(Rating),
        typeof(int),
        typeof(StarRatingAudioControl),
        0,
        propertyChanged: OnRatingChanged);

    /// <summary>
    /// Child's age for age-appropriate audio feedback.
    /// </summary>
    public static readonly BindableProperty ChildAgeProperty = BindableProperty.Create(
        nameof(ChildAge),
        typeof(int),
        typeof(StarRatingAudioControl),
        6,
        propertyChanged: OnChildAgeChanged);

    /// <summary>
    /// Language for bilingual audio support.
    /// </summary>
    public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
        nameof(Language),
        typeof(string),
        typeof(StarRatingAudioControl),
        "en",
        propertyChanged: OnLanguageChanged);

    /// <summary>
    /// Activity type for context-appropriate audio.
    /// </summary>
    public static readonly BindableProperty ActivityTypeProperty = BindableProperty.Create(
        nameof(ActivityType),
        typeof(string),
        typeof(StarRatingAudioControl),
        "general",
        propertyChanged: OnActivityTypeChanged);

    /// <summary>
    /// Whether to auto-play celebration audio when rating is set.
    /// </summary>
    public static readonly BindableProperty AutoPlayCelebrationProperty = BindableProperty.Create(
        nameof(AutoPlayCelebration),
        typeof(bool),
        typeof(StarRatingAudioControl),
        true);

    /// <summary>
    /// Whether to show audio control button.
    /// </summary>
    public static readonly BindableProperty ShowAudioControlProperty = BindableProperty.Create(
        nameof(ShowAudioControl),
        typeof(bool),
        typeof(StarRatingAudioControl),
        true,
        propertyChanged: OnShowAudioControlChanged);

    /// <summary>
    /// Custom celebration message to play.
    /// </summary>
    public static readonly BindableProperty CelebrationMessageProperty = BindableProperty.Create(
        nameof(CelebrationMessage),
        typeof(string),
        typeof(StarRatingAudioControl),
        null);

    #endregion

    #region Public Properties

    public int Rating
    {
        get => (int)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
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

    public string ActivityType
    {
        get => (string)GetValue(ActivityTypeProperty);
        set => SetValue(ActivityTypeProperty, value);
    }

    public bool AutoPlayCelebration
    {
        get => (bool)GetValue(AutoPlayCelebrationProperty);
        set => SetValue(AutoPlayCelebrationProperty, value);
    }

    public bool ShowAudioControl
    {
        get => (bool)GetValue(ShowAudioControlProperty);
        set => SetValue(ShowAudioControlProperty, value);
    }

    public string? CelebrationMessage
    {
        get => (string?)GetValue(CelebrationMessageProperty);
        set => SetValue(CelebrationMessageProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when star rating celebration audio starts.
    /// </summary>
    public event EventHandler<StarRatingAudioEventArgs>? CelebrationAudioStarted;

    /// <summary>
    /// Event fired when star rating celebration audio completes.
    /// </summary>
    public event EventHandler<StarRatingAudioEventArgs>? CelebrationAudioCompleted;

    /// <summary>
    /// Event fired when user requests audio replay.
    /// </summary>
    public event EventHandler<StarRatingAudioEventArgs>? AudioReplayRequested;

    #endregion

    #region Constructor

    public StarRatingAudioControl(IAudioService audioService, ILogger<StarRatingAudioControl> logger)
    {
        _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _stars = new List<StarView>();
        _starContainer = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };

        _ratingText = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.DarkSlateGray
        };

        _playAudioButton = new Button
        {
            Text = "🔊",
            FontSize = 20,
            BackgroundColor = Colors.LightBlue,
            TextColor = Colors.White,
            CornerRadius = 20,
            WidthRequest = 40,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        _playAudioButton.Clicked += OnPlayAudioButtonClicked;

        BuildStarLayout();
        BuildControlLayout();
        UpdateDisplay();
    }

    #endregion

    #region Layout Construction

    private void BuildStarLayout()
    {
        // Create 3 star views
        for (int i = 0; i < 3; i++)
        {
            var star = new StarView
            {
                WidthRequest = 60,
                HeightRequest = 60,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsEnabled = false // Stars are for display only
            };

            _stars.Add(star);
            _starContainer.SetColumn(star, i);
            _starContainer.SetRow(star, 0);
            _starContainer.Children.Add(star);
        }
    }

    private void BuildControlLayout()
    {
        var mainStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Add star container
        mainStack.Children.Add(_starContainer);

        // Add rating text
        _starContainer.SetColumn(_ratingText, 0);
        _starContainer.SetColumnSpan(_ratingText, 3);
        _starContainer.SetRow(_ratingText, 1);
        _starContainer.Children.Add(_ratingText);

        // Add audio button
        _starContainer.SetColumn(_playAudioButton, 1);
        _starContainer.SetRow(_playAudioButton, 2);
        _starContainer.Children.Add(_playAudioButton);

        Content = mainStack;
    }

    #endregion

    #region Star Rating Display

    private void UpdateDisplay()
    {
        try
        {
            // Update star visuals
            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].IsStarFilled = i < _currentRating;
                _stars[i].AnimateStarAppearance(i < _currentRating);
            }

            // Update rating text
            _ratingText.Text = GetRatingText(_currentRating);

            // Update audio button visibility
            _playAudioButton.IsVisible = ShowAudioControl && _currentRating > 0;

            _logger.LogDebug("Star rating display updated: {Rating} stars", _currentRating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating star rating display");
        }
    }

    private string GetRatingText(int rating)
    {
        return rating switch
        {
            3 => Language == "es" ? "¡Excelente! 3 estrellas" : "Excellent! 3 stars",
            2 => Language == "es" ? "¡Muy bien! 2 estrellas" : "Great job! 2 stars",
            1 => Language == "es" ? "¡Bien hecho! 1 estrella" : "Good work! 1 star",
            _ => Language == "es" ? "Sigue intentando" : "Keep trying"
        };
    }

    #endregion

    #region Audio Celebration

    /// <summary>
    /// Plays celebration audio for the current star rating.
    /// </summary>
    public async Task PlayCelebrationAudioAsync()
    {
        try
        {
            if (_currentRating <= 0) return;

            var eventArgs = new StarRatingAudioEventArgs
            {
                Rating = _currentRating,
                ChildAge = _childAge,
                Language = _language,
                ActivityType = _activityType,
                CelebrationMessage = CelebrationMessage
            };

            OnCelebrationAudioStarted(eventArgs);

            // Play rating-specific celebration
            await PlayRatingCelebrationAsync(_currentRating);

            // Play custom message if provided
            if (!string.IsNullOrEmpty(CelebrationMessage))
            {
                await Task.Delay(1000); // Brief pause
                await _audioService.PlayQuestionAudioAsync(CelebrationMessage, "celebration_message");
            }

            _hasPlayedCelebration = true;
            OnCelebrationAudioCompleted(eventArgs);

            _logger.LogInformation("Celebration audio played for {Rating} stars", _currentRating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing celebration audio for {Rating} stars", _currentRating);
        }
    }

    private async Task PlayRatingCelebrationAsync(int rating)
    {
        try
        {
            // Determine celebration intensity based on rating and child age
            var intensity = rating switch
            {
                3 => FeedbackIntensity.High,
                2 => FeedbackIntensity.Medium,
                1 => _childAge <= 4 ? FeedbackIntensity.Medium : FeedbackIntensity.Soft,
                _ => FeedbackIntensity.Soft
            };

            // Play success feedback
            await _audioService.PlaySuccessFeedbackAsync(intensity, _childAge);

            // Brief pause
            await Task.Delay(800);

            // Play activity completion audio
            await _audioService.PlayActivityCompletionAsync(rating, _activityType);

            // Additional celebration for perfect score
            if (rating == 3)
            {
                await Task.Delay(1200);
                await PlayPerfectScoreCelebrationAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing rating celebration for {Rating} stars", rating);
        }
    }

    private async Task PlayPerfectScoreCelebrationAsync()
    {
        try
        {
            var perfectScoreKey = $"celebration.perfect_score.{_activityType}";
            var success = await _audioService.PlayLocalizedAudioAsync(perfectScoreKey, AudioType.Achievement, _language);

            if (!success)
            {
                // Fallback to general perfect score celebration
                var message = _language == "es"
                    ? "¡Puntuación perfecta! ¡Eres increíble!"
                    : "Perfect score! You're amazing!";
                await _audioService.PlayQuestionAudioAsync(message, "perfect_score");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing perfect score celebration");
        }
    }

    /// <summary>
    /// Replays the celebration audio.
    /// </summary>
    public async Task ReplayCelebrationAsync()
    {
        try
        {
            var eventArgs = new StarRatingAudioEventArgs
            {
                Rating = _currentRating,
                ChildAge = _childAge,
                Language = _language,
                ActivityType = _activityType,
                IsReplay = true
            };

            OnAudioReplayRequested(eventArgs);

            await PlayCelebrationAudioAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replaying celebration audio");
        }
    }

    #endregion

    #region Animation Methods

    /// <summary>
    /// Animates the star rating reveal with audio.
    /// </summary>
    public async Task AnimateStarRatingRevealAsync(int targetRating, TimeSpan duration)
    {
        try
        {
            if (targetRating < 0 || targetRating > 3) return;

            var delayBetweenStars = duration.TotalMilliseconds / Math.Max(1, targetRating);

            for (int i = 0; i < targetRating; i++)
            {
                // Animate star appearance
                await _stars[i].AnimateStarRevealAsync();

                // Play star sound
                await _audioService.PlayUIFeedbackAsync(UIInteractionType.ItemSelection);

                // Delay before next star (except for last one)
                if (i < targetRating - 1)
                {
                    await Task.Delay((int)delayBetweenStars);
                }
            }

            // Update rating after animation
            Rating = targetRating;

            // Play final celebration after all stars are revealed
            if (AutoPlayCelebration)
            {
                await Task.Delay(500);
                await PlayCelebrationAudioAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error animating star rating reveal");
        }
    }

    /// <summary>
    /// Animates star sparkle effect with sound.
    /// </summary>
    public async Task AnimateStarSparkleAsync()
    {
        try
        {
            var sparkleDelay = _childAge <= 4 ? 200 : 100; // Slower for younger children

            for (int i = 0; i < _currentRating; i++)
            {
                // Animate sparkle
                await _stars[i].AnimateSparkleAsync();

                // Play sparkle sound
                await _audioService.PlayUIFeedbackAsync(UIInteractionType.ButtonPress);

                await Task.Delay(sparkleDelay);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error animating star sparkle");
        }
    }

    #endregion

    #region Event Handlers

    private async void OnPlayAudioButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            await ReplayCelebrationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling play audio button click");
        }
    }

    #endregion

    #region Property Change Handlers

    private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingAudioControl control && newValue is int rating)
        {
            control.OnRatingChangedInternal(rating);
        }
    }

    private async void OnRatingChangedInternal(int newRating)
    {
        try
        {
            var oldRating = _currentRating;
            _currentRating = Math.Clamp(newRating, 0, 3);
            _ratingSetTime = DateTime.UtcNow;
            _hasPlayedCelebration = false;

            UpdateDisplay();

            // Auto-play celebration if enabled and rating increased
            if (AutoPlayCelebration && _currentRating > 0 && _currentRating > oldRating)
            {
                await Task.Delay(300); // Brief delay for visual setup
                await PlayCelebrationAudioAsync();
            }

            _logger.LogDebug("Star rating changed from {OldRating} to {NewRating}", oldRating, _currentRating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling rating change");
        }
    }

    private static void OnChildAgeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingAudioControl control && newValue is int age)
        {
            control._childAge = age;
        }
    }

    private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingAudioControl control && newValue is string language)
        {
            control._language = language;
            control.UpdateDisplay();
        }
    }

    private static void OnActivityTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingAudioControl control && newValue is string activityType)
        {
            control._activityType = activityType;
        }
    }

    private static void OnShowAudioControlChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingAudioControl control && newValue is bool showControl)
        {
            control._playAudioButton.IsVisible = showControl && control._currentRating > 0;
        }
    }

    #endregion

    #region Event Invocation

    protected virtual void OnCelebrationAudioStarted(StarRatingAudioEventArgs e)
    {
        CelebrationAudioStarted?.Invoke(this, e);
    }

    protected virtual void OnCelebrationAudioCompleted(StarRatingAudioEventArgs e)
    {
        CelebrationAudioCompleted?.Invoke(this, e);
    }

    protected virtual void OnAudioReplayRequested(StarRatingAudioEventArgs e)
    {
        AudioReplayRequested?.Invoke(this, e);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets whether celebration audio has been played for current rating.
    /// </summary>
    public bool HasPlayedCelebration => _hasPlayedCelebration;

    /// <summary>
    /// Gets time when rating was set.
    /// </summary>
    public DateTime RatingSetTime => _ratingSetTime;

    /// <summary>
    /// Resets the control state.
    /// </summary>
    public void Reset()
    {
        Rating = 0;
        _hasPlayedCelebration = false;
        _ratingSetTime = DateTime.MinValue;
        UpdateDisplay();
    }

    #endregion
}

/// <summary>
/// Event arguments for star rating audio events.
/// </summary>
public class StarRatingAudioEventArgs : EventArgs
{
    public int Rating { get; set; }
    public int ChildAge { get; set; }
    public string Language { get; set; } = "en";
    public string ActivityType { get; set; } = "general";
    public string? CelebrationMessage { get; set; }
    public bool IsReplay { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Individual star view with animation support.
/// </summary>
public class StarView : ContentView
{
    private readonly Label _starLabel;
    private bool _isStarFilled;

    public bool IsStarFilled
    {
        get => _isStarFilled;
        set
        {
            _isStarFilled = value;
            UpdateStarAppearance();
        }
    }

    public StarView()
    {
        _starLabel = new Label
        {
            Text = "⭐",
            FontSize = 40,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray,
            Opacity = 0.3
        };

        Content = _starLabel;
    }

    private void UpdateStarAppearance()
    {
        _starLabel.TextColor = _isStarFilled ? Colors.Gold : Colors.Gray;
        _starLabel.Opacity = _isStarFilled ? 1.0 : 0.3;
    }

    public async Task AnimateStarAppearance(bool filled)
    {
        IsStarFilled = filled;

        if (filled)
        {
            // Scale animation
            await _starLabel.ScaleTo(1.3, 200, Easing.BounceOut);
            await _starLabel.ScaleTo(1.0, 200, Easing.BounceOut);
        }
    }

    public async Task AnimateStarRevealAsync()
    {
        _starLabel.Scale = 0;
        _starLabel.Opacity = 0;

        IsStarFilled = true;

        await Task.WhenAll(
            _starLabel.ScaleTo(1.2, 300, Easing.BounceOut),
            _starLabel.FadeTo(1.0, 300, Easing.CubicOut)
        );

        await _starLabel.ScaleTo(1.0, 200, Easing.BounceOut);
    }

    public async Task AnimateSparkleAsync()
    {
        await Task.WhenAll(
            _starLabel.ScaleTo(1.15, 150, Easing.CubicOut),
            _starLabel.RotateTo(15, 150, Easing.CubicOut)
        );

        await Task.WhenAll(
            _starLabel.ScaleTo(1.0, 150, Easing.CubicIn),
            _starLabel.RotateTo(0, 150, Easing.CubicIn)
        );
    }
}