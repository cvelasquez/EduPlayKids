using EduPlayKids.Application.Models.Audio;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Interface for platform-specific audio player implementations.
/// Provides low-level audio playback control for individual audio items.
/// </summary>
public interface IAudioPlayer : IDisposable
{
    /// <summary>
    /// Gets whether the audio is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets whether the audio is currently paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Gets the current playback position in milliseconds.
    /// </summary>
    int CurrentPositionMs { get; }

    /// <summary>
    /// Gets the total duration of the audio in milliseconds.
    /// </summary>
    int DurationMs { get; }

    /// <summary>
    /// Gets or sets the current volume level (0.0 to 1.0).
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Event raised when playback completes naturally.
    /// </summary>
    event EventHandler? PlaybackCompleted;

    /// <summary>
    /// Event raised when an error occurs during playback.
    /// </summary>
    event EventHandler<AudioPlayerErrorEventArgs>? PlaybackError;

    /// <summary>
    /// Starts audio playback.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if playback started successfully, false otherwise</returns>
    Task<bool> PlayAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops audio playback with optional fade-out.
    /// </summary>
    /// <param name="fadeOutDurationMs">Fade-out duration in milliseconds</param>
    /// <returns>Task representing the async operation</returns>
    Task StopAsync(int fadeOutDurationMs = 0);

    /// <summary>
    /// Pauses audio playback.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task PauseAsync();

    /// <summary>
    /// Resumes paused audio playback.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task ResumeAsync();

    /// <summary>
    /// Sets the playback volume.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <returns>Task representing the async operation</returns>
    Task SetVolumeAsync(float volume);

    /// <summary>
    /// Seeks to a specific position in the audio.
    /// </summary>
    /// <param name="positionMs">Position in milliseconds</param>
    /// <returns>Task representing the async operation</returns>
    Task SeekToAsync(int positionMs);

    /// <summary>
    /// Prepares the audio for playback (loads and buffers).
    /// </summary>
    /// <returns>True if preparation was successful, false otherwise</returns>
    Task<bool> PrepareAsync();
}

/// <summary>
/// Event arguments for audio player error events.
/// </summary>
public class AudioPlayerErrorEventArgs : EventArgs
{
    public Exception Error { get; }
    public string Message { get; }

    public AudioPlayerErrorEventArgs(Exception error, string message)
    {
        Error = error;
        Message = message;
    }
}