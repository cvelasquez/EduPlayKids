using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Services;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.Controls;

/// <summary>
/// Interactive star rating control designed for children ages 3-8.
/// Features large touch targets, celebration animations, and positive reinforcement.
/// Supports accessibility and bilingual feedback.
/// </summary>
public partial class StarRatingControl : ContentView
{
    private readonly IAudioService? _audioService;
    private readonly ILogger<StarRatingControl>? _logger;
    private int _currentStarRating = 0;
    private bool _isAnimating = false;

    #region Bindable Properties

    /// <summary>
    /// The number of stars to display (1-3).
    /// </summary>
    public static readonly BindableProperty StarsEarnedProperty =
        BindableProperty.Create(
            nameof(StarsEarned),
            typeof(int),
            typeof(StarRatingControl),
            0,
            propertyChanged: OnStarsEarnedChanged);

    /// <summary>
    /// Title text displayed above the stars.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(StarRatingControl),
            string.Empty);

    /// <summary>
    /// Encouraging message displayed below the stars.
    /// </summary>
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(
            nameof(Message),
            typeof(string),
            typeof(StarRatingControl),
            string.Empty);

    /// <summary>
    /// Whether to show the title label.
    /// </summary>
    public static readonly BindableProperty ShowTitleProperty =
        BindableProperty.Create(
            nameof(ShowTitle),
            typeof(bool),
            typeof(StarRatingControl),
            true);

    /// <summary>
    /// Whether to show the message frame.
    /// </summary>
    public static readonly BindableProperty ShowMessageProperty =
        BindableProperty.Create(
            nameof(ShowMessage),
            typeof(bool),
            typeof(StarRatingControl),
            true);

    /// <summary>
    /// Whether to allow interactive star tapping.
    /// </summary>
    public static readonly BindableProperty IsInteractiveProperty =
        BindableProperty.Create(
            nameof(IsInteractive),
            typeof(bool),
            typeof(StarRatingControl),
            false);

    /// <summary>
    /// Whether to show glow effects during celebrations.
    /// </summary>
    public static readonly BindableProperty ShowGlowEffectProperty =
        BindableProperty.Create(
            nameof(ShowGlowEffect),
            typeof(bool),
            typeof(StarRatingControl),
            false);

    /// <summary>
    /// Animation speed multiplier for celebrations.
    /// </summary>
    public static readonly BindableProperty AnimationSpeedProperty =
        BindableProperty.Create(
            nameof(AnimationSpeed),
            typeof(double),
            typeof(StarRatingControl),
            1.0);

    /// <summary>
    /// Language for audio feedback (en/es).
    /// </summary>
    public static readonly BindableProperty LanguageProperty =
        BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(StarRatingControl),
            "en");

    public int StarsEarned
    {
        get => (int)GetValue(StarsEarnedProperty);
        set => SetValue(StarsEarnedProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    public bool ShowMessage
    {
        get => (bool)GetValue(ShowMessageProperty);
        set => SetValue(ShowMessageProperty, value);
    }

    public bool IsInteractive
    {
        get => (bool)GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public bool ShowGlowEffect
    {
        get => (bool)GetValue(ShowGlowEffectProperty);
        set => SetValue(ShowGlowEffectProperty, value);
    }

    public double AnimationSpeed
    {
        get => (double)GetValue(AnimationSpeedProperty);
        set => SetValue(AnimationSpeedProperty, value);
    }

    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Fired when a star is tapped (interactive mode only).
    /// </summary>
    public event EventHandler<StarTappedEventArgs>? StarTapped;

    /// <summary>
    /// Fired when star animation completes.
    /// </summary>
    public event EventHandler<StarAnimationCompletedEventArgs>? AnimationCompleted;

    #endregion

    #region Constructor

    public StarRatingControl()
    {
        InitializeComponent();

        // Try to get services from dependency injection if available
        try
        {
            _audioService = ServiceHelper.GetService<IAudioService>();
            _logger = ServiceHelper.GetService<ILogger<StarRatingControl>>();
        }
        catch
        {
            // Services not available, will work without audio feedback
        }

        InitializeStarDisplay();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the star display with default state.
    /// </summary>
    private void InitializeStarDisplay()
    {
        // Set initial star colors to silver/gray
        UpdateStarColors(0);

        // Configure interactive mode
        UpdateInteractiveMode();

        _logger?.LogDebug("StarRatingControl initialized with 0 stars");
    }

    /// <summary>
    /// Updates interactive mode settings.
    /// </summary>
    private void UpdateInteractiveMode()
    {
        Star1Button.IsVisible = IsInteractive;
        Star2Button.IsVisible = IsInteractive;
        Star3Button.IsVisible = IsInteractive;
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to the StarsEarned property.
    /// </summary>
    private static void OnStarsEarnedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StarRatingControl control && newValue is int newStars)
        {
            control.UpdateStarsWithAnimation(newStars);
        }
    }

    #endregion

    #region Star Display Management

    /// <summary>
    /// Updates star display with celebration animations.
    /// </summary>
    /// <param name="starCount">Number of stars to display (0-3)</param>
    private async void UpdateStarsWithAnimation(int starCount)
    {
        if (_isAnimating) return;

        var clampedStars = Math.Max(0, Math.Min(3, starCount));
        var previousStars = _currentStarRating;
        _currentStarRating = clampedStars;

        _logger?.LogInformation("Updating stars from {Previous} to {Current}", previousStars, clampedStars);

        try
        {
            _isAnimating = true;

            // Play audio feedback for star earning
            if (clampedStars > previousStars && _audioService != null)
            {
                await PlayStarEarnedAudioAsync(clampedStars);
            }

            // Animate new stars being earned
            if (clampedStars > previousStars)
            {
                await AnimateStarsEarnedAsync(previousStars, clampedStars);
            }
            else
            {
                // Just update colors without celebration animation
                UpdateStarColors(clampedStars);
            }

            // Update message based on star count
            UpdateEncouragingMessage(clampedStars);

            AnimationCompleted?.Invoke(this, new StarAnimationCompletedEventArgs
            {
                PreviousStars = previousStars,
                CurrentStars = clampedStars,
                IsEarningStars = clampedStars > previousStars
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating star display");
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <summary>
    /// Updates star colors based on earned count.
    /// </summary>
    /// <param name="earnedStars">Number of earned stars</param>
    private void UpdateStarColors(int earnedStars)
    {
        // Gold stars for earned, silver for not earned
        Star1.TextColor = earnedStars >= 1 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");
        Star2.TextColor = earnedStars >= 2 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");
        Star3.TextColor = earnedStars >= 3 ? Color.FromArgb("#FFD700") : Color.FromArgb("#C0C0C0");

        // Update emojis for visual appeal
        Star1.Text = earnedStars >= 1 ? "🌟" : "⭐";
        Star2.Text = earnedStars >= 2 ? "🌟" : "⭐";
        Star3.Text = earnedStars >= 3 ? "🌟" : "⭐";
    }

    /// <summary>
    /// Updates the encouraging message based on star count.
    /// </summary>
    /// <param name="starCount">Number of stars earned</param>
    private void UpdateEncouragingMessage(int starCount)
    {
        if (!ShowMessage) return;

        Message = starCount switch
        {
            3 => Language == "es" ? "¡Perfecto! ¡Eres increíble!" : "Perfect! You're amazing!",
            2 => Language == "es" ? "¡Muy bien! ¡Sigue así!" : "Great job! Keep it up!",
            1 => Language == "es" ? "¡Buen trabajo! ¡Puedes hacerlo mejor!" : "Good work! You can do even better!",
            _ => Language == "es" ? "¡Sigue intentando! ¡Tú puedes!" : "Keep trying! You can do it!"
        };
    }

    #endregion

    #region Animations

    /// <summary>
    /// Animates stars being earned with celebration effects.
    /// </summary>
    /// <param name="fromStars">Starting star count</param>
    /// <param name="toStars">Ending star count</param>
    private async Task AnimateStarsEarnedAsync(int fromStars, int toStars)
    {
        try
        {
            // Show glow effect if enabled
            if (ShowGlowEffect)
            {
                await StarGlow.FadeTo(0.3, 200);
            }

            // Animate each new star individually
            for (int i = fromStars + 1; i <= toStars; i++)
            {
                await AnimateSingleStarAsync(i);
                await Task.Delay(300); // Delay between stars for dramatic effect
            }

            // Show confetti celebration for perfect scores
            if (toStars == 3)
            {
                await ShowConfettiCelebrationAsync();
            }

            // Fade out glow effect
            if (ShowGlowEffect)
            {
                await StarGlow.FadeTo(0, 300);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in star earning animation");
        }
    }

    /// <summary>
    /// Animates a single star being earned.
    /// </summary>
    /// <param name="starNumber">Star number (1-3)</param>
    private async Task AnimateSingleStarAsync(int starNumber)
    {
        var starLabel = starNumber switch
        {
            1 => Star1,
            2 => Star2,
            3 => Star3,
            _ => null
        };

        var starGrid = starNumber switch
        {
            1 => Star1Grid,
            2 => Star2Grid,
            3 => Star3Grid,
            _ => null
        };

        if (starLabel == null || starGrid == null) return;

        try
        {
            // Update star appearance
            starLabel.Text = "🌟";
            starLabel.TextColor = Color.FromArgb("#FFD700");

            // Bounce animation
            await starGrid.ScaleTo(1.3, 200, Easing.BounceOut);
            await starGrid.RotateTo(360, 400, Easing.SpringOut);
            await starGrid.ScaleTo(1.0, 200, Easing.BounceOut);
            starGrid.Rotation = 0; // Reset rotation
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error animating star {StarNumber}", starNumber);
        }
    }

    /// <summary>
    /// Shows confetti celebration for perfect scores.
    /// </summary>
    private async Task ShowConfettiCelebrationAsync()
    {
        try
        {
            CelebrationOverlay.IsVisible = true;

            // Animate confetti particles
            var tasks = new List<Task>
            {
                AnimateConfettiParticle(Confetti1, -50, -100),
                AnimateConfettiParticle(Confetti2, 50, -80),
                AnimateConfettiParticle(Confetti3, -30, -120),
                AnimateConfettiParticle(Confetti4, 30, -90),
                AnimateConfettiParticle(Confetti5, 0, -110)
            };

            await Task.WhenAll(tasks);

            // Hide confetti after animation
            await Task.Delay(1000);
            CelebrationOverlay.IsVisible = false;

            // Reset confetti positions
            ResetConfettiPositions();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in confetti celebration");
        }
    }

    /// <summary>
    /// Animates a single confetti particle.
    /// </summary>
    /// <param name="confetti">Confetti label to animate</param>
    /// <param name="deltaX">Horizontal movement</param>
    /// <param name="deltaY">Vertical movement</param>
    private async Task AnimateConfettiParticle(Label confetti, double deltaX, double deltaY)
    {
        try
        {
            // Start from center
            confetti.TranslationX = 0;
            confetti.TranslationY = 0;
            confetti.Opacity = 0;
            confetti.Scale = 0.5;

            // Fade in and scale up
            await Task.WhenAll(
                confetti.FadeTo(1, 200),
                confetti.ScaleTo(1, 200)
            );

            // Move and rotate
            await Task.WhenAll(
                confetti.TranslateTo(deltaX, deltaY, 800, Easing.CubicOut),
                confetti.RotateTo(360, 800)
            );

            // Fade out while falling
            await Task.WhenAll(
                confetti.FadeTo(0, 500),
                confetti.TranslateTo(deltaX, deltaY + 100, 500, Easing.CubicIn)
            );
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error animating confetti particle");
        }
    }

    /// <summary>
    /// Resets confetti particles to initial positions.
    /// </summary>
    private void ResetConfettiPositions()
    {
        var confettiElements = new[] { Confetti1, Confetti2, Confetti3, Confetti4, Confetti5 };

        foreach (var confetti in confettiElements)
        {
            confetti.TranslationX = 0;
            confetti.TranslationY = 0;
            confetti.Opacity = 0;
            confetti.Scale = 0.5;
            confetti.Rotation = 0;
        }
    }

    #endregion

    #region Audio Feedback

    /// <summary>
    /// Plays audio feedback for star earning.
    /// </summary>
    /// <param name="starCount">Number of stars earned</param>
    private async Task PlayStarEarnedAudioAsync(int starCount)
    {
        if (_audioService == null) return;

        try
        {
            var audioPath = starCount switch
            {
                3 => "audio/achievements/three_stars.mp3",
                2 => "audio/achievements/two_stars.mp3",
                1 => "audio/achievements/one_star.mp3",
                _ => "audio/achievements/star_earned.mp3"
            };

            await _audioService.PlayAudioAsync(audioPath, AudioType.Achievement);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error playing star earned audio");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles star tap events in interactive mode.
    /// </summary>
    private async void OnStarTapped(object sender, EventArgs e)
    {
        if (!IsInteractive || _isAnimating) return;

        if (sender is Button button && int.TryParse(button.CommandParameter?.ToString(), out int starNumber))
        {
            _logger?.LogInformation("Star {StarNumber} tapped", starNumber);

            // Provide haptic feedback for touch confirmation
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

            // Play tap sound
            if (_audioService != null)
            {
                await _audioService.PlayAudioAsync("audio/ui/star_tap.mp3", AudioType.UIInteraction);
            }

            // Update star rating
            StarsEarned = starNumber;

            // Fire event
            StarTapped?.Invoke(this, new StarTappedEventArgs
            {
                StarNumber = starNumber,
                TotalStars = starNumber
            });
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Resets the star display to zero stars.
    /// </summary>
    public void Reset()
    {
        _currentStarRating = 0;
        UpdateStarColors(0);
        Message = string.Empty;
        ShowGlowEffect = false;
    }

    /// <summary>
    /// Sets star count without animation.
    /// </summary>
    /// <param name="starCount">Number of stars to display</param>
    public void SetStarsInstantly(int starCount)
    {
        var clampedStars = Math.Max(0, Math.Min(3, starCount));
        _currentStarRating = clampedStars;
        UpdateStarColors(clampedStars);
        UpdateEncouragingMessage(clampedStars);
    }

    /// <summary>
    /// Triggers celebration animation manually.
    /// </summary>
    public async Task CelebrateAsync()
    {
        if (_currentStarRating > 0)
        {
            await AnimateStarsEarnedAsync(0, _currentStarRating);
        }
    }

    #endregion
}

#region Event Args

/// <summary>
/// Event arguments for star tapped events.
/// </summary>
public class StarTappedEventArgs : EventArgs
{
    public int StarNumber { get; set; }
    public int TotalStars { get; set; }
}

/// <summary>
/// Event arguments for animation completed events.
/// </summary>
public class StarAnimationCompletedEventArgs : EventArgs
{
    public int PreviousStars { get; set; }
    public int CurrentStars { get; set; }
    public bool IsEarningStars { get; set; }
}

#endregion

/// <summary>
/// Helper class for service resolution.
/// </summary>
public static class ServiceHelper
{
    public static T? GetService<T>() where T : class
    {
        try
        {
            return Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetService<T>(
                Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services);
        }
        catch
        {
            return null;
        }
    }
}