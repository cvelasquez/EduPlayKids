namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Represents advanced playback settings for fine-tuned audio control in educational applications.
/// Provides platform-specific configuration options for optimal child learning experience.
/// </summary>
public class AudioPlaybackSettings
{
    /// <summary>
    /// Gets or sets the playback rate (speed) for audio content.
    /// 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.
    /// Useful for adjusting speech rate for different age groups.
    /// </summary>
    public float PlaybackRate { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the audio pitch adjustment.
    /// 1.0 = normal pitch, values above/below adjust pitch higher/lower.
    /// Can be used to make voices more appealing to children.
    /// </summary>
    public float Pitch { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether to preserve pitch when changing playback rate.
    /// True maintains natural voice sound when speeding up/down speech.
    /// </summary>
    public bool PreservePitchOnRateChange { get; set; } = true;

    /// <summary>
    /// Gets or sets the start position in milliseconds for audio playback.
    /// Allows playing specific segments of longer audio files.
    /// </summary>
    public int StartPositionMs { get; set; } = 0;

    /// <summary>
    /// Gets or sets the end position in milliseconds for audio playback.
    /// If null, plays to the end of the audio file.
    /// </summary>
    public int? EndPositionMs { get; set; }

    /// <summary>
    /// Gets or sets whether to normalize audio volume across different files.
    /// Ensures consistent volume levels for better user experience.
    /// </summary>
    public bool NormalizeVolume { get; set; } = true;

    /// <summary>
    /// Gets or sets the audio ducking behavior when higher priority audio plays.
    /// Defines how background audio should be reduced when important audio starts.
    /// </summary>
    public AudioDuckingBehavior DuckingBehavior { get; set; } = AudioDuckingBehavior.FadeOut;

    /// <summary>
    /// Gets or sets the ducking volume level (0.0 to 1.0) when other audio has priority.
    /// </summary>
    public float DuckingVolume { get; set; } = 0.1f;

    /// <summary>
    /// Gets or sets the duration in milliseconds for volume ducking transitions.
    /// </summary>
    public int DuckingTransitionMs { get; set; } = 300;

    /// <summary>
    /// Gets or sets whether this audio should respect the device's silent/mute mode.
    /// Educational content might need to play even in silent mode for accessibility.
    /// </summary>
    public bool RespectSilentMode { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable spatial audio effects if supported by the device.
    /// Can enhance immersive learning experiences for compatible content.
    /// </summary>
    public bool EnableSpatialAudio { get; set; } = false;

    /// <summary>
    /// Gets or sets the preferred audio output route (speaker, headphones, etc.).
    /// Allows optimization for different listening scenarios.
    /// </summary>
    public AudioOutputRoute? PreferredOutputRoute { get; set; }

    /// <summary>
    /// Gets or sets audio enhancement settings for clearer speech delivery.
    /// </summary>
    public SpeechEnhancementSettings? SpeechEnhancement { get; set; }

    /// <summary>
    /// Gets or sets custom platform-specific audio settings.
    /// Allows for iOS/Android specific optimizations.
    /// </summary>
    public Dictionary<string, object>? PlatformSettings { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of simultaneous playback attempts.
    /// Prevents audio conflicts during rapid user interactions.
    /// </summary>
    public int MaxConcurrentPlayback { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether to preload the next audio in a sequence.
    /// Reduces latency during educational activity sequences.
    /// </summary>
    public bool PreloadNext { get; set; } = false;

    /// <summary>
    /// Gets or sets the buffer size for audio streaming in bytes.
    /// Can be tuned for performance on different device capabilities.
    /// </summary>
    public int? BufferSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioPlaybackSettings class with default values.
    /// </summary>
    public AudioPlaybackSettings()
    {
        PlatformSettings = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates settings optimized for educational speech content.
    /// </summary>
    /// <param name="ageGroup">Target age group for appropriate speech rate</param>
    /// <returns>AudioPlaybackSettings configured for speech content</returns>
    public static AudioPlaybackSettings ForSpeechContent(AgeGroup ageGroup)
    {
        var settings = new AudioPlaybackSettings
        {
            NormalizeVolume = true,
            PreservePitchOnRateChange = true,
            RespectSilentMode = false, // Educational speech should play even in silent mode
            DuckingBehavior = AudioDuckingBehavior.Pause,
            SpeechEnhancement = new SpeechEnhancementSettings
            {
                EnableClarityEnhancement = true,
                ReduceBackgroundNoise = true,
                EnhanceVoiceFrequencies = true
            }
        };

        // Adjust speech rate based on age group
        settings.PlaybackRate = ageGroup switch
        {
            AgeGroup.PreK => 0.8f, // Slower for younger children
            AgeGroup.Kindergarten => 0.9f, // Slightly slower
            AgeGroup.Primary => 1.0f, // Normal rate
            _ => 1.0f
        };

        return settings;
    }

    /// <summary>
    /// Creates settings optimized for background music during activities.
    /// </summary>
    /// <returns>AudioPlaybackSettings configured for background music</returns>
    public static AudioPlaybackSettings ForBackgroundMusic()
    {
        return new AudioPlaybackSettings
        {
            DuckingBehavior = AudioDuckingBehavior.Duck,
            DuckingVolume = 0.2f,
            DuckingTransitionMs = 500,
            RespectSilentMode = true,
            NormalizeVolume = true,
            PreloadNext = true
        };
    }

    /// <summary>
    /// Creates settings optimized for UI interaction sounds.
    /// </summary>
    /// <returns>AudioPlaybackSettings configured for UI feedback</returns>
    public static AudioPlaybackSettings ForUIFeedback()
    {
        return new AudioPlaybackSettings
        {
            DuckingBehavior = AudioDuckingBehavior.None,
            RespectSilentMode = true,
            NormalizeVolume = true,
            MaxConcurrentPlayback = 3, // Allow multiple UI sounds
            BufferSize = 4096 // Small buffer for low latency
        };
    }

    /// <summary>
    /// Validates the settings and returns any configuration errors.
    /// </summary>
    /// <returns>Collection of validation error messages</returns>
    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (PlaybackRate <= 0 || PlaybackRate > 3.0f)
        {
            errors.Add("PlaybackRate must be between 0.1 and 3.0");
        }

        if (Pitch <= 0 || Pitch > 2.0f)
        {
            errors.Add("Pitch must be between 0.1 and 2.0");
        }

        if (StartPositionMs < 0)
        {
            errors.Add("StartPositionMs cannot be negative");
        }

        if (EndPositionMs.HasValue && EndPositionMs.Value <= StartPositionMs)
        {
            errors.Add("EndPositionMs must be greater than StartPositionMs");
        }

        if (DuckingVolume < 0.0f || DuckingVolume > 1.0f)
        {
            errors.Add("DuckingVolume must be between 0.0 and 1.0");
        }

        if (DuckingTransitionMs < 0)
        {
            errors.Add("DuckingTransitionMs cannot be negative");
        }

        if (MaxConcurrentPlayback < 1)
        {
            errors.Add("MaxConcurrentPlayback must be at least 1");
        }

        return errors;
    }

    /// <summary>
    /// Creates a deep copy of these settings.
    /// </summary>
    /// <returns>New AudioPlaybackSettings instance with identical configuration</returns>
    public AudioPlaybackSettings Clone()
    {
        var clone = new AudioPlaybackSettings
        {
            PlaybackRate = PlaybackRate,
            Pitch = Pitch,
            PreservePitchOnRateChange = PreservePitchOnRateChange,
            StartPositionMs = StartPositionMs,
            EndPositionMs = EndPositionMs,
            NormalizeVolume = NormalizeVolume,
            DuckingBehavior = DuckingBehavior,
            DuckingVolume = DuckingVolume,
            DuckingTransitionMs = DuckingTransitionMs,
            RespectSilentMode = RespectSilentMode,
            EnableSpatialAudio = EnableSpatialAudio,
            PreferredOutputRoute = PreferredOutputRoute,
            SpeechEnhancement = SpeechEnhancement?.Clone(),
            MaxConcurrentPlayback = MaxConcurrentPlayback,
            PreloadNext = PreloadNext,
            BufferSize = BufferSize
        };

        if (PlatformSettings != null)
        {
            clone.PlatformSettings = new Dictionary<string, object>(PlatformSettings);
        }

        return clone;
    }
}

/// <summary>
/// Defines how audio should behave when higher priority audio starts playing.
/// </summary>
public enum AudioDuckingBehavior
{
    /// <summary>
    /// No ducking - audio continues at current volume.
    /// </summary>
    None,

    /// <summary>
    /// Reduce volume but continue playing.
    /// </summary>
    Duck,

    /// <summary>
    /// Fade out and pause until higher priority audio finishes.
    /// </summary>
    Pause,

    /// <summary>
    /// Stop playing completely.
    /// </summary>
    Stop,

    /// <summary>
    /// Fade out gradually over the transition period.
    /// </summary>
    FadeOut
}

/// <summary>
/// Defines preferred audio output routes for different scenarios.
/// </summary>
public enum AudioOutputRoute
{
    /// <summary>
    /// Use system default output device.
    /// </summary>
    Default,

    /// <summary>
    /// Prefer built-in speakers for group learning.
    /// </summary>
    Speaker,

    /// <summary>
    /// Prefer headphones for individual learning.
    /// </summary>
    Headphones,

    /// <summary>
    /// Prefer Bluetooth audio devices if available.
    /// </summary>
    Bluetooth,

    /// <summary>
    /// Use earpiece for quiet environments.
    /// </summary>
    Earpiece
}

/// <summary>
/// Settings for enhancing speech clarity and comprehension.
/// </summary>
public class SpeechEnhancementSettings
{
    /// <summary>
    /// Gets or sets whether to enable general speech clarity enhancement.
    /// </summary>
    public bool EnableClarityEnhancement { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to reduce background noise in speech audio.
    /// </summary>
    public bool ReduceBackgroundNoise { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enhance voice frequency ranges for better comprehension.
    /// </summary>
    public bool EnhanceVoiceFrequencies { get; set; } = false;

    /// <summary>
    /// Gets or sets the level of speech compression to apply (0.0 to 1.0).
    /// Helps maintain consistent volume levels during speech.
    /// </summary>
    public float CompressionLevel { get; set; } = 0.3f;

    /// <summary>
    /// Gets or sets whether to add subtle echo reduction for clearer speech.
    /// </summary>
    public bool EchoReduction { get; set; } = false;

    /// <summary>
    /// Creates a copy of these speech enhancement settings.
    /// </summary>
    /// <returns>New SpeechEnhancementSettings instance</returns>
    public SpeechEnhancementSettings Clone()
    {
        return new SpeechEnhancementSettings
        {
            EnableClarityEnhancement = EnableClarityEnhancement,
            ReduceBackgroundNoise = ReduceBackgroundNoise,
            EnhanceVoiceFrequencies = EnhanceVoiceFrequencies,
            CompressionLevel = CompressionLevel,
            EchoReduction = EchoReduction
        };
    }
}