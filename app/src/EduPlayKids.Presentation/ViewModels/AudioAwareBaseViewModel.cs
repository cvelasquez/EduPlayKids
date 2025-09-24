using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// Audio-aware base ViewModel that provides audio feedback capabilities for child-friendly interactions.
/// Extends BaseViewModel with comprehensive audio support for educational applications targeting children aged 3-8.
/// </summary>
public abstract class AudioAwareBaseViewModel : BaseViewModel
{
    #region Protected Fields

    protected readonly IAudioService? _audioService;
    private bool _audioEnabled = true;
    private bool _isPlayingAudio = false;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets whether audio feedback is enabled for this ViewModel.
    /// </summary>
    public bool AudioEnabled
    {
        get => _audioEnabled;
        set => SetProperty(ref _audioEnabled, value);
    }

    /// <summary>
    /// Gets whether audio is currently playing from this ViewModel.
    /// </summary>
    public bool IsPlayingAudio
    {
        get => _isPlayingAudio;
        private set => SetProperty(ref _isPlayingAudio, value);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the AudioAwareBaseViewModel class.
    /// </summary>
    /// <param name="logger">Logger for debugging and error tracking</param>
    /// <param name="audioService">Audio service for sound feedback</param>
    protected AudioAwareBaseViewModel(ILogger logger, IAudioService? audioService = null)
        : base(logger)
    {
        _audioService = audioService;

        // Subscribe to audio service events if available
        if (_audioService != null)
        {
            _audioService.AudioStarted += OnAudioStarted;
            _audioService.AudioStopped += OnAudioStopped;
            _audioService.AudioError += OnAudioError;
        }

        _logger.LogDebug("AudioAwareBaseViewModel initialized with audio support: {HasAudio}", _audioService != null);
    }

    #endregion

    #region Audio Feedback Methods

    /// <summary>
    /// Plays success feedback audio with child-appropriate celebration.
    /// </summary>
    /// <param name="intensity">Intensity of the success feedback</param>
    /// <param name="childAge">Optional child age for age-appropriate feedback</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlaySuccessFeedbackAsync(FeedbackIntensity intensity = FeedbackIntensity.Medium, int? childAge = null)
    {
        if (!AudioEnabled || _audioService == null) return;

        try
        {
            await _audioService.PlaySuccessFeedbackAsync(intensity, childAge);
            _logger.LogDebug("Played success feedback with intensity {Intensity}", intensity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing success feedback");
        }
    }

    /// <summary>
    /// Plays gentle error feedback audio that encourages continued learning.
    /// </summary>
    /// <param name="encouragement">Level of encouragement in the feedback</param>
    /// <param name="childAge">Optional child age for age-appropriate feedback</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayErrorFeedbackAsync(FeedbackIntensity encouragement = FeedbackIntensity.Soft, int? childAge = null)
    {
        if (!AudioEnabled || _audioService == null) return;

        try
        {
            await _audioService.PlayErrorFeedbackAsync(encouragement, childAge);
            _logger.LogDebug("Played error feedback with encouragement {Encouragement}", encouragement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing error feedback");
        }
    }

    /// <summary>
    /// Plays activity completion audio with star rating celebration.
    /// </summary>
    /// <param name="starRating">Number of stars earned (1-3)</param>
    /// <param name="activityType">Type of activity completed</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayCompletionFeedbackAsync(int starRating, string? activityType = null)
    {
        if (!AudioEnabled || _audioService == null) return;

        try
        {
            await _audioService.PlayActivityCompletionAsync(starRating, activityType);
            _logger.LogDebug("Played completion feedback for {Stars} stars, activity: {Activity}", starRating, activityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing completion feedback");
        }
    }

    /// <summary>
    /// Plays UI interaction feedback for navigation and button presses.
    /// </summary>
    /// <param name="interactionType">Type of UI interaction</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayUIFeedbackAsync(UIInteractionType interactionType = UIInteractionType.ButtonPress)
    {
        if (!AudioEnabled || _audioService == null) return;

        try
        {
            await _audioService.PlayUIFeedbackAsync(interactionType);
            _logger.LogDebug("Played UI feedback for {Interaction}", interactionType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing UI feedback");
        }
    }

    /// <summary>
    /// Plays instructional audio for educational content.
    /// </summary>
    /// <param name="instructionKey">Localization key for the instruction</param>
    /// <param name="childAge">Optional child age for appropriate speech rate</param>
    /// <returns>True if instruction audio was played successfully</returns>
    protected async Task<bool> PlayInstructionAsync(string instructionKey, int? childAge = null)
    {
        if (!AudioEnabled || _audioService == null || string.IsNullOrEmpty(instructionKey))
        {
            return false;
        }

        try
        {
            var success = await _audioService.PlayInstructionAsync(instructionKey, childAge);
            _logger.LogDebug("Played instruction audio: {Key}, Success: {Success}", instructionKey, success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing instruction audio: {Key}", instructionKey);
            return false;
        }
    }

    /// <summary>
    /// Starts background music appropriate for the current activity.
    /// </summary>
    /// <param name="activityType">Type of activity for music selection</param>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <returns>True if background music started successfully</returns>
    protected async Task<bool> StartBackgroundMusicAsync(string activityType, float volume = 0.3f)
    {
        if (!AudioEnabled || _audioService == null) return false;

        try
        {
            var success = await _audioService.PlayBackgroundMusicAsync(activityType, volume, loop: true);
            _logger.LogDebug("Started background music: {Activity}, Volume: {Volume}, Success: {Success}",
                activityType, volume, success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting background music: {Activity}", activityType);
            return false;
        }
    }

    /// <summary>
    /// Stops background music with a smooth fade-out.
    /// </summary>
    /// <param name="fadeOutDuration">Fade-out duration in milliseconds</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task StopBackgroundMusicAsync(int fadeOutDuration = 500)
    {
        if (_audioService == null) return;

        try
        {
            await _audioService.StopAudioAsync(AudioType.BackgroundMusic, fadeOutDuration);
            _logger.LogDebug("Stopped background music with fade-out: {Duration}ms", fadeOutDuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping background music");
        }
    }

    #endregion

    #region Educational Audio Methods

    /// <summary>
    /// Plays audio for a correct answer with appropriate celebration.
    /// </summary>
    /// <param name="consecutiveCorrect">Number of consecutive correct answers</param>
    /// <param name="childAge">Child's age for appropriate feedback</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayCorrectAnswerFeedbackAsync(int consecutiveCorrect = 1, int? childAge = null)
    {
        if (!AudioEnabled) return;

        // Escalate celebration based on consecutive correct answers
        var intensity = consecutiveCorrect switch
        {
            1 => FeedbackIntensity.Medium,
            >= 2 and <= 4 => FeedbackIntensity.High,
            > 4 => FeedbackIntensity.High, // Keep high but maybe add special audio
            _ => FeedbackIntensity.Medium
        };

        await PlaySuccessFeedbackAsync(intensity, childAge);

        // Play special celebration for achievement streaks
        if (consecutiveCorrect >= 5)
        {
            await Task.Delay(500); // Brief pause
            await PlayInstructionAsync("achievement_streak", childAge);
        }
    }

    /// <summary>
    /// Plays audio for an incorrect answer with encouraging correction.
    /// </summary>
    /// <param name="attemptsRemaining">Number of attempts remaining</param>
    /// <param name="childAge">Child's age for appropriate feedback</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayIncorrectAnswerFeedbackAsync(int attemptsRemaining = 2, int? childAge = null)
    {
        if (!AudioEnabled) return;

        // Adjust encouragement based on remaining attempts
        var encouragement = attemptsRemaining switch
        {
            > 1 => FeedbackIntensity.Soft, // Gentle for early attempts
            1 => FeedbackIntensity.Medium, // More supportive for last attempt
            0 => FeedbackIntensity.High,   // Very encouraging when showing answer
            _ => FeedbackIntensity.Soft
        };

        await PlayErrorFeedbackAsync(encouragement, childAge);

        // Play encouraging instruction based on attempts
        var instructionKey = attemptsRemaining switch
        {
            > 1 => "try_again_gentle",
            1 => "try_again_encouraging",
            0 => "show_correct_answer",
            _ => "try_again_gentle"
        };

        await Task.Delay(300); // Brief pause
        await PlayInstructionAsync(instructionKey, childAge);
    }

    /// <summary>
    /// Plays activity-specific instruction audio when the child enters a new screen.
    /// </summary>
    /// <param name="activityType">Type of activity for appropriate instructions</param>
    /// <param name="childAge">Child's age for appropriate complexity</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayActivityIntroductionAsync(string activityType, int? childAge = null)
    {
        if (!AudioEnabled) return;

        var instructionKey = $"activity_intro_{activityType}";
        await PlayInstructionAsync(instructionKey, childAge);
    }

    /// <summary>
    /// Plays encouragement audio for a child who seems to be struggling.
    /// </summary>
    /// <param name="errorCount">Number of recent errors</param>
    /// <param name="childAge">Child's age for appropriate encouragement</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayEncouragementAsync(int errorCount, int? childAge = null)
    {
        if (!AudioEnabled || errorCount < 2) return;

        var encouragementKey = errorCount switch
        {
            >= 2 and <= 3 => "encouragement_gentle",
            >= 4 and <= 5 => "encouragement_supportive",
            > 5 => "encouragement_motivating",
            _ => "encouragement_gentle"
        };

        await PlayInstructionAsync(encouragementKey, childAge);
    }

    #endregion

    #region Audio State Management

    /// <summary>
    /// Gets the current volume level for a specific audio type.
    /// </summary>
    /// <param name="audioType">Audio type to check</param>
    /// <returns>Current volume level (0.0 to 1.0)</returns>
    protected float GetAudioVolume(AudioType audioType)
    {
        return _audioService?.GetVolumeLevel(audioType) ?? 0.0f;
    }

    /// <summary>
    /// Sets the volume level for a specific audio type.
    /// </summary>
    /// <param name="audioType">Audio type to adjust</param>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task SetAudioVolumeAsync(AudioType audioType, float volume)
    {
        if (_audioService == null) return;

        try
        {
            await _audioService.SetVolumeLevelAsync(audioType, volume);
            _logger.LogDebug("Set audio volume for {AudioType} to {Volume}", audioType, volume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting audio volume for {AudioType}", audioType);
        }
    }

    /// <summary>
    /// Checks if any audio is currently playing.
    /// </summary>
    /// <returns>True if audio is playing, false otherwise</returns>
    protected bool IsAnyAudioPlaying()
    {
        return _audioService?.IsAnyAudioPlaying() ?? false;
    }

    /// <summary>
    /// Stops all currently playing audio.
    /// </summary>
    /// <param name="fadeOutDuration">Fade-out duration in milliseconds</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task StopAllAudioAsync(int fadeOutDuration = 200)
    {
        if (_audioService == null) return;

        try
        {
            await _audioService.StopAudioAsync(fadeOutDuration: fadeOutDuration);
            _logger.LogDebug("Stopped all audio with fade-out: {Duration}ms", fadeOutDuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping all audio");
        }
    }

    #endregion

    #region Navigation Audio

    /// <summary>
    /// Plays audio feedback when navigating to a new page.
    /// </summary>
    /// <param name="pageType">Type of page being navigated to</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayNavigationAudioAsync(string pageType)
    {
        await PlayUIFeedbackAsync(UIInteractionType.PageTransition);

        // Play page-specific welcome audio if available
        var welcomeKey = $"welcome_{pageType}";
        await Task.Delay(200); // Brief pause after navigation sound
        await PlayInstructionAsync(welcomeKey);
    }

    /// <summary>
    /// Plays audio feedback when going back to a previous page.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayBackNavigationAudioAsync()
    {
        await PlayUIFeedbackAsync(UIInteractionType.ButtonPress);
        await PlayInstructionAsync("navigation_back");
    }

    /// <summary>
    /// Plays audio from a specified file path with given type and language.
    /// Generic method for playing any audio file with proper categorization.
    /// </summary>
    /// <param name="audioFilePath">Path to the audio file</param>
    /// <param name="audioType">Type of audio for proper categorization</param>
    /// <param name="language">Language for the audio content</param>
    /// <returns>Task representing the async operation</returns>
    protected async Task PlayAudioAsync(string audioFilePath, AudioType audioType, string language = "en")
    {
        if (!AudioEnabled || _audioService == null) return;

        try
        {
            var audioItem = new AudioItem
            {
                FilePath = audioFilePath,
                Type = audioType,
                Language = language == "es" ? AudioLanguage.Spanish.ToString().ToLowerInvariant() : AudioLanguage.English.ToString().ToLowerInvariant(),
                Priority = audioType switch
                {
                    AudioType.Instruction => AudioPriority.Critical,
                    AudioType.SuccessFeedback => AudioPriority.High,
                    AudioType.ErrorFeedback => AudioPriority.High,
                    AudioType.Achievement => AudioPriority.High,
                    AudioType.UIInteraction => AudioPriority.Normal,
                    _ => AudioPriority.Normal
                }
            };

            await _audioService.PlayAudioAsync(audioItem);
            _logger.LogDebug("Playing audio: {FilePath} ({Type})", audioFilePath, audioType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing audio: {FilePath}", audioFilePath);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when the ViewModel is appearing (page is being navigated to).
    /// Override to add page-specific audio introduction.
    /// </summary>
    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();

        // Play default page introduction audio
        await PlayPageIntroductionAudio();
    }

    /// <summary>
    /// Called when the ViewModel is disappearing (page is being navigated away from).
    /// Stops any playing audio to prevent conflicts.
    /// </summary>
    public override async Task OnDisappearingAsync()
    {
        await base.OnDisappearingAsync();

        // Stop non-critical audio when leaving page
        if (_audioService != null)
        {
            await _audioService.StopAudioAsync(AudioType.Instruction, fadeOutDuration: 100);
            await _audioService.StopAudioAsync(AudioType.BackgroundMusic, fadeOutDuration: 300);

            // Unsubscribe from audio service events
            _audioService.AudioStarted -= OnAudioStarted;
            _audioService.AudioStopped -= OnAudioStopped;
            _audioService.AudioError -= OnAudioError;

            // Stop any playing audio from this ViewModel
            try
            {
                await StopAllAudioAsync(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping audio during ViewModel cleanup");
            }
        }
    }

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Plays introduction audio for the current page.
    /// Override in derived classes to provide page-specific introductions.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    protected virtual async Task PlayPageIntroductionAudio()
    {
        // Default implementation - override in derived classes
        var pageType = GetType().Name.Replace("ViewModel", "").ToLowerInvariant();
        await PlayInstructionAsync($"page_intro_{pageType}");
    }

    /// <summary>
    /// Called when an audio error occurs.
    /// Override to handle audio errors specific to the page.
    /// </summary>
    /// <param name="error">The audio error that occurred</param>
    protected virtual void OnAudioErrorOccurred(AudioErrorEventArgs error)
    {
        _logger.LogWarning("Audio error in {ViewModel}: {Error}", GetType().Name, error.Message);

        // For children's apps, we generally don't want to show technical errors
        // Instead, we might want to gracefully degrade to visual-only feedback
        AudioEnabled = error.IsRecoverable;
    }

    #endregion

    #region Private Methods

    private void OnAudioStarted(object? sender, AudioPlaybackEventArgs e)
    {
        IsPlayingAudio = true;
    }

    private void OnAudioStopped(object? sender, AudioPlaybackEventArgs e)
    {
        IsPlayingAudio = false;
    }

    private void OnAudioError(object? sender, AudioErrorEventArgs e)
    {
        OnAudioErrorOccurred(e);
    }

    #endregion

    #region Cleanup


    #endregion
    /// <summary>
    /// Called when the ViewModel is being disposed.
    /// Performs audio-specific cleanup operations.
    /// </summary>
    protected virtual void OnDisposing()
    {
        try
        {
            // Unsubscribe from audio service events to prevent memory leaks
            if (_audioService != null)
            {
                _audioService.AudioStarted -= OnAudioStarted;
                _audioService.AudioStopped -= OnAudioStopped;
                _audioService.AudioError -= OnAudioError;
            }

            // Stop any playing audio from this ViewModel
            if (_audioService != null && IsPlayingAudio)
            {
                // Use synchronous stopping since we are in cleanup
                Task.Run(async () =>
                {
                    try
                    {
                        await StopAllAudioAsync(50); // Quick fade-out for cleanup
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error stopping audio during ViewModel disposal");
                    }
                });
            }

            _logger.LogDebug("AudioAwareBaseViewModel cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during AudioAwareBaseViewModel cleanup");
        }
    }
}