namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Represents an audio item with all configuration parameters for playback in the EduPlayKids application.
/// Encapsulates audio content, metadata, and playback settings for child-friendly educational audio delivery.
/// </summary>
public class AudioItem
{
    /// <summary>
    /// Gets or sets the unique identifier for this audio item.
    /// Used for tracking, caching, and event correlation.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the file path or URL to the audio content.
    /// Can be a local asset path, cached file path, or remote URL.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localization key for the audio content.
    /// Used for bilingual support and content management.
    /// </summary>
    public string? LocalizationKey { get; set; }

    /// <summary>
    /// Gets or sets the type of audio content for appropriate handling and volume control.
    /// </summary>
    public AudioType AudioType { get; set; } = AudioType.UIInteraction;
    /// <summary>
    /// Gets or sets the type of audio content (alias for AudioType for test compatibility).
    /// </summary>
    public AudioType Type
    {
        get => AudioType;
        set => AudioType = value;
    }

    /// <summary>
    /// Gets or sets the playback priority for managing multiple simultaneous audio requests.
    /// </summary>
    public AudioPriority Priority { get; set; } = AudioPriority.Normal;

    /// <summary>
    /// Gets or sets the volume level for this specific audio item (0.0 to 1.0).
    /// This is relative to the master volume and type-specific volume settings.
    /// </summary>
    public float Volume { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether this audio should loop continuously.
    /// Typically used for background music during activities.
    /// </summary>
    public bool Loop { get; set; } = false;

    /// <summary>
    /// Gets or sets the fade-in duration in milliseconds for smooth audio start.
    /// Prevents jarring audio beginnings that might startle children.
    /// </summary>
    public int FadeInDuration { get; set; } = 0;

    /// <summary>
    /// Gets or sets the fade-out duration in milliseconds for smooth audio ending.
    /// Ensures pleasant audio transitions during activities.
    /// </summary>
    public int FadeOutDuration { get; set; } = 200;

    /// <summary>
    /// Gets or sets the language code for this audio content.
    /// If null, uses the current application language setting.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the target age group for age-appropriate audio selection.
    /// Used to determine speech rate, complexity, and content appropriateness.
    /// </summary>
    public AgeGroup? TargetAgeGroup { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about the audio content.
    /// Can include duration, file size, content description, etc.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets whether this audio should interrupt currently playing audio of lower priority.
    /// </summary>
    public bool InterruptLowerPriority { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this audio should be cached for faster subsequent playback.
    /// Important for frequently used audio like UI sounds and common instructions.
    /// </summary>
    public bool CacheAfterPlayback { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum playback duration in milliseconds.
    /// Used as a safety mechanism to prevent runaway audio playback.
    /// </summary>
    public int? MaxDuration { get; set; }

    /// <summary>
    /// Gets or sets custom playback settings specific to this audio item.
    /// Allows for fine-tuned control over audio behavior.
    /// </summary>
    public AudioPlaybackSettings? PlaybackSettings { get; set; }

    /// <summary>
    /// Gets or sets the session ID for tracking audio playback across related activities.
    /// Used for analytics and session management.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this audio item was created.
    /// Used for caching, analytics, and debugging purposes.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the AudioItem class with default settings.
    /// </summary>
    public AudioItem()
    {
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the AudioItem class with specified file path and type.
    /// </summary>
    /// <param name="filePath">Path to the audio file</param>
    /// <param name="audioType">Type of audio content</param>
    public AudioItem(string filePath, AudioType audioType)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        AudioType = audioType;
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the AudioItem class with localization support.
    /// </summary>
    /// <param name="localizationKey">Localization key for bilingual support</param>
    /// <param name="audioType">Type of audio content</param>
    /// <param name="language">Optional language override</param>
    public AudioItem(string localizationKey, AudioType audioType, string? language = null)
    {
        LocalizationKey = localizationKey ?? throw new ArgumentNullException(nameof(localizationKey));
        AudioType = audioType;
        Language = language;
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates a copy of this AudioItem with specified volume override.
    /// Useful for temporary volume adjustments without modifying the original item.
    /// </summary>
    /// <param name="newVolume">New volume level (0.0 to 1.0)</param>
    /// <returns>New AudioItem instance with updated volume</returns>
    public AudioItem WithVolume(float newVolume)
    {
        var copy = Clone();
        copy.Volume = Math.Clamp(newVolume, 0.0f, 1.0f);
        return copy;
    }

    /// <summary>
    /// Creates a copy of this AudioItem with specified priority override.
    /// </summary>
    /// <param name="newPriority">New priority level</param>
    /// <returns>New AudioItem instance with updated priority</returns>
    public AudioItem WithPriority(AudioPriority newPriority)
    {
        var copy = Clone();
        copy.Priority = newPriority;
        return copy;
    }

    /// <summary>
    /// Creates a copy of this AudioItem with specified language override.
    /// </summary>
    /// <param name="newLanguage">New language code</param>
    /// <returns>New AudioItem instance with updated language</returns>
    public AudioItem WithLanguage(string newLanguage)
    {
        var copy = Clone();
        copy.Language = newLanguage;
        return copy;
    }

    /// <summary>
    /// Creates a copy of this AudioItem with looping enabled.
    /// </summary>
    /// <returns>New AudioItem instance with looping enabled</returns>
    public AudioItem WithLooping()
    {
        var copy = Clone();
        copy.Loop = true;
        return copy;
    }

    /// <summary>
    /// Creates a copy of this AudioItem with fade effects configured.
    /// </summary>
    /// <param name="fadeInMs">Fade-in duration in milliseconds</param>
    /// <param name="fadeOutMs">Fade-out duration in milliseconds</param>
    /// <returns>New AudioItem instance with fade effects</returns>
    public AudioItem WithFadeEffects(int fadeInMs, int fadeOutMs)
    {
        var copy = Clone();
        copy.FadeInDuration = Math.Max(0, fadeInMs);
        copy.FadeOutDuration = Math.Max(0, fadeOutMs);
        return copy;
    }

    /// <summary>
    /// Validates the AudioItem configuration and returns any validation errors.
    /// </summary>
    /// <returns>Collection of validation error messages, empty if valid</returns>
    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(FilePath) && string.IsNullOrEmpty(LocalizationKey))
        {
            errors.Add("Either FilePath or LocalizationKey must be specified");
        }

        if (Volume < 0.0f || Volume > 1.0f)
        {
            errors.Add("Volume must be between 0.0 and 1.0");
        }

        if (FadeInDuration < 0)
        {
            errors.Add("FadeInDuration cannot be negative");
        }

        if (FadeOutDuration < 0)
        {
            errors.Add("FadeOutDuration cannot be negative");
        }

        if (MaxDuration.HasValue && MaxDuration.Value <= 0)
        {
            errors.Add("MaxDuration must be positive if specified");
        }

        return errors;
    }

    /// <summary>
    /// Creates a deep copy of this AudioItem instance.
    /// </summary>
    /// <returns>New AudioItem instance with identical configuration</returns>
    public AudioItem Clone()
    {
        var clone = new AudioItem
        {
            Id = Id,
            FilePath = FilePath,
            LocalizationKey = LocalizationKey,
            AudioType = AudioType,
            Priority = Priority,
            Volume = Volume,
            Loop = Loop,
            FadeInDuration = FadeInDuration,
            FadeOutDuration = FadeOutDuration,
            Language = Language,
            TargetAgeGroup = TargetAgeGroup,
            InterruptLowerPriority = InterruptLowerPriority,
            CacheAfterPlayback = CacheAfterPlayback,
            MaxDuration = MaxDuration,
            PlaybackSettings = PlaybackSettings?.Clone(),
            SessionId = SessionId,
            CreatedAt = CreatedAt
        };

        if (Metadata != null)
        {
            clone.Metadata = new Dictionary<string, object>(Metadata);
        }

        return clone;
    }

    /// <summary>
    /// Returns a string representation of this AudioItem for debugging purposes.
    /// </summary>
    /// <returns>String description of the audio item</returns>
    public override string ToString()
    {
        var identifier = !string.IsNullOrEmpty(LocalizationKey) ? LocalizationKey : FilePath;
        return $"AudioItem[{AudioType}]: {identifier} (Priority: {Priority}, Volume: {Volume:P0})";
    }
}