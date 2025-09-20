using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents application settings and preferences for the EduPlayKids application.
/// Manages configuration for language preferences, audio settings, parental controls,
/// and child-specific learning customizations.
/// </summary>
public class Settings : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the user these settings belong to.
    /// Each user (parent) has their own settings configuration.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the application language preference.
    /// Primary language for UI and content delivery.
    /// Values: "en" (English), "es" (Spanish)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string AppLanguage { get; set; } = "es";

    /// <summary>
    /// Gets or sets the audio instruction language preference.
    /// May differ from app language for bilingual learning.
    /// Values: "en" (English), "es" (Spanish), "both" (auto-detect)
    /// </summary>
    [StringLength(10)]
    public string AudioLanguage { get; set; } = "es";

    /// <summary>
    /// Gets or sets the master volume level for all audio.
    /// Range: 0-100, where 0 is muted and 100 is maximum volume.
    /// </summary>
    [Range(0, 100)]
    public int MasterVolume { get; set; } = 80;

    /// <summary>
    /// Gets or sets the volume level for background music.
    /// Range: 0-100, relative to master volume.
    /// </summary>
    [Range(0, 100)]
    public int MusicVolume { get; set; } = 60;

    /// <summary>
    /// Gets or sets the volume level for sound effects.
    /// Range: 0-100, relative to master volume.
    /// </summary>
    [Range(0, 100)]
    public int SoundEffectsVolume { get; set; } = 80;

    /// <summary>
    /// Gets or sets the volume level for voice instructions.
    /// Range: 0-100, relative to master volume.
    /// Critical for non-readers aged 3-5.
    /// </summary>
    [Range(0, 100)]
    public int VoiceVolume { get; set; } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether audio instructions are enabled.
    /// Essential accessibility feature for young children.
    /// </summary>
    public bool AudioInstructionsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether background music is enabled.
    /// Some children focus better without background music.
    /// </summary>
    public bool BackgroundMusicEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether sound effects are enabled.
    /// Provides audio feedback for interactions and achievements.
    /// </summary>
    public bool SoundEffectsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether haptic feedback is enabled.
    /// Touch vibration for supported devices.
    /// </summary>
    public bool HapticFeedbackEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation speed preference.
    /// Values: Slow, Normal, Fast, Disabled
    /// Accommodates different attention spans and needs.
    /// </summary>
    [StringLength(20)]
    public string AnimationSpeed { get; set; } = "Normal";

    /// <summary>
    /// Gets or sets a value indicating whether parental controls are enabled.
    /// Master switch for all parental control features.
    /// </summary>
    public bool ParentalControlsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the daily screen time limit in minutes.
    /// 0 means no limit. Promotes healthy usage habits.
    /// </summary>
    [Range(0, 480)] // Max 8 hours
    public int DailyTimeLimitMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether bedtime restrictions are enabled.
    /// Prevents app usage during specified sleep hours.
    /// </summary>
    public bool BedtimeRestrictionsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the bedtime start hour (24-hour format).
    /// App becomes inaccessible from this time.
    /// </summary>
    [Range(0, 23)]
    public int BedtimeStartHour { get; set; } = 20; // 8 PM

    /// <summary>
    /// Gets or sets the bedtime end hour (24-hour format).
    /// App becomes accessible again from this time.
    /// </summary>
    [Range(0, 23)]
    public int BedtimeEndHour { get; set; } = 7; // 7 AM

    /// <summary>
    /// Gets or sets a value indicating whether crown challenges are enabled.
    /// Advanced content for high-performing children.
    /// </summary>
    public bool CrownChallengesEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default difficulty preference for new activities.
    /// Values: Easy, Medium, Hard, Adaptive
    /// </summary>
    [StringLength(20)]
    public string DefaultDifficulty { get; set; } = "Adaptive";

    /// <summary>
    /// Gets or sets a value indicating whether adaptive learning is enabled.
    /// Automatically adjusts difficulty based on performance.
    /// </summary>
    public bool AdaptiveLearningEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether progress reports are enabled.
    /// Weekly/monthly progress summaries for parents.
    /// </summary>
    public bool ProgressReportsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the frequency of progress reports.
    /// Values: Daily, Weekly, Monthly, Disabled
    /// </summary>
    [StringLength(20)]
    public string ProgressReportFrequency { get; set; } = "Weekly";

    /// <summary>
    /// Gets or sets a value indicating whether achievement notifications are enabled.
    /// Push notifications for earned achievements and milestones.
    /// </summary>
    public bool AchievementNotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether reminder notifications are enabled.
    /// Gentle reminders to continue learning.
    /// </summary>
    public bool ReminderNotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timezone for accurate time-based features.
    /// Important for bedtime restrictions and usage analytics.
    /// </summary>
    [StringLength(50)]
    public string TimeZone { get; set; } = "America/Los_Angeles";

    /// <summary>
    /// Gets or sets the date format preference.
    /// Values: MM/DD/YYYY, DD/MM/YYYY, YYYY-MM-DD
    /// </summary>
    [StringLength(20)]
    public string DateFormat { get; set; } = "MM/DD/YYYY";

    /// <summary>
    /// Gets or sets accessibility settings as JSON.
    /// Flexible structure for special needs accommodations:
    /// - High contrast mode
    /// - Large text size
    /// - Reduced motion
    /// - Screen reader compatibility
    /// - Color blind support
    /// </summary>
    public string? AccessibilitySettings { get; set; }

    /// <summary>
    /// Gets or sets data collection preferences as JSON.
    /// COPPA-compliant privacy settings:
    /// - Analytics collection level
    /// - Performance monitoring
    /// - Error reporting
    /// - Usage pattern tracking
    /// </summary>
    public string? DataCollectionPreferences { get; set; }

    /// <summary>
    /// Gets or sets child safety settings as JSON.
    /// Enhanced safety features:
    /// - Exit confirmation requirements
    /// - PIN protection for settings
    /// - Safe content filters
    /// - Emergency contact information
    /// </summary>
    public string? ChildSafetySettings { get; set; }

    /// <summary>
    /// Gets or sets custom learning goals as JSON.
    /// Parent-defined objectives and milestones:
    /// - Subject focus areas
    /// - Skill development targets
    /// - Time-based goals
    /// - Achievement aspirations
    /// </summary>
    public string? CustomLearningGoals { get; set; }

    /// <summary>
    /// Gets or sets notification preferences as JSON.
    /// Detailed notification configuration:
    /// - Notification types enabled
    /// - Delivery times
    /// - Frequency limits
    /// - Priority levels
    /// </summary>
    public string? NotificationPreferences { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether these settings should be synced.
    /// Used for future cloud synchronization features.
    /// </summary>
    public bool NeedsSync { get; set; } = false;

    /// <summary>
    /// Gets or sets the version of the settings schema.
    /// Used for settings migration and compatibility.
    /// </summary>
    public int SettingsVersion { get; set; } = 1;

    /// <summary>
    /// Navigation property: The user these settings belong to.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the Settings class.
    /// Sets default values optimized for children aged 3-8.
    /// </summary>
    public Settings()
    {
        AppLanguage = "es";
        AudioLanguage = "es";
        MasterVolume = 80;
        MusicVolume = 60;
        SoundEffectsVolume = 80;
        VoiceVolume = 100;
        AudioInstructionsEnabled = true;
        BackgroundMusicEnabled = true;
        SoundEffectsEnabled = true;
        HapticFeedbackEnabled = true;
        AnimationSpeed = "Normal";
        ParentalControlsEnabled = true;
        DailyTimeLimitMinutes = 60;
        BedtimeRestrictionsEnabled = false;
        BedtimeStartHour = 20;
        BedtimeEndHour = 7;
        CrownChallengesEnabled = true;
        DefaultDifficulty = "Adaptive";
        AdaptiveLearningEnabled = true;
        ProgressReportsEnabled = true;
        ProgressReportFrequency = "Weekly";
        AchievementNotificationsEnabled = true;
        ReminderNotificationsEnabled = true;
        TimeZone = "America/Los_Angeles";
        DateFormat = "MM/DD/YYYY";
        NeedsSync = false;
        SettingsVersion = 1;
    }

    /// <summary>
    /// Determines if the app should be accessible based on bedtime restrictions.
    /// </summary>
    /// <returns>True if app is accessible, false if in bedtime restriction period.</returns>
    public bool IsAppAccessible()
    {
        if (!BedtimeRestrictionsEnabled) return true;

        var currentHour = DateTime.Now.Hour;

        // Handle overnight bedtime (e.g., 20:00 - 7:00)
        if (BedtimeStartHour > BedtimeEndHour)
        {
            return currentHour < BedtimeStartHour && currentHour >= BedtimeEndHour;
        }
        // Handle same-day bedtime (unusual but possible)
        else
        {
            return currentHour < BedtimeStartHour || currentHour >= BedtimeEndHour;
        }
    }

    /// <summary>
    /// Gets the effective audio volume for a specific audio type.
    /// </summary>
    /// <param name="audioType">Type of audio: "music", "effects", "voice"</param>
    /// <returns>Effective volume (0-100) considering master volume.</returns>
    public int GetEffectiveVolume(string audioType)
    {
        if (MasterVolume == 0) return 0;

        var typeVolume = audioType.ToLower() switch
        {
            "music" => BackgroundMusicEnabled ? MusicVolume : 0,
            "effects" => SoundEffectsEnabled ? SoundEffectsVolume : 0,
            "voice" => AudioInstructionsEnabled ? VoiceVolume : 0,
            _ => MasterVolume
        };

        return (int)(typeVolume * (MasterVolume / 100.0));
    }

    /// <summary>
    /// Updates accessibility settings from a dictionary.
    /// </summary>
    /// <param name="settings">Dictionary of accessibility setting key-value pairs.</param>
    public void UpdateAccessibilitySettings(Dictionary<string, object> settings)
    {
        AccessibilitySettings = System.Text.Json.JsonSerializer.Serialize(settings);
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Gets accessibility settings as a dictionary.
    /// </summary>
    /// <returns>Dictionary of accessibility settings, or empty dictionary if none set.</returns>
    public Dictionary<string, object> GetAccessibilitySettings()
    {
        if (string.IsNullOrEmpty(AccessibilitySettings))
            return new Dictionary<string, object>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(AccessibilitySettings)
                ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Determines if high contrast mode is enabled.
    /// </summary>
    /// <returns>True if high contrast mode is enabled.</returns>
    public bool IsHighContrastEnabled()
    {
        var accessibility = GetAccessibilitySettings();
        return accessibility.ContainsKey("highContrast") && Convert.ToBoolean(accessibility["highContrast"]);
    }

    /// <summary>
    /// Determines if reduced motion is enabled.
    /// </summary>
    /// <returns>True if reduced motion is enabled.</returns>
    public bool IsReducedMotionEnabled()
    {
        var accessibility = GetAccessibilitySettings();
        return accessibility.ContainsKey("reducedMotion") && Convert.ToBoolean(accessibility["reducedMotion"]);
    }

    /// <summary>
    /// Gets the current daily screen time usage in minutes.
    /// </summary>
    /// <param name="todaysUsageMinutes">Today's usage from session data.</param>
    /// <returns>Remaining daily minutes, or null if no limit set.</returns>
    public int? GetRemainingDailyMinutes(int todaysUsageMinutes)
    {
        if (DailyTimeLimitMinutes == 0) return null;

        var remaining = DailyTimeLimitMinutes - todaysUsageMinutes;
        return Math.Max(0, remaining);
    }

    /// <summary>
    /// Determines if the daily time limit has been reached.
    /// </summary>
    /// <param name="todaysUsageMinutes">Today's usage from session data.</param>
    /// <returns>True if time limit reached.</returns>
    public bool HasReachedDailyLimit(int todaysUsageMinutes)
    {
        if (DailyTimeLimitMinutes == 0) return false;
        return todaysUsageMinutes >= DailyTimeLimitMinutes;
    }

    /// <summary>
    /// Gets a comprehensive settings summary for display.
    /// </summary>
    /// <returns>Settings summary object.</returns>
    public object GetSettingsSummary()
    {
        return new
        {
            Languages = new { App = AppLanguage, Audio = AudioLanguage },
            Audio = new
            {
                MasterVolume,
                MusicEnabled = BackgroundMusicEnabled,
                SoundEffectsEnabled,
                AudioInstructionsEnabled,
                EffectiveVoiceVolume = GetEffectiveVolume("voice")
            },
            ParentalControls = new
            {
                Enabled = ParentalControlsEnabled,
                DailyLimitMinutes = DailyTimeLimitMinutes,
                BedtimeEnabled = BedtimeRestrictionsEnabled,
                BedtimeHours = $"{BedtimeStartHour:00}:00 - {BedtimeEndHour:00}:00"
            },
            Learning = new
            {
                DefaultDifficulty,
                AdaptiveLearningEnabled,
                CrownChallengesEnabled
            },
            Accessibility = new
            {
                HighContrast = IsHighContrastEnabled(),
                ReducedMotion = IsReducedMotionEnabled(),
                AnimationSpeed
            },
            IsAppCurrentlyAccessible = IsAppAccessible()
        };
    }

    /// <summary>
    /// Resets settings to default values optimized for children.
    /// </summary>
    public void ResetToDefaults()
    {
        var defaultSettings = new Settings();

        AppLanguage = defaultSettings.AppLanguage;
        AudioLanguage = defaultSettings.AudioLanguage;
        MasterVolume = defaultSettings.MasterVolume;
        MusicVolume = defaultSettings.MusicVolume;
        SoundEffectsVolume = defaultSettings.SoundEffectsVolume;
        VoiceVolume = defaultSettings.VoiceVolume;
        AudioInstructionsEnabled = defaultSettings.AudioInstructionsEnabled;
        BackgroundMusicEnabled = defaultSettings.BackgroundMusicEnabled;
        SoundEffectsEnabled = defaultSettings.SoundEffectsEnabled;
        HapticFeedbackEnabled = defaultSettings.HapticFeedbackEnabled;
        AnimationSpeed = defaultSettings.AnimationSpeed;
        ParentalControlsEnabled = defaultSettings.ParentalControlsEnabled;
        DailyTimeLimitMinutes = defaultSettings.DailyTimeLimitMinutes;
        BedtimeRestrictionsEnabled = defaultSettings.BedtimeRestrictionsEnabled;
        BedtimeStartHour = defaultSettings.BedtimeStartHour;
        BedtimeEndHour = defaultSettings.BedtimeEndHour;
        CrownChallengesEnabled = defaultSettings.CrownChallengesEnabled;
        DefaultDifficulty = defaultSettings.DefaultDifficulty;
        AdaptiveLearningEnabled = defaultSettings.AdaptiveLearningEnabled;
        ProgressReportsEnabled = defaultSettings.ProgressReportsEnabled;
        ProgressReportFrequency = defaultSettings.ProgressReportFrequency;
        AchievementNotificationsEnabled = defaultSettings.AchievementNotificationsEnabled;
        ReminderNotificationsEnabled = defaultSettings.ReminderNotificationsEnabled;

        AccessibilitySettings = null;
        DataCollectionPreferences = null;
        ChildSafetySettings = null;
        CustomLearningGoals = null;
        NotificationPreferences = null;

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }
}