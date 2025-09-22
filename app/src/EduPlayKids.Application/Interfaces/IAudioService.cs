using EduPlayKids.Application.Models.Audio;

namespace EduPlayKids.Application.Interfaces;

/// <summary>
/// Service interface for managing audio playback in the EduPlayKids application.
/// Provides comprehensive audio functionality including bilingual support, child-friendly feedback,
/// and parental controls optimized for educational content targeting children aged 3-8.
/// </summary>
public interface IAudioService
{
    #region Audio Playback Control

    /// <summary>
    /// Plays an audio file with specified configuration.
    /// Automatically manages volume levels appropriate for children and respects parental settings.
    /// </summary>
    /// <param name="audioItem">The audio item to play with all configuration parameters</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if audio started playing successfully, false otherwise</returns>
    Task<bool> PlayAudioAsync(AudioItem audioItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays audio by file path with automatic type detection and language preference.
    /// Uses current user language settings and applies child-safe volume controls.
    /// </summary>
    /// <param name="filePath">Path to the audio file (local asset or cached file)</param>
    /// <param name="audioType">Type of audio content for appropriate volume and priority handling</param>
    /// <param name="priority">Playback priority for managing multiple simultaneous audio requests</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if audio started playing successfully, false otherwise</returns>
    Task<bool> PlayAudioAsync(string filePath, AudioType audioType, AudioPriority priority = AudioPriority.Normal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops currently playing audio with smooth fade-out to prevent jarring interruptions.
    /// Respects child-friendly audio transitions to maintain pleasant user experience.
    /// </summary>
    /// <param name="audioType">Optional audio type filter to stop specific categories only</param>
    /// <param name="fadeOutDuration">Duration of fade-out in milliseconds (default: 200ms)</param>
    /// <returns>Task representing the async operation</returns>
    Task StopAudioAsync(AudioType? audioType = null, int fadeOutDuration = 200);

    /// <summary>
    /// Pauses currently playing audio with ability to resume from the same position.
    /// Maintains playback state for educational content continuity.
    /// </summary>
    /// <param name="audioType">Optional audio type filter to pause specific categories only</param>
    /// <returns>Task representing the async operation</returns>
    Task PauseAudioAsync(AudioType? audioType = null);

    /// <summary>
    /// Resumes previously paused audio from the last position.
    /// Ensures smooth continuation of educational content without disruption.
    /// </summary>
    /// <param name="audioType">Optional audio type filter to resume specific categories only</param>
    /// <returns>Task representing the async operation</returns>
    Task ResumeAudioAsync(AudioType? audioType = null);

    #endregion

    #region Bilingual Audio Support

    /// <summary>
    /// Plays localized audio content based on current language settings.
    /// Automatically selects Spanish or English version of the content.
    /// </summary>
    /// <param name="audioKey">Localization key for the audio content</param>
    /// <param name="audioType">Type of audio content for appropriate handling</param>
    /// <param name="language">Optional language override (uses app setting if not specified)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if localized audio was found and started playing, false otherwise</returns>
    Task<bool> PlayLocalizedAudioAsync(string audioKey, AudioType audioType, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the path to a localized audio file based on current language settings.
    /// Used for preloading or validating audio availability before playback.
    /// </summary>
    /// <param name="audioKey">Localization key for the audio content</param>
    /// <param name="language">Optional language override (uses app setting if not specified)</param>
    /// <returns>Path to the localized audio file, or null if not found</returns>
    string? GetLocalizedAudioPath(string audioKey, string? language = null);

    /// <summary>
    /// Sets the preferred language for audio playback.
    /// Updates all subsequent audio playback to use the specified language.
    /// </summary>
    /// <param name="languageCode">ISO language code (e.g., "en", "es")</param>
    /// <returns>True if language was set successfully, false if unsupported</returns>
    Task<bool> SetAudioLanguageAsync(string languageCode);

    /// <summary>
    /// Gets the currently active audio language setting.
    /// </summary>
    /// <returns>ISO language code for current audio language</returns>
    string GetCurrentAudioLanguage();

    #endregion

    #region Child-Friendly Audio Feedback

    /// <summary>
    /// Plays encouraging feedback sound for correct answers.
    /// Uses age-appropriate positive reinforcement audio with celebration elements.
    /// </summary>
    /// <param name="intensity">Intensity level of celebration (Soft, Medium, High)</param>
    /// <param name="childAge">Child's age for age-appropriate sound selection</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task PlaySuccessFeedbackAsync(FeedbackIntensity intensity = FeedbackIntensity.Medium, int? childAge = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays gentle correction sound for incorrect answers.
    /// Uses supportive, non-discouraging audio to maintain child motivation.
    /// </summary>
    /// <param name="encouragement">Level of encouragement in the feedback</param>
    /// <param name="childAge">Child's age for age-appropriate sound selection</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task PlayErrorFeedbackAsync(FeedbackIntensity encouragement = FeedbackIntensity.Soft, int? childAge = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays activity completion celebration audio.
    /// Provides satisfying conclusion to learning activities with achievement recognition.
    /// </summary>
    /// <param name="starRating">Number of stars earned (1-3) for appropriate celebration level</param>
    /// <param name="activityType">Type of activity completed for context-appropriate audio</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task PlayActivityCompletionAsync(int starRating, string? activityType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays UI interaction sound for button presses and navigation.
    /// Provides immediate audio feedback for touch interactions suitable for children.
    /// </summary>
    /// <param name="interactionType">Type of UI interaction for appropriate sound selection</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task PlayUIFeedbackAsync(UIInteractionType interactionType, CancellationToken cancellationToken = default);

    #endregion

    #region Educational Content Audio

    /// <summary>
    /// Plays activity instructions with clear, child-friendly narration.
    /// Designed for non-reading children with appropriate pacing and clarity.
    /// </summary>
    /// <param name="instructionKey">Localization key for the instruction content</param>
    /// <param name="childAge">Child's age for appropriate speech rate and complexity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if instruction audio was played successfully, false otherwise</returns>
    Task<bool> PlayInstructionAsync(string instructionKey, int? childAge = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays question text audio for educational activities.
    /// Ensures clear delivery of question content for young learners.
    /// </summary>
    /// <param name="questionText">The question text to be narrated</param>
    /// <param name="questionType">Type of question for appropriate audio styling</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if question audio was played successfully, false otherwise</returns>
    Task<bool> PlayQuestionAudioAsync(string questionText, string? questionType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays background music during educational activities.
    /// Provides ambient audio that enhances learning without distraction.
    /// </summary>
    /// <param name="activityType">Type of activity for appropriate background music selection</param>
    /// <param name="volume">Volume level (0.0 to 1.0) relative to master volume settings</param>
    /// <param name="loop">Whether to loop the background music continuously</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if background music started successfully, false otherwise</returns>
    Task<bool> PlayBackgroundMusicAsync(string activityType, float volume = 0.3f, bool loop = true, CancellationToken cancellationToken = default);

    #endregion

    #region Audio State Management

    /// <summary>
    /// Gets the current playback state for a specific audio type.
    /// Used for UI state management and preventing conflicting audio operations.
    /// </summary>
    /// <param name="audioType">Type of audio to check status for</param>
    /// <returns>Current playback state of the specified audio type</returns>
    AudioPlaybackState GetPlaybackState(AudioType audioType);

    /// <summary>
    /// Checks if any audio is currently playing.
    /// Useful for managing audio conflicts and user experience optimization.
    /// </summary>
    /// <returns>True if any audio is currently playing, false otherwise</returns>
    bool IsAnyAudioPlaying();

    /// <summary>
    /// Gets the current volume level for a specific audio type.
    /// Returns the effective volume considering both type-specific and master volume settings.
    /// </summary>
    /// <param name="audioType">Type of audio to get volume for</param>
    /// <returns>Current volume level (0.0 to 1.0)</returns>
    float GetVolumeLevel(AudioType audioType);

    /// <summary>
    /// Sets the volume level for a specific audio type.
    /// Immediately applies to currently playing audio of that type.
    /// </summary>
    /// <param name="audioType">Type of audio to set volume for</param>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <returns>Task representing the async operation</returns>
    Task SetVolumeLevelAsync(AudioType audioType, float volume);

    #endregion

    #region Audio Events and Notifications

    /// <summary>
    /// Event raised when audio playback starts.
    /// Provides information about the audio being played for UI state management.
    /// </summary>
    event EventHandler<AudioPlaybackEventArgs>? AudioStarted;

    /// <summary>
    /// Event raised when audio playback stops or completes.
    /// Allows for cleanup operations and UI state updates.
    /// </summary>
    event EventHandler<AudioPlaybackEventArgs>? AudioStopped;

    /// <summary>
    /// Event raised when audio playback is paused.
    /// Used for coordinating UI states and user experience flows.
    /// </summary>
    event EventHandler<AudioPlaybackEventArgs>? AudioPaused;

    /// <summary>
    /// Event raised when audio playback is resumed.
    /// Enables proper UI synchronization and state management.
    /// </summary>
    event EventHandler<AudioPlaybackEventArgs>? AudioResumed;

    /// <summary>
    /// Event raised when an audio error occurs.
    /// Provides error information for logging and user notification.
    /// </summary>
    event EventHandler<AudioErrorEventArgs>? AudioError;

    #endregion

    #region Resource Management

    /// <summary>
    /// Preloads audio files for immediate playback during activities.
    /// Reduces latency and ensures smooth audio experience during learning.
    /// </summary>
    /// <param name="audioKeys">Collection of audio keys to preload</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary indicating which audio files were successfully preloaded</returns>
    Task<Dictionary<string, bool>> PreloadAudioAsync(IEnumerable<string> audioKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears audio cache to free up memory.
    /// Maintains frequently used audio files while removing unused cached content.
    /// </summary>
    /// <param name="keepRecent">Whether to keep recently used audio files in cache</param>
    /// <returns>Amount of memory freed in bytes</returns>
    Task<long> ClearAudioCacheAsync(bool keepRecent = true);

    /// <summary>
    /// Gets information about current audio cache usage.
    /// Useful for monitoring memory usage and optimization.
    /// </summary>
    /// <returns>Audio cache statistics including size and file count</returns>
    AudioCacheInfo GetCacheInfo();

    #endregion

    #region Platform-Specific Operations

    /// <summary>
    /// Handles audio interruptions from phone calls, notifications, or other apps.
    /// Automatically pauses educational content and resumes when appropriate.
    /// </summary>
    /// <param name="interruption">Type of audio interruption</param>
    /// <returns>Task representing the async operation</returns>
    Task HandleAudioInterruptionAsync(AudioInterruption interruption);

    /// <summary>
    /// Updates audio session configuration for optimal child learning experience.
    /// Configures platform-specific audio settings for educational content.
    /// </summary>
    /// <param name="sessionType">Type of audio session for appropriate configuration</param>
    /// <returns>True if audio session was configured successfully, false otherwise</returns>
    Task<bool> ConfigureAudioSessionAsync(AudioSessionType sessionType);

    /// <summary>
    /// Checks if audio permissions are granted and requests them if needed.
    /// Ensures proper audio functionality while respecting platform privacy requirements.
    /// </summary>
    /// <returns>True if audio permissions are available, false otherwise</returns>
    Task<bool> EnsureAudioPermissionsAsync();

    #endregion
}