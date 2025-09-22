using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace EduPlayKids.App.Controls.Audio;

/// <summary>
/// A custom button control that provides automatic audio feedback for child interactions.
/// Designed for children aged 3-8 with large touch targets, visual feedback, and appropriate audio cues.
/// </summary>
public partial class AudioFeedbackButton : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty ButtonTextProperty =
        BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(AudioFeedbackButton), string.Empty);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(AudioFeedbackButton), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(AudioFeedbackButton), null);

    public static readonly BindableProperty AudioFeedbackTypeProperty =
        BindableProperty.Create(nameof(AudioFeedbackType), typeof(UIInteractionType), typeof(AudioFeedbackButton), UIInteractionType.ButtonPress);

    public static readonly BindableProperty ButtonStyleProperty =
        BindableProperty.Create(nameof(ButtonStyle), typeof(Style), typeof(AudioFeedbackButton), null, propertyChanged: OnButtonStyleChanged);

    public static readonly BindableProperty FeedbackTypeProperty =
        BindableProperty.Create(nameof(FeedbackType), typeof(AudioButtonFeedbackType), typeof(AudioFeedbackButton), AudioButtonFeedbackType.Standard);

    public static readonly BindableProperty IsPlayingAudioProperty =
        BindableProperty.Create(nameof(IsPlayingAudio), typeof(bool), typeof(AudioFeedbackButton), false);

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(AudioFeedbackButton), false);

    public static readonly BindableProperty EnableAudioFeedbackProperty =
        BindableProperty.Create(nameof(EnableAudioFeedback), typeof(bool), typeof(AudioFeedbackButton), true);

    public static readonly BindableProperty AudioInstructionKeyProperty =
        BindableProperty.Create(nameof(AudioInstructionKey), typeof(string), typeof(AudioFeedbackButton), null);

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the text displayed on the button.
    /// </summary>
    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the button is pressed.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter to pass to the command.
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of audio feedback to play on interaction.
    /// </summary>
    public UIInteractionType AudioFeedbackType
    {
        get => (UIInteractionType)GetValue(AudioFeedbackTypeProperty);
        set => SetValue(AudioFeedbackTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual style of the button.
    /// </summary>
    public Style ButtonStyle
    {
        get => (Style)GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the feedback type which determines button appearance and audio.
    /// </summary>
    public AudioButtonFeedbackType FeedbackType
    {
        get => (AudioButtonFeedbackType)GetValue(FeedbackTypeProperty);
        set => SetValue(FeedbackTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether audio is currently playing for this button.
    /// </summary>
    public bool IsPlayingAudio
    {
        get => (bool)GetValue(IsPlayingAudioProperty);
        set => SetValue(IsPlayingAudioProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the button is in a loading state.
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether audio feedback is enabled for this button.
    /// </summary>
    public bool EnableAudioFeedback
    {
        get => (bool)GetValue(EnableAudioFeedbackProperty);
        set => SetValue(EnableAudioFeedbackProperty, value);
    }

    /// <summary>
    /// Gets or sets the audio instruction key to play when the button is long-pressed.
    /// </summary>
    public string? AudioInstructionKey
    {
        get => (string?)GetValue(AudioInstructionKeyProperty);
        set => SetValue(AudioInstructionKeyProperty, value);
    }

    #endregion

    #region Private Fields

    private readonly IAudioService? _audioService;
    private readonly ILogger<AudioFeedbackButton>? _logger;
    private bool _isPressed;
    private DateTime _pressStartTime;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the AudioFeedbackButton class.
    /// </summary>
    public AudioFeedbackButton()
    {
        InitializeComponent();

        // Try to get audio service from dependency injection
        try
        {
            _audioService = Handler?.MauiContext?.Services.GetService<IAudioService>();
            _logger = Handler?.MauiContext?.Services.GetService<ILogger<AudioFeedbackButton>>();
        }
        catch
        {
            // Services not available during design time
        }

        // Set up event handlers
        MainButton.Pressed += OnButtonPressed;
        MainButton.Released += OnButtonReleased;
        MainButton.Clicked += OnButtonClicked;

        // Apply initial style based on feedback type
        UpdateButtonStyle();
    }

    #endregion

    #region Event Handlers

    private async void OnButtonPressed(object? sender, EventArgs e)
    {
        _isPressed = true;
        _pressStartTime = DateTime.Now;

        // Play tactile feedback audio immediately
        if (EnableAudioFeedback && _audioService != null)
        {
            try
            {
                await _audioService.PlayUIFeedbackAsync(AudioFeedbackType);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error playing button press audio feedback");
            }
        }

        // Start visual feedback animation
        await MainButton.ScaleTo(0.95, 100, Easing.CubicOut);
    }

    private async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (!_isPressed) return;

        _isPressed = false;
        var pressDuration = DateTime.Now - _pressStartTime;

        // Restore visual state
        await MainButton.ScaleTo(1.0, 100, Easing.CubicOut);

        // Check for long press (instruction audio)
        if (pressDuration.TotalMilliseconds > 1000 && !string.IsNullOrEmpty(AudioInstructionKey))
        {
            await PlayInstructionAudio();
        }
    }

    private async void OnButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            // Set loading state if command is async
            if (Command != null && IsAsyncCommand(Command))
            {
                IsLoading = true;
            }

            // Execute the command
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }

            // Play success feedback based on feedback type
            if (EnableAudioFeedback && _audioService != null)
            {
                await PlayFeedbackAudio();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling button click");

            // Play error feedback
            if (EnableAudioFeedback && _audioService != null)
            {
                try
                {
                    await _audioService.PlayErrorFeedbackAsync(FeedbackIntensity.Soft);
                }
                catch (Exception audioEx)
                {
                    _logger?.LogError(audioEx, "Error playing error feedback audio");
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Private Methods

    private async Task PlayFeedbackAudio()
    {
        if (_audioService == null) return;

        try
        {
            IsPlayingAudio = true;

            switch (FeedbackType)
            {
                case AudioButtonFeedbackType.Success:
                    await _audioService.PlaySuccessFeedbackAsync(FeedbackIntensity.Medium);
                    break;

                case AudioButtonFeedbackType.Error:
                    await _audioService.PlayErrorFeedbackAsync(FeedbackIntensity.Soft);
                    break;

                case AudioButtonFeedbackType.Standard:
                default:
                    await _audioService.PlayUIFeedbackAsync(AudioFeedbackType);
                    break;
            }

            // Keep indicator visible for minimum duration
            await Task.Delay(500);
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    private async Task PlayInstructionAudio()
    {
        if (_audioService == null || string.IsNullOrEmpty(AudioInstructionKey)) return;

        try
        {
            IsPlayingAudio = true;
            await _audioService.PlayInstructionAsync(AudioInstructionKey);
            await Task.Delay(1000); // Keep indicator visible
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error playing instruction audio: {InstructionKey}", AudioInstructionKey);
        }
        finally
        {
            IsPlayingAudio = false;
        }
    }

    private bool IsAsyncCommand(ICommand command)
    {
        // Simple heuristic to detect async commands
        var commandType = command.GetType();
        return commandType.Name.Contains("Async") ||
               commandType.GetMethods().Any(m => m.ReturnType == typeof(Task));
    }

    private void UpdateButtonStyle()
    {
        if (ButtonStyle != null) return;

        // Apply default style based on feedback type
        var styleName = FeedbackType switch
        {
            AudioButtonFeedbackType.Success => "SuccessButtonStyle",
            AudioButtonFeedbackType.Error => "ErrorButtonStyle",
            AudioButtonFeedbackType.Standard => "StandardButtonStyle",
            _ => "StandardButtonStyle"
        };

        if (Resources.TryGetValue(styleName, out var style) && style is Style buttonStyle)
        {
            MainButton.Style = buttonStyle;
        }
    }

    private static void OnButtonStyleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AudioFeedbackButton button && newValue is Style style)
        {
            button.MainButton.Style = style;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Triggers the button click programmatically with audio feedback.
    /// </summary>
    public async Task ClickAsync()
    {
        OnButtonClicked(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Plays the instruction audio for this button.
    /// </summary>
    public async Task PlayInstructionAsync()
    {
        await PlayInstructionAudio();
    }

    /// <summary>
    /// Sets the button to success state with appropriate visual and audio feedback.
    /// </summary>
    public async Task SetSuccessStateAsync()
    {
        FeedbackType = AudioButtonFeedbackType.Success;
        UpdateButtonStyle();

        if (EnableAudioFeedback && _audioService != null)
        {
            await _audioService.PlaySuccessFeedbackAsync(FeedbackIntensity.Medium);
        }
    }

    /// <summary>
    /// Sets the button to error state with appropriate visual and audio feedback.
    /// </summary>
    public async Task SetErrorStateAsync()
    {
        FeedbackType = AudioButtonFeedbackType.Error;
        UpdateButtonStyle();

        if (EnableAudioFeedback && _audioService != null)
        {
            await _audioService.PlayErrorFeedbackAsync(FeedbackIntensity.Soft);
        }
    }

    /// <summary>
    /// Resets the button to standard state.
    /// </summary>
    public void ResetToStandardState()
    {
        FeedbackType = AudioButtonFeedbackType.Standard;
        UpdateButtonStyle();
        IsLoading = false;
        IsPlayingAudio = false;
    }

    #endregion
}

/// <summary>
/// Defines the types of audio feedback for buttons.
/// </summary>
public enum AudioButtonFeedbackType
{
    /// <summary>
    /// Standard button with normal interaction sounds.
    /// </summary>
    Standard,

    /// <summary>
    /// Success button with positive reinforcement sounds.
    /// </summary>
    Success,

    /// <summary>
    /// Error button with gentle correction sounds.
    /// </summary>
    Error
}