using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace EduPlayKids.App.Controls.Audio;

/// <summary>
/// A volume control component designed for parental settings and child-friendly audio management.
/// Provides safe volume limits, visual feedback, and easy-to-use controls for different audio types.
/// </summary>
public partial class AudioVolumeControl : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty AudioTypeProperty =
        BindableProperty.Create(nameof(AudioType), typeof(AudioType), typeof(AudioVolumeControl), AudioType.UIInteraction, propertyChanged: OnAudioTypeChanged);

    public static readonly BindableProperty VolumeLevelProperty =
        BindableProperty.Create(nameof(VolumeLevel), typeof(double), typeof(AudioVolumeControl), 0.7, BindingMode.TwoWay, propertyChanged: OnVolumeLevelChanged);

    public static readonly BindableProperty MaxVolumeLevelProperty =
        BindableProperty.Create(nameof(MaxVolumeLevel), typeof(double), typeof(AudioVolumeControl), 0.85);

    public static readonly BindableProperty ShowControlButtonsProperty =
        BindableProperty.Create(nameof(ShowControlButtons), typeof(bool), typeof(AudioVolumeControl), true);

    public static readonly BindableProperty ShowVolumeIconsProperty =
        BindableProperty.Create(nameof(ShowVolumeIcons), typeof(bool), typeof(AudioVolumeControl), true);

    public static readonly BindableProperty ShowResetButtonProperty =
        BindableProperty.Create(nameof(ShowResetButton), typeof(bool), typeof(AudioVolumeControl), true);

    public static readonly BindableProperty IsVolumeChangingProperty =
        BindableProperty.Create(nameof(IsVolumeChanging), typeof(bool), typeof(AudioVolumeControl), false);

    public static readonly BindableProperty CanTestAudioProperty =
        BindableProperty.Create(nameof(CanTestAudio), typeof(bool), typeof(AudioVolumeControl), true);

    #endregion

    #region Commands

    public ICommand ToggleMuteCommand { get; private set; }
    public ICommand TestAudioCommand { get; private set; }
    public ICommand ResetVolumeCommand { get; private set; }
    public ICommand SetLowVolumeCommand { get; private set; }
    public ICommand SetHighVolumeCommand { get; private set; }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the audio type that this control manages.
    /// </summary>
    public AudioType AudioType
    {
        get => (AudioType)GetValue(AudioTypeProperty);
        set => SetValue(AudioTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the current volume level (0.0 to 1.0).
    /// </summary>
    public double VolumeLevel
    {
        get => (double)GetValue(VolumeLevelProperty);
        set => SetValue(VolumeLevelProperty, Math.Clamp(value, 0.0, MaxVolumeLevel));
    }

    /// <summary>
    /// Gets or sets the maximum allowed volume level for child safety.
    /// </summary>
    public double MaxVolumeLevel
    {
        get => (double)GetValue(MaxVolumeLevelProperty);
        set => SetValue(MaxVolumeLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show control buttons (mute, test, reset).
    /// </summary>
    public bool ShowControlButtons
    {
        get => (bool)GetValue(ShowControlButtonsProperty);
        set => SetValue(ShowControlButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show volume level icons.
    /// </summary>
    public bool ShowVolumeIcons
    {
        get => (bool)GetValue(ShowVolumeIconsProperty);
        set => SetValue(ShowVolumeIconsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the reset button.
    /// </summary>
    public bool ShowResetButton
    {
        get => (bool)GetValue(ShowResetButtonProperty);
        set => SetValue(ShowResetButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the volume is currently being changed.
    /// </summary>
    public bool IsVolumeChanging
    {
        get => (bool)GetValue(IsVolumeChangingProperty);
        set => SetValue(IsVolumeChangingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the test audio button is enabled.
    /// </summary>
    public bool CanTestAudio
    {
        get => (bool)GetValue(CanTestAudioProperty);
        set => SetValue(CanTestAudioProperty, value);
    }

    /// <summary>
    /// Gets the display name for the audio type.
    /// </summary>
    public string AudioTypeDisplayName => GetAudioTypeDisplayName(AudioType);

    /// <summary>
    /// Gets the volume percentage text for display.
    /// </summary>
    public string VolumePercentageText => $"{VolumeLevel:P0}";

    /// <summary>
    /// Gets the mute button text based on current state.
    /// </summary>
    public string MuteButtonText => _isMuted ? "🔇" : "🔊";

    /// <summary>
    /// Gets the mute button color based on current state.
    /// </summary>
    public Color MuteButtonColor => _isMuted ? Colors.Red : Colors.Transparent;

    #endregion

    #region Private Fields

    private readonly IAudioService? _audioService;
    private readonly ILogger<AudioVolumeControl>? _logger;
    private bool _isMuted;
    private double _volumeBeforeMute;
    private DateTime _lastVolumeChangeTime;
    private readonly Timer _volumeChangeTimer;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when the volume level changes.
    /// </summary>
    public event EventHandler<AudioVolumeEventArgs>? VolumeChanged;

    /// <summary>
    /// Event raised when the mute state changes.
    /// </summary>
    public event EventHandler<bool>? MuteStateChanged;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the AudioVolumeControl class.
    /// </summary>
    public AudioVolumeControl()
    {
        InitializeComponent();

        // Try to get services from dependency injection
        try
        {
            _audioService = Handler?.MauiContext?.Services.GetService<IAudioService>();
            _logger = Handler?.MauiContext?.Services.GetService<ILogger<AudioVolumeControl>>();
        }
        catch
        {
            // Services not available during design time
        }

        // Initialize commands
        InitializeCommands();

        // Set up volume change debouncing timer
        _volumeChangeTimer = new Timer(OnVolumeChangeTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);

        // Initialize slider maximum based on child safety limits
        UpdateSliderMaximum();
    }

    #endregion

    #region Event Handlers

    private void OnVolumeChanged(object? sender, ValueChangedEventArgs e)
    {
        IsVolumeChanging = true;
        _lastVolumeChangeTime = DateTime.Now;

        // Debounce volume changes to avoid too many updates
        _volumeChangeTimer.Change(300, Timeout.Infinite);

        // Update dependent properties
        OnPropertyChanged(nameof(VolumePercentageText));
        OnPropertyChanged(nameof(MuteButtonText));
        OnPropertyChanged(nameof(MuteButtonColor));

        // Reset mute state if volume was manually changed
        if (_isMuted && e.NewValue > 0)
        {
            _isMuted = false;
            OnPropertyChanged(nameof(MuteButtonText));
            OnPropertyChanged(nameof(MuteButtonColor));
        }
    }

    private void OnVolumeDragCompleted(object? sender, EventArgs e)
    {
        // Apply volume change immediately when drag is completed
        ApplyVolumeChange();
    }

    private void OnVolumeChangeTimerElapsed(object? state)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            IsVolumeChanging = false;
            ApplyVolumeChange();
        });
    }

    #endregion

    #region Private Methods

    private void InitializeCommands()
    {
        ToggleMuteCommand = new Command(async () => await ToggleMuteAsync());
        TestAudioCommand = new Command(async () => await TestAudioAsync(), () => CanTestAudio && !_isMuted);
        ResetVolumeCommand = new Command(ResetVolume);
        SetLowVolumeCommand = new Command(() => VolumeLevel = 0.3);
        SetHighVolumeCommand = new Command(() => VolumeLevel = MaxVolumeLevel);
    }

    private async Task ToggleMuteAsync()
    {
        try
        {
            if (_isMuted)
            {
                // Unmute: restore previous volume
                VolumeLevel = _volumeBeforeMute > 0 ? _volumeBeforeMute : 0.5;
                _isMuted = false;
            }
            else
            {
                // Mute: save current volume and set to 0
                _volumeBeforeMute = VolumeLevel;
                VolumeLevel = 0;
                _isMuted = true;
            }

            OnPropertyChanged(nameof(MuteButtonText));
            OnPropertyChanged(nameof(MuteButtonColor));

            // Apply volume change to audio service
            ApplyVolumeChange();

            // Notify listeners
            MuteStateChanged?.Invoke(this, _isMuted);

            _logger?.LogDebug("Audio {AudioType} {Action}", AudioType, _isMuted ? "muted" : "unmuted");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error toggling mute for audio type {AudioType}", AudioType);
        }
    }

    private async Task TestAudioAsync()
    {
        if (_audioService == null || _isMuted)
        {
            return;
        }

        try
        {
            CanTestAudio = false;

            // Play appropriate test audio based on audio type
            switch (AudioType)
            {
                case AudioType.UIInteraction:
                    await _audioService.PlayUIFeedbackAsync(UIInteractionType.ButtonPress);
                    break;

                case AudioType.SuccessFeedback:
                    await _audioService.PlaySuccessFeedbackAsync(FeedbackIntensity.Medium);
                    break;

                case AudioType.ErrorFeedback:
                    await _audioService.PlayErrorFeedbackAsync(FeedbackIntensity.Soft);
                    break;

                case AudioType.Completion:
                    await _audioService.PlayActivityCompletionAsync(3); // 3 stars
                    break;

                case AudioType.Instruction:
                    await _audioService.PlayInstructionAsync("test_instruction");
                    break;

                case AudioType.BackgroundMusic:
                    await _audioService.PlayBackgroundMusicAsync("test", VolumeLevel * 0.8f, false);
                    break;

                case AudioType.Achievement:
                    await _audioService.PlaySuccessFeedbackAsync(FeedbackIntensity.High);
                    break;

                case AudioType.Mascot:
                    await _audioService.PlayInstructionAsync("mascot_greeting");
                    break;
            }

            _logger?.LogDebug("Played test audio for {AudioType} at volume {Volume}", AudioType, VolumeLevel);

            // Brief delay before re-enabling
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error playing test audio for {AudioType}", AudioType);
        }
        finally
        {
            CanTestAudio = true;
        }
    }

    private void ResetVolume()
    {
        try
        {
            // Reset to default volume based on audio type
            VolumeLevel = GetDefaultVolumeForType(AudioType);
            _isMuted = false;

            OnPropertyChanged(nameof(MuteButtonText));
            OnPropertyChanged(nameof(MuteButtonColor));

            ApplyVolumeChange();

            _logger?.LogDebug("Reset volume for {AudioType} to {Volume}", AudioType, VolumeLevel);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resetting volume for {AudioType}", AudioType);
        }
    }

    private void ApplyVolumeChange()
    {
        if (_audioService == null) return;

        try
        {
            // Apply volume to audio service
            _ = Task.Run(async () =>
            {
                await _audioService.SetVolumeLevelAsync(AudioType, (float)VolumeLevel);
            });

            // Raise volume changed event
            var previousVolume = _volumeBeforeMute > 0 ? _volumeBeforeMute : VolumeLevel;
            var eventArgs = new AudioVolumeEventArgs(AudioType, (float)VolumeLevel, (float)previousVolume, true);
            VolumeChanged?.Invoke(this, eventArgs);

        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying volume change for {AudioType}", AudioType);
        }
    }

    private void UpdateSliderMaximum()
    {
        if (VolumeSlider != null)
        {
            VolumeSlider.Maximum = MaxVolumeLevel;
        }
    }

    private static string GetAudioTypeDisplayName(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.UIInteraction => "UI Sounds",
            AudioType.Instruction => "Instructions",
            AudioType.SuccessFeedback => "Success Sounds",
            AudioType.ErrorFeedback => "Correction Sounds",
            AudioType.Completion => "Completion Sounds",
            AudioType.BackgroundMusic => "Background Music",
            AudioType.Achievement => "Achievement Sounds",
            AudioType.Mascot => "Leo the Lion",
            _ => audioType.ToString()
        };
    }

    private static double GetDefaultVolumeForType(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.UIInteraction => 0.7,
            AudioType.Instruction => 0.9,
            AudioType.SuccessFeedback => 0.8,
            AudioType.ErrorFeedback => 0.6,
            AudioType.Completion => 0.9,
            AudioType.BackgroundMusic => 0.3,
            AudioType.Achievement => 0.8,
            AudioType.Mascot => 0.8,
            _ => 0.7
        };
    }

    private static void OnAudioTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AudioVolumeControl control)
        {
            control.OnPropertyChanged(nameof(AudioTypeDisplayName));

            // Set default volume for the new audio type
            if (control.VolumeLevel == GetDefaultVolumeForType((AudioType)oldValue))
            {
                control.VolumeLevel = GetDefaultVolumeForType((AudioType)newValue);
            }
        }
    }

    private static void OnVolumeLevelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AudioVolumeControl control)
        {
            control.OnPropertyChanged(nameof(VolumePercentageText));
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the volume level programmatically.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <param name="animate">Whether to animate the change</param>
    public async Task SetVolumeAsync(double volume, bool animate = true)
    {
        var clampedVolume = Math.Clamp(volume, 0.0, MaxVolumeLevel);

        if (animate && VolumeSlider != null)
        {
            await VolumeSlider.TranslateTo(VolumeSlider.X, VolumeSlider.Y, 200, Easing.CubicOut);
        }

        VolumeLevel = clampedVolume;
    }

    /// <summary>
    /// Synchronizes this control with the audio service volume.
    /// </summary>
    public async Task SynchronizeWithAudioServiceAsync()
    {
        if (_audioService == null) return;

        try
        {
            var currentVolume = _audioService.GetVolumeLevel(AudioType);
            VolumeLevel = currentVolume;

            _logger?.LogDebug("Synchronized volume control for {AudioType} with service volume {Volume}",
                AudioType, currentVolume);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error synchronizing volume control with audio service for {AudioType}", AudioType);
        }
    }

    /// <summary>
    /// Updates the maximum volume level for child safety.
    /// </summary>
    /// <param name="maxVolume">New maximum volume level</param>
    public void UpdateMaxVolumeLevel(double maxVolume)
    {
        MaxVolumeLevel = Math.Clamp(maxVolume, 0.1, 1.0);

        // Adjust current volume if it exceeds new maximum
        if (VolumeLevel > MaxVolumeLevel)
        {
            VolumeLevel = MaxVolumeLevel;
        }

        UpdateSliderMaximum();
    }

    #endregion

    #region IDisposable

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent == null)
        {
            // Cleanup when removed from parent
            _volumeChangeTimer?.Dispose();
        }
    }

    #endregion
}