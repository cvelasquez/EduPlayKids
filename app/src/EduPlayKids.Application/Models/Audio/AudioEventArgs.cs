namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Event arguments for audio playback events in the EduPlayKids application.
/// Provides detailed information about audio state changes for UI coordination and logging.
/// </summary>
public class AudioPlaybackEventArgs : EventArgs
{
    /// <summary>
    /// Gets the audio item that triggered this event.
    /// </summary>
    public AudioItem AudioItem { get; }

    /// <summary>
    /// Gets the current playback state.
    /// </summary>
    public AudioPlaybackState State { get; }

    /// <summary>
    /// Gets the timestamp when this event occurred.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets the current playback position in milliseconds.
    /// </summary>
    public int PositionMs { get; }

    /// <summary>
    /// Gets the total duration of the audio in milliseconds.
    /// </summary>
    public int DurationMs { get; }

    /// <summary>
    /// Gets additional context information about the event.
    /// </summary>
    public Dictionary<string, object>? Context { get; }

    /// <summary>
    /// Gets the unique identifier for this playback session.
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// Gets the audio ID for compatibility with specialized services.
    /// </summary>
    public string AudioId => AudioItem.Id;

    /// <summary>
    /// Initializes a new instance of the AudioPlaybackEventArgs class.
    /// </summary>
    /// <param name="audioItem">The audio item associated with this event</param>
    /// <param name="state">The current playback state</param>
    /// <param name="positionMs">Current playback position in milliseconds</param>
    /// <param name="durationMs">Total duration of the audio in milliseconds</param>
    /// <param name="sessionId">Unique identifier for this playback session</param>
    /// <param name="context">Additional context information</param>
    public AudioPlaybackEventArgs(
        AudioItem audioItem,
        AudioPlaybackState state,
        int positionMs = 0,
        int durationMs = 0,
        string? sessionId = null,
        Dictionary<string, object>? context = null)
    {
        AudioItem = audioItem ?? throw new ArgumentNullException(nameof(audioItem));
        State = state;
        Timestamp = DateTime.UtcNow;
        PositionMs = Math.Max(0, positionMs);
        DurationMs = Math.Max(0, durationMs);
        SessionId = sessionId ?? Guid.NewGuid().ToString();
        Context = context;
    }

    /// <summary>
    /// Gets the playback progress as a percentage (0.0 to 1.0).
    /// </summary>
    public double ProgressPercentage =>
        DurationMs > 0 ? Math.Clamp((double)PositionMs / DurationMs, 0.0, 1.0) : 0.0;

    /// <summary>
    /// Gets the remaining playback time in milliseconds.
    /// </summary>
    public int RemainingMs => Math.Max(0, DurationMs - PositionMs);

    /// <summary>
    /// Gets whether this audio playback has completed.
    /// </summary>
    public bool IsCompleted => State == AudioPlaybackState.Stopped && PositionMs >= DurationMs;

    /// <summary>
    /// Adds context information to this event.
    /// </summary>
    /// <param name="key">Context key</param>
    /// <param name="value">Context value</param>
    public void AddContext(string key, object value)
    {
        var context = Context ?? new Dictionary<string, object>();
        context[key] = value;
    }

    /// <summary>
    /// Gets context information by key.
    /// </summary>
    /// <typeparam name="T">Type of the context value</typeparam>
    /// <param name="key">Context key</param>
    /// <returns>Context value if found, default(T) otherwise</returns>
    public T? GetContext<T>(string key)
    {
        if (Context?.TryGetValue(key, out var value) == true && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Returns a string representation of this event for logging purposes.
    /// </summary>
    /// <returns>String description of the audio event</returns>
    public override string ToString()
    {
        var identifier = !string.IsNullOrEmpty(AudioItem.LocalizationKey)
            ? AudioItem.LocalizationKey
            : Path.GetFileName(AudioItem.FilePath);

        return $"AudioEvent[{State}]: {identifier} at {PositionMs}ms/{DurationMs}ms ({ProgressPercentage:P1})";
    }
}

/// <summary>
/// Event arguments for audio error events in the EduPlayKids application.
/// Provides detailed error information for troubleshooting and user notification.
/// </summary>
public class AudioErrorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the audio item that caused the error.
    /// </summary>
    public AudioItem? AudioItem { get; }

    /// <summary>
    /// Gets the error that occurred.
    /// </summary>
    public Exception Error { get; }

    /// <summary>
    /// Gets the error code for categorizing different types of audio errors.
    /// </summary>
    public AudioErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets the user-friendly error message suitable for display to parents.
    /// </summary>
    public string UserMessage { get; }

    /// <summary>
    /// Gets the technical error message for logging and debugging.
    /// </summary>
    public string TechnicalMessage { get; }

    /// <summary>
    /// Gets the error message.
    /// Alias for UserMessage property for backward compatibility.
    /// </summary>
    public string Message => UserMessage;

    /// <summary>
    /// Gets the error message for compatibility with specialized services.
    /// </summary>
    public string ErrorMessage => UserMessage;

    /// <summary>
    /// Gets the timestamp when this error occurred.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets whether this error is recoverable (can retry playback).
    /// </summary>
    public bool IsRecoverable { get; }

    /// <summary>
    /// Gets the suggested recovery action for this error.
    /// </summary>
    public string? RecoveryAction { get; }

    /// <summary>
    /// Gets additional error context information.
    /// </summary>
    public Dictionary<string, object>? Context { get; }

    /// <summary>
    /// Initializes a new instance of the AudioErrorEventArgs class.
    /// </summary>
    /// <param name="error">The exception that occurred</param>
    /// <param name="errorCode">Categorization code for the error</param>
    /// <param name="userMessage">User-friendly error message</param>
    /// <param name="isRecoverable">Whether the error is recoverable</param>
    /// <param name="audioItem">The audio item that caused the error (optional)</param>
    /// <param name="recoveryAction">Suggested recovery action (optional)</param>
    /// <param name="context">Additional error context (optional)</param>
    public AudioErrorEventArgs(
        Exception error,
        AudioErrorCode errorCode,
        string userMessage,
        bool isRecoverable = true,
        AudioItem? audioItem = null,
        string? recoveryAction = null,
        Dictionary<string, object>? context = null)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        ErrorCode = errorCode;
        UserMessage = userMessage ?? throw new ArgumentNullException(nameof(userMessage));
        TechnicalMessage = error.Message;
        Timestamp = DateTime.UtcNow;
        IsRecoverable = isRecoverable;
        AudioItem = audioItem;
        RecoveryAction = recoveryAction;
        Context = context;
    }

    /// <summary>
    /// Gets the file path associated with the error, if available.
    /// </summary>
    public string? FilePath => AudioItem?.FilePath;

    /// <summary>
    /// Gets the audio type associated with the error, if available.
    /// </summary>
    public AudioType? AudioType => AudioItem?.AudioType;

    /// <summary>
    /// Adds context information to this error event.
    /// </summary>
    /// <param name="key">Context key</param>
    /// <param name="value">Context value</param>
    public void AddContext(string key, object value)
    {
        var context = Context ?? new Dictionary<string, object>();
        context[key] = value;
    }

    /// <summary>
    /// Creates a child-friendly error message based on the error type.
    /// </summary>
    /// <returns>Age-appropriate error message for children</returns>
    public string GetChildFriendlyMessage()
    {
        return ErrorCode switch
        {
            AudioErrorCode.FileNotFound => "Hmm, I can't find that sound. Let's try something else!",
            AudioErrorCode.NetworkError => "The sound is taking a little break. Let's try again!",
            AudioErrorCode.PermissionDenied => "I need permission to play sounds. Let's ask a grown-up!",
            AudioErrorCode.DeviceBusy => "The sound player is busy right now. Let's wait a moment!",
            AudioErrorCode.UnsupportedFormat => "That sound is in a special format I don't understand yet.",
            AudioErrorCode.InsufficientMemory => "There's not enough space for sounds right now.",
            AudioErrorCode.HardwareError => "The sound system needs a little rest. Let's try again!",
            _ => "Something happened with the sound, but don't worry - let's keep learning!"
        };
    }

    /// <summary>
    /// Returns a string representation of this error for logging purposes.
    /// </summary>
    /// <returns>String description of the audio error</returns>
    public override string ToString()
    {
        var identifier = AudioItem != null
            ? (!string.IsNullOrEmpty(AudioItem.LocalizationKey)
                ? AudioItem.LocalizationKey
                : Path.GetFileName(AudioItem.FilePath))
            : "Unknown";

        return $"AudioError[{ErrorCode}]: {identifier} - {TechnicalMessage}";
    }
}

/// <summary>
/// Defines categories of audio errors for appropriate handling and user messaging.
/// </summary>
public enum AudioErrorCode
{
    /// <summary>
    /// Audio file could not be found at the specified path.
    /// </summary>
    FileNotFound,

    /// <summary>
    /// Audio file format is not supported by the platform.
    /// </summary>
    UnsupportedFormat,

    /// <summary>
    /// Network error when trying to load remote audio content.
    /// </summary>
    NetworkError,

    /// <summary>
    /// Insufficient permissions to access audio system or files.
    /// </summary>
    PermissionDenied,

    /// <summary>
    /// Audio device is busy or in use by another application.
    /// </summary>
    DeviceBusy,

    /// <summary>
    /// Insufficient memory to load or play the audio content.
    /// </summary>
    InsufficientMemory,

    /// <summary>
    /// Hardware-related audio error (speakers, headphones, etc.).
    /// </summary>
    HardwareError,

    /// <summary>
    /// Audio codec or decoding error.
    /// </summary>
    DecodingError,

    /// <summary>
    /// Playback was interrupted by system or other applications.
    /// </summary>
    Interrupted,

    /// <summary>
    /// Timeout occurred during audio loading or playback.
    /// </summary>
    Timeout,

    /// <summary>
    /// Configuration or settings error.
    /// </summary>
    ConfigurationError,

    /// <summary>
    /// General platform-specific audio error.
    /// </summary>
    PlatformError,

    /// <summary>
    /// Unknown or unspecified error occurred.
    /// </summary>
    Unknown
}

/// <summary>
/// Event arguments for audio volume change events.
/// </summary>
public class AudioVolumeEventArgs : EventArgs
{
    /// <summary>
    /// Gets the audio type whose volume changed.
    /// </summary>
    public AudioType AudioType { get; }

    /// <summary>
    /// Gets the new volume level (0.0 to 1.0).
    /// </summary>
    public float NewVolume { get; }

    /// <summary>
    /// Gets the previous volume level (0.0 to 1.0).
    /// </summary>
    public float PreviousVolume { get; }

    /// <summary>
    /// Gets whether this volume change was initiated by the user.
    /// </summary>
    public bool UserInitiated { get; }

    /// <summary>
    /// Gets the timestamp when this volume change occurred.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the AudioVolumeEventArgs class.
    /// </summary>
    /// <param name="audioType">The audio type whose volume changed</param>
    /// <param name="newVolume">The new volume level</param>
    /// <param name="previousVolume">The previous volume level</param>
    /// <param name="userInitiated">Whether this change was user-initiated</param>
    public AudioVolumeEventArgs(AudioType audioType, float newVolume, float previousVolume, bool userInitiated = true)
    {
        AudioType = audioType;
        NewVolume = Math.Clamp(newVolume, 0.0f, 1.0f);
        PreviousVolume = Math.Clamp(previousVolume, 0.0f, 1.0f);
        UserInitiated = userInitiated;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the volume change delta (-1.0 to 1.0).
    /// </summary>
    public float VolumeDelta => NewVolume - PreviousVolume;

    /// <summary>
    /// Gets whether the volume was increased.
    /// </summary>
    public bool VolumeIncreased => VolumeDelta > 0;

    /// <summary>
    /// Gets whether the volume was decreased.
    /// </summary>
    public bool VolumeDecreased => VolumeDelta < 0;

    /// <summary>
    /// Gets whether the volume was muted (set to 0).
    /// </summary>
    public bool WasMuted => NewVolume == 0.0f && PreviousVolume > 0.0f;

    /// <summary>
    /// Gets whether the volume was unmuted (changed from 0 to > 0).
    /// </summary>
    public bool WasUnmuted => PreviousVolume == 0.0f && NewVolume > 0.0f;
}