using EduPlayKids.Application.Models.Audio;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Configuration settings for the audio service.
/// Provides child-friendly defaults and parental control options for educational audio.
/// </summary>
public class AudioConfiguration
{
    /// <summary>
    /// Gets or sets the maximum cache size in bytes.
    /// </summary>
    public long MaxCacheSizeBytes { get; set; } = 50 * 1024 * 1024; // 50MB default

    /// <summary>
    /// Gets or sets the default audio language.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets whether audio should respect device silent mode.
    /// </summary>
    public bool RespectSilentMode { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of concurrent audio players.
    /// </summary>
    public int MaxConcurrentPlayers { get; set; } = 5;

    /// <summary>
    /// Gets or sets the default fade transition duration in milliseconds.
    /// </summary>
    public int DefaultFadeDurationMs { get; set; } = 200;

    /// <summary>
    /// Gets or sets volume levels for each audio type.
    /// </summary>
    public Dictionary<AudioType, float> DefaultVolumes { get; set; }

    /// <summary>
    /// Gets or sets whether to enable spatial audio effects.
    /// </summary>
    public bool EnableSpatialAudio { get; set; } = false;

    /// <summary>
    /// Gets or sets the audio buffer size for optimal performance.
    /// </summary>
    public int AudioBufferSize { get; set; } = 8192;

    /// <summary>
    /// Gets or sets whether to preload frequently used audio files.
    /// </summary>
    public bool EnableAudioPreloading { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache cleanup interval in minutes.
    /// </summary>
    public int CacheCleanupIntervalMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets child safety audio settings.
    /// </summary>
    public ChildSafetyAudioSettings ChildSafety { get; set; }

    /// <summary>
    /// Gets or sets platform-specific audio configurations.
    /// </summary>
    public Dictionary<string, object> PlatformSettings { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioConfiguration class with child-friendly defaults.
    /// </summary>
    public AudioConfiguration()
    {
        DefaultVolumes = new Dictionary<AudioType, float>
        {
            [AudioType.UIInteraction] = 0.7f,
            [AudioType.Instruction] = 0.9f,
            [AudioType.SuccessFeedback] = 0.8f,
            [AudioType.ErrorFeedback] = 0.6f,
            [AudioType.Completion] = 0.9f,
            [AudioType.BackgroundMusic] = 0.3f,
            [AudioType.Achievement] = 0.8f,
            [AudioType.Mascot] = 0.8f
        };

        ChildSafety = new ChildSafetyAudioSettings();
        PlatformSettings = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates a default configuration optimized for children aged 3-8.
    /// </summary>
    public static AudioConfiguration Default => new AudioConfiguration
    {
        MaxCacheSizeBytes = 50 * 1024 * 1024, // 50MB
        DefaultLanguage = "en",
        RespectSilentMode = true,
        MaxConcurrentPlayers = 5,
        DefaultFadeDurationMs = 200,
        EnableSpatialAudio = false,
        AudioBufferSize = 8192,
        EnableAudioPreloading = true,
        CacheCleanupIntervalMinutes = 30,
        ChildSafety = new ChildSafetyAudioSettings
        {
            MaxVolumeLevel = 0.85f,
            RequireFadeTransitions = true,
            PreventSuddenLoudSounds = true,
            MaxContinuousPlaybackMinutes = 60,
            AutoPauseOnInactivity = true,
            InactivityTimeoutMinutes = 10
        }
    };

    /// <summary>
    /// Creates a configuration for debugging and development.
    /// </summary>
    public static AudioConfiguration Debug => new AudioConfiguration
    {
        MaxCacheSizeBytes = 10 * 1024 * 1024, // 10MB for testing
        DefaultLanguage = "en",
        RespectSilentMode = false, // Always play during development
        MaxConcurrentPlayers = 10,
        DefaultFadeDurationMs = 100,
        EnableSpatialAudio = false,
        AudioBufferSize = 4096,
        EnableAudioPreloading = false, // Disable for faster testing
        CacheCleanupIntervalMinutes = 5,
        ChildSafety = new ChildSafetyAudioSettings
        {
            MaxVolumeLevel = 1.0f,
            RequireFadeTransitions = false,
            PreventSuddenLoudSounds = false,
            MaxContinuousPlaybackMinutes = 120,
            AutoPauseOnInactivity = false
        }
    };

    /// <summary>
    /// Validates the configuration and returns any issues found.
    /// </summary>
    /// <returns>Collection of validation error messages</returns>
    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (MaxCacheSizeBytes < 1024 * 1024) // Less than 1MB
        {
            errors.Add("MaxCacheSizeBytes should be at least 1MB for proper operation");
        }

        if (MaxConcurrentPlayers < 1)
        {
            errors.Add("MaxConcurrentPlayers must be at least 1");
        }

        if (DefaultFadeDurationMs < 0)
        {
            errors.Add("DefaultFadeDurationMs cannot be negative");
        }

        if (AudioBufferSize < 1024)
        {
            errors.Add("AudioBufferSize should be at least 1024 bytes");
        }

        if (CacheCleanupIntervalMinutes < 1)
        {
            errors.Add("CacheCleanupIntervalMinutes must be at least 1");
        }

        foreach (var volume in DefaultVolumes.Values)
        {
            if (volume < 0.0f || volume > 1.0f)
            {
                errors.Add("All default volumes must be between 0.0 and 1.0");
                break;
            }
        }

        // Validate child safety settings
        var childSafetyErrors = ChildSafety.Validate();
        errors.AddRange(childSafetyErrors);

        return errors;
    }

    /// <summary>
    /// Applies child safety constraints to ensure age-appropriate audio settings.
    /// </summary>
    public void ApplyChildSafetyConstraints()
    {
        // Ensure volume levels don't exceed safety limits
        var maxSafeVolume = ChildSafety.MaxVolumeLevel;
        foreach (var audioType in DefaultVolumes.Keys.ToList())
        {
            DefaultVolumes[audioType] = Math.Min(DefaultVolumes[audioType], maxSafeVolume);
        }

        // Enforce fade transitions for child safety
        if (ChildSafety.RequireFadeTransitions && DefaultFadeDurationMs < 100)
        {
            DefaultFadeDurationMs = 200;
        }

        // Limit concurrent players for simpler audio environment
        if (MaxConcurrentPlayers > 3)
        {
            MaxConcurrentPlayers = 3;
        }
    }

    /// <summary>
    /// Updates configuration for a specific age group.
    /// </summary>
    /// <param name="ageGroup">Target age group</param>
    public void OptimizeForAgeGroup(AgeGroup ageGroup)
    {
        switch (ageGroup)
        {
            case AgeGroup.PreK:
                // Younger children need simpler audio environment
                MaxConcurrentPlayers = 2;
                DefaultVolumes[AudioType.Instruction] = 0.95f;
                DefaultVolumes[AudioType.BackgroundMusic] = 0.2f;
                ChildSafety.MaxContinuousPlaybackMinutes = 30;
                ChildSafety.InactivityTimeoutMinutes = 5;
                break;

            case AgeGroup.Kindergarten:
                // Moderate complexity for 5-year-olds
                MaxConcurrentPlayers = 3;
                DefaultVolumes[AudioType.Instruction] = 0.9f;
                DefaultVolumes[AudioType.BackgroundMusic] = 0.25f;
                ChildSafety.MaxContinuousPlaybackMinutes = 45;
                ChildSafety.InactivityTimeoutMinutes = 8;
                break;

            case AgeGroup.Primary:
                // More sophisticated audio for older children
                MaxConcurrentPlayers = 4;
                DefaultVolumes[AudioType.Instruction] = 0.85f;
                DefaultVolumes[AudioType.BackgroundMusic] = 0.3f;
                ChildSafety.MaxContinuousPlaybackMinutes = 60;
                ChildSafety.InactivityTimeoutMinutes = 10;
                break;
        }
    }

    /// <summary>
    /// Creates a copy of this configuration.
    /// </summary>
    /// <returns>New AudioConfiguration instance with identical settings</returns>
    public AudioConfiguration Clone()
    {
        return new AudioConfiguration
        {
            MaxCacheSizeBytes = MaxCacheSizeBytes,
            DefaultLanguage = DefaultLanguage,
            RespectSilentMode = RespectSilentMode,
            MaxConcurrentPlayers = MaxConcurrentPlayers,
            DefaultFadeDurationMs = DefaultFadeDurationMs,
            DefaultVolumes = new Dictionary<AudioType, float>(DefaultVolumes),
            EnableSpatialAudio = EnableSpatialAudio,
            AudioBufferSize = AudioBufferSize,
            EnableAudioPreloading = EnableAudioPreloading,
            CacheCleanupIntervalMinutes = CacheCleanupIntervalMinutes,
            ChildSafety = ChildSafety.Clone(),
            PlatformSettings = new Dictionary<string, object>(PlatformSettings)
        };
    }
}

/// <summary>
/// Child safety settings for audio playback.
/// </summary>
public class ChildSafetyAudioSettings
{
    /// <summary>
    /// Gets or sets the maximum allowed volume level (0.0 to 1.0).
    /// </summary>
    public float MaxVolumeLevel { get; set; } = 0.85f;

    /// <summary>
    /// Gets or sets whether fade transitions are required for volume changes.
    /// </summary>
    public bool RequireFadeTransitions { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to prevent sudden loud sounds that might startle children.
    /// </summary>
    public bool PreventSuddenLoudSounds { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum continuous playback time in minutes.
    /// </summary>
    public int MaxContinuousPlaybackMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether to automatically pause audio after period of inactivity.
    /// </summary>
    public bool AutoPauseOnInactivity { get; set; } = true;

    /// <summary>
    /// Gets or sets the inactivity timeout in minutes before auto-pause.
    /// </summary>
    public int InactivityTimeoutMinutes { get; set; } = 10;

    /// <summary>
    /// Gets or sets whether to automatically adjust volume based on ambient noise.
    /// </summary>
    public bool AutoVolumeAdjustment { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum volume level during quiet periods.
    /// </summary>
    public float MinVolumeLevel { get; set; } = 0.1f;

    /// <summary>
    /// Gets or sets whether to enable hearing protection warnings.
    /// </summary>
    public bool EnableHearingProtection { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum recommended listening duration in minutes per day.
    /// </summary>
    public int MaxDailyListeningMinutes { get; set; } = 180; // 3 hours

    /// <summary>
    /// Validates the child safety settings.
    /// </summary>
    /// <returns>Collection of validation error messages</returns>
    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (MaxVolumeLevel < 0.0f || MaxVolumeLevel > 1.0f)
        {
            errors.Add("MaxVolumeLevel must be between 0.0 and 1.0");
        }

        if (MinVolumeLevel < 0.0f || MinVolumeLevel > MaxVolumeLevel)
        {
            errors.Add("MinVolumeLevel must be between 0.0 and MaxVolumeLevel");
        }

        if (MaxContinuousPlaybackMinutes < 1)
        {
            errors.Add("MaxContinuousPlaybackMinutes must be at least 1");
        }

        if (InactivityTimeoutMinutes < 1)
        {
            errors.Add("InactivityTimeoutMinutes must be at least 1");
        }

        if (MaxDailyListeningMinutes < 30)
        {
            errors.Add("MaxDailyListeningMinutes should be at least 30 for meaningful use");
        }

        return errors;
    }

    /// <summary>
    /// Creates a copy of these child safety settings.
    /// </summary>
    /// <returns>New ChildSafetyAudioSettings instance</returns>
    public ChildSafetyAudioSettings Clone()
    {
        return new ChildSafetyAudioSettings
        {
            MaxVolumeLevel = MaxVolumeLevel,
            RequireFadeTransitions = RequireFadeTransitions,
            PreventSuddenLoudSounds = PreventSuddenLoudSounds,
            MaxContinuousPlaybackMinutes = MaxContinuousPlaybackMinutes,
            AutoPauseOnInactivity = AutoPauseOnInactivity,
            InactivityTimeoutMinutes = InactivityTimeoutMinutes,
            AutoVolumeAdjustment = AutoVolumeAdjustment,
            MinVolumeLevel = MinVolumeLevel,
            EnableHearingProtection = EnableHearingProtection,
            MaxDailyListeningMinutes = MaxDailyListeningMinutes
        };
    }
}