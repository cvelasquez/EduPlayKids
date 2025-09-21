using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Settings entity operations.
/// Manages user preferences, parental controls, and application configuration.
/// Supports accessibility options, language settings, and personalized experiences.
/// </summary>
public interface ISettingsRepository : IGenericRepository<Settings>
{
    #region User-Specific Settings Management

    /// <summary>
    /// Gets all settings for a specific user (parent/guardian).
    /// Essential for loading personalized app configuration and preferences.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of settings for the user</returns>
    Task<IEnumerable<Settings>> GetSettingsByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific setting by user and setting key.
    /// Used for retrieving individual configuration values efficiently.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="settingKey">The setting key identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The setting if found, null otherwise</returns>
    Task<Settings?> GetSettingByUserAndKeyAsync(int userId, string settingKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates or creates a setting for a user with proper validation.
    /// Central method for managing user preferences and configuration changes.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="settingKey">The setting key identifier</param>
    /// <param name="settingValue">The new setting value</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated or created setting</returns>
    Task<Settings> UpdateSettingAsync(int userId, string settingKey, string settingValue, CancellationToken cancellationToken = default);

    #endregion

    #region Language and Localization Settings

    /// <summary>
    /// Gets the user's preferred language setting for app localization.
    /// Critical for bilingual Spanish/English content delivery.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The language code or default if not set</returns>
    Task<string> GetPreferredLanguageAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's preferred language and related localization settings.
    /// Affects app interface, audio instructions, and content presentation.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="languageCode">The new language code (es, en)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdatePreferredLanguageAsync(int userId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets regional settings for cultural adaptation and curriculum alignment.
    /// Important for providing culturally relevant content to Hispanic families.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing regional settings</returns>
    Task<Dictionary<string, string>> GetRegionalSettingsAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Parental Control Settings

    /// <summary>
    /// Gets parental control settings for child safety and content management.
    /// Essential for COPPA compliance and age-appropriate content filtering.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing parental control settings</returns>
    Task<Dictionary<string, string>> GetParentalControlSettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the daily usage limit setting for all children in the family.
    /// Used for enforcing healthy screen time and usage management.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Daily usage limit in minutes</returns>
    Task<int> GetDailyUsageLimitAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates parental control settings with validation and security checks.
    /// Ensures only authorized parents can modify child safety settings.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="controlSettings">Dictionary of control settings to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateParentalControlSettingsAsync(int userId, Dictionary<string, string> controlSettings, CancellationToken cancellationToken = default);

    #endregion

    #region Accessibility and Special Needs Settings

    /// <summary>
    /// Gets accessibility settings for children with special needs support.
    /// Includes visual, auditory, and motor accessibility configurations.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing accessibility settings</returns>
    Task<Dictionary<string, string>> GetAccessibilitySettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates accessibility settings for inclusive learning experiences.
    /// Supports WCAG 2.1 AA compliance and child-specific accessibility needs.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="accessibilitySettings">Dictionary of accessibility settings</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateAccessibilitySettingsAsync(int userId, Dictionary<string, string> accessibilitySettings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audio settings for non-readers and hearing accessibility.
    /// Critical for supporting children ages 3-5 who cannot read yet.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing audio accessibility settings</returns>
    Task<Dictionary<string, string>> GetAudioAccessibilitySettingsAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Child-Specific Personalization Settings

    /// <summary>
    /// Gets personalization settings for individual children in the family.
    /// Supports individual preferences while maintaining family-level controls.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing child-specific settings</returns>
    Task<Dictionary<string, string>> GetChildPersonalizationSettingsAsync(int userId, int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates personalization settings for a specific child.
    /// Allows customization while maintaining parental oversight and safety.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="personalizationSettings">Dictionary of personalization settings</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateChildPersonalizationSettingsAsync(int userId, int childId, Dictionary<string, string> personalizationSettings, CancellationToken cancellationToken = default);

    #endregion

    #region Notification and Communication Settings

    /// <summary>
    /// Gets notification preferences for parent communication and alerts.
    /// Manages how parents receive information about child progress and achievements.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing notification preferences</returns>
    Task<Dictionary<string, string>> GetNotificationSettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates notification settings for parent communication preferences.
    /// Controls frequency and types of progress reports and achievement notifications.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="notificationSettings">Dictionary of notification preferences</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateNotificationSettingsAsync(int userId, Dictionary<string, string> notificationSettings, CancellationToken cancellationToken = default);

    #endregion

    #region Privacy and Data Management Settings

    /// <summary>
    /// Gets privacy settings for COPPA and GDPR-K compliance.
    /// Manages data collection preferences and privacy controls.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing privacy settings</returns>
    Task<Dictionary<string, string>> GetPrivacySettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates privacy settings with legal compliance validation.
    /// Ensures all data collection and usage complies with child privacy laws.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="privacySettings">Dictionary of privacy preferences</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdatePrivacySettingsAsync(int userId, Dictionary<string, string> privacySettings, CancellationToken cancellationToken = default);

    #endregion

    #region App Behavior and Performance Settings

    /// <summary>
    /// Gets app performance settings for device optimization.
    /// Includes graphics quality, animation settings, and performance preferences.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing performance settings</returns>
    Task<Dictionary<string, string>> GetPerformanceSettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets offline mode settings for content availability management.
    /// Critical for the offline-first educational app functionality.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing offline mode settings</returns>
    Task<Dictionary<string, string>> GetOfflineModeSettingsAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Backup and Sync Settings

    /// <summary>
    /// Gets backup and synchronization preferences for data management.
    /// Important for maintaining progress across devices and preventing data loss.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing backup and sync settings</returns>
    Task<Dictionary<string, string>> GetBackupSyncSettingsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates backup and synchronization settings with security validation.
    /// Ensures data backup complies with privacy requirements and user preferences.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="backupSyncSettings">Dictionary of backup and sync preferences</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateBackupSyncSettingsAsync(int userId, Dictionary<string, string> backupSyncSettings, CancellationToken cancellationToken = default);

    #endregion

    #region Settings Analytics and Defaults

    /// <summary>
    /// Gets default settings template for new user initialization.
    /// Provides safe, child-appropriate defaults for all app configurations.
    /// </summary>
    /// <param name="languageCode">The user's preferred language for defaults</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing default settings template</returns>
    Task<Dictionary<string, string>> GetDefaultSettingsTemplateAsync(string languageCode = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes default settings for a new user account.
    /// Creates safe, COPPA-compliant default configuration for families.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="languageCode">The user's preferred language</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task InitializeDefaultSettingsAsync(int userId, string languageCode = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets settings usage statistics for app improvement and user experience.
    /// Provides insights into common configurations and preference patterns.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing anonymized settings usage statistics</returns>
    Task<Dictionary<string, object>> GetSettingsUsageStatisticsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Settings Validation and Security

    /// <summary>
    /// Validates setting values for security and child safety compliance.
    /// Ensures all setting changes maintain child protection and app security.
    /// </summary>
    /// <param name="settingKey">The setting key to validate</param>
    /// <param name="settingValue">The setting value to validate</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if valid, false if validation fails</returns>
    Task<bool> ValidateSettingValueAsync(string settingKey, string settingValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets settings to defaults for troubleshooting or user preference.
    /// Provides safe fallback when settings become corrupted or problematic.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="settingCategory">Optional category to reset (null for all)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task ResetSettingsToDefaultAsync(int userId, string? settingCategory = null, CancellationToken cancellationToken = default);

    #endregion
}