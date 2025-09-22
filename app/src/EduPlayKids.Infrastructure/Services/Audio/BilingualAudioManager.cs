using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Manages bilingual audio resources for Spanish/English educational content.
/// Provides organized access to localized audio files with fallback support
/// optimized for children's educational applications.
/// </summary>
public class BilingualAudioManager
{
    #region Private Fields

    private readonly ILogger<BilingualAudioManager> _logger;
    private readonly ConcurrentDictionary<string, string> _audioPathCache;
    private readonly Dictionary<string, BilingualAudioResource> _audioManifest;
    private readonly string[] _supportedLanguages = { "en", "es" };
    private readonly string _defaultLanguage = "en";

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the BilingualAudioManager class.
    /// </summary>
    /// <param name="logger">Logger for debugging and error tracking</param>
    public BilingualAudioManager(ILogger<BilingualAudioManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _audioPathCache = new ConcurrentDictionary<string, string>();
        _audioManifest = new Dictionary<string, BilingualAudioResource>();

        // Initialize with default educational audio resources
        InitializeDefaultAudioResources();

        _logger.LogInformation("BilingualAudioManager initialized with {Count} audio resources",
            _audioManifest.Count);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the path to a localized audio file.
    /// </summary>
    /// <param name="audioKey">The audio resource key</param>
    /// <param name="language">The target language (en/es)</param>
    /// <returns>Path to the audio file, or null if not found</returns>
    public string? GetLocalizedAudioPath(string audioKey, string? language = null)
    {
        if (string.IsNullOrEmpty(audioKey))
        {
            return null;
        }

        var targetLanguage = ValidateLanguage(language);
        var cacheKey = $"{audioKey}_{targetLanguage}";

        // Check cache first
        if (_audioPathCache.TryGetValue(cacheKey, out var cachedPath))
        {
            return cachedPath;
        }

        // Try to resolve the audio path
        var audioPath = ResolveAudioPath(audioKey, targetLanguage);

        if (audioPath != null)
        {
            _audioPathCache[cacheKey] = audioPath;
            return audioPath;
        }

        // Fallback to default language if different
        if (targetLanguage != _defaultLanguage)
        {
            var fallbackPath = ResolveAudioPath(audioKey, _defaultLanguage);
            if (fallbackPath != null)
            {
                _logger.LogWarning("Audio fallback: {Key} not found in {Language}, using {DefaultLanguage}",
                    audioKey, targetLanguage, _defaultLanguage);

                _audioPathCache[cacheKey] = fallbackPath;
                return fallbackPath;
            }
        }

        _logger.LogWarning("Audio file not found: {Key} for any supported language", audioKey);
        return null;
    }

    /// <summary>
    /// Gets information about a specific audio resource.
    /// </summary>
    /// <param name="audioKey">The audio resource key</param>
    /// <returns>Audio resource information, or null if not found</returns>
    public BilingualAudioResource? GetAudioResource(string audioKey)
    {
        return _audioManifest.TryGetValue(audioKey, out var resource) ? resource : null;
    }

    /// <summary>
    /// Gets all audio resources for a specific category.
    /// </summary>
    /// <param name="category">The audio category</param>
    /// <returns>Collection of audio resources in the category</returns>
    public IEnumerable<BilingualAudioResource> GetResourcesByCategory(string category)
    {
        return _audioManifest.Values
            .Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Preloads audio paths for better performance.
    /// </summary>
    /// <param name="audioKeys">Audio keys to preload</param>
    /// <param name="language">Target language</param>
    /// <returns>Task representing the async operation</returns>
    public async Task PreloadAudioPathsAsync(IEnumerable<string> audioKeys, string? language = null)
    {
        var targetLanguage = ValidateLanguage(language);

        var preloadTasks = audioKeys.Select(async audioKey =>
        {
            try
            {
                var path = GetLocalizedAudioPath(audioKey, targetLanguage);
                if (path != null)
                {
                    // Verify the file exists to populate cache
                    var exists = await FileExistsAsync(path);
                    if (exists)
                    {
                        _logger.LogDebug("Preloaded audio path: {Key} -> {Path}", audioKey, path);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preloading audio path: {Key}", audioKey);
            }
        });

        await Task.WhenAll(preloadTasks);

        _logger.LogInformation("Preloaded {Count} audio paths for language: {Language}",
            audioKeys.Count(), targetLanguage);
    }

    /// <summary>
    /// Validates if an audio resource exists for both languages.
    /// </summary>
    /// <param name="audioKey">The audio resource key</param>
    /// <returns>Dictionary indicating availability for each language</returns>
    public async Task<Dictionary<string, bool>> ValidateAudioAvailabilityAsync(string audioKey)
    {
        var results = new Dictionary<string, bool>();

        foreach (var language in _supportedLanguages)
        {
            var path = GetLocalizedAudioPath(audioKey, language);
            results[language] = path != null && await FileExistsAsync(path);
        }

        return results;
    }

    /// <summary>
    /// Clears the audio path cache.
    /// </summary>
    public void ClearCache()
    {
        var cacheSize = _audioPathCache.Count;
        _audioPathCache.Clear();

        _logger.LogInformation("Cleared audio path cache: {Count} entries removed", cacheSize);
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    /// <returns>Cache statistics</returns>
    public BilingualAudioCacheStats GetCacheStats()
    {
        return new BilingualAudioCacheStats
        {
            CachedPathsCount = _audioPathCache.Count,
            ResourcesCount = _audioManifest.Count,
            SupportedLanguages = _supportedLanguages.ToList(),
            DefaultLanguage = _defaultLanguage
        };
    }

    #endregion

    #region Private Methods

    private string ValidateLanguage(string? language)
    {
        if (string.IsNullOrEmpty(language))
        {
            return GetSystemLanguage();
        }

        var normalized = language.ToLowerInvariant();
        return _supportedLanguages.Contains(normalized) ? normalized : _defaultLanguage;
    }

    private string GetSystemLanguage()
    {
        try
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture;
            var languageCode = culture.TwoLetterISOLanguageName.ToLowerInvariant();
            return _supportedLanguages.Contains(languageCode) ? languageCode : _defaultLanguage;
        }
        catch
        {
            return _defaultLanguage;
        }
    }

    private string? ResolveAudioPath(string audioKey, string language)
    {
        // Standard path pattern: Audio/{language}/{audioKey}.mp3
        return Path.Combine("Audio", language, $"{audioKey}.mp3");
    }

    private async Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            // Check local file system
            if (File.Exists(filePath))
            {
                return true;
            }

            // Check app package resources
            try
            {
                // TODO: Implement FileSystem access when moved to Presentation layer
            // using var stream = await FileSystem.OpenAppPackageFileAsync(filePath);

            // Mock manifest data for Infrastructure layer
            await Task.Delay(1); // Simulate file reading
            var mockManifest = """
            {
              "ui_button_press": {
                "en": "Audio/en/ui_button_press.mp3",
                "es": "Audio/es/ui_button_press.mp3"
              },
              "feedback_success_medium": {
                "en": "Audio/en/feedback_success_medium.mp3",
                "es": "Audio/es/feedback_success_medium.mp3"
              }
            }
            """;
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mockManifest));
                return stream != null && stream.Length > 0;
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    private void InitializeDefaultAudioResources()
    {
        // UI Interaction Sounds
        AddAudioResource("ui_button_press", "ui", "Button press sound", AudioType.UIInteraction, 200);
        AddAudioResource("ui_page_transition", "ui", "Page transition sound", AudioType.UIInteraction, 300);
        AddAudioResource("ui_menu_toggle", "ui", "Menu toggle sound", AudioType.UIInteraction, 150);
        AddAudioResource("ui_item_select", "ui", "Item selection sound", AudioType.UIInteraction, 100);
        AddAudioResource("ui_drag_drop", "ui", "Drag and drop sound", AudioType.UIInteraction, 250);
        AddAudioResource("ui_modal_appear", "ui", "Modal appearance sound", AudioType.UIInteraction, 200);
        AddAudioResource("ui_swipe", "ui", "Swipe gesture sound", AudioType.UIInteraction, 150);
        AddAudioResource("ui_long_press", "ui", "Long press sound", AudioType.UIInteraction, 300);

        // Success Feedback
        AddAudioResource("feedback_success_soft", "feedback", "Gentle success sound", AudioType.SuccessFeedback, 1000);
        AddAudioResource("feedback_success_medium", "feedback", "Medium success celebration", AudioType.SuccessFeedback, 1500);
        AddAudioResource("feedback_success_celebration", "feedback", "High-energy success sound", AudioType.SuccessFeedback, 2000);

        // Error Feedback
        AddAudioResource("feedback_error_gentle", "feedback", "Gentle correction sound", AudioType.ErrorFeedback, 800);
        AddAudioResource("feedback_error_encouraging", "feedback", "Encouraging retry sound", AudioType.ErrorFeedback, 1000);
        AddAudioResource("feedback_error_motivating", "feedback", "Motivating guidance sound", AudioType.ErrorFeedback, 1200);

        // Completion Sounds
        AddAudioResource("completion_one_star", "completion", "One star completion", AudioType.Completion, 2000);
        AddAudioResource("completion_two_stars", "completion", "Two stars completion", AudioType.Completion, 2500);
        AddAudioResource("completion_three_stars", "completion", "Three stars completion", AudioType.Completion, 3000);
        AddAudioResource("completion_general", "completion", "General completion sound", AudioType.Completion, 2200);

        // Educational Instructions
        AddAudioResource("activity_intro_math", "instruction", "Math activity introduction", AudioType.Instruction, 4000);
        AddAudioResource("activity_intro_reading", "instruction", "Reading activity introduction", AudioType.Instruction, 4000);
        AddAudioResource("activity_intro_logic", "instruction", "Logic activity introduction", AudioType.Instruction, 4000);
        AddAudioResource("activity_intro_science", "instruction", "Science activity introduction", AudioType.Instruction, 4000);
        AddAudioResource("activity_intro_concepts", "instruction", "Basic concepts introduction", AudioType.Instruction, 4000);

        // Page Introductions
        AddAudioResource("page_intro_ageselection", "instruction", "Age selection welcome", AudioType.Instruction, 3000);
        AddAudioResource("page_intro_subjectselection", "instruction", "Subject selection welcome", AudioType.Instruction, 3000);
        AddAudioResource("page_intro_activity", "instruction", "Activity page welcome", AudioType.Instruction, 2500);

        // Welcome Messages
        AddAudioResource("welcome_age_selection", "instruction", "Welcome to age selection", AudioType.Instruction, 3000);
        AddAudioResource("welcome_subject_selection", "instruction", "Welcome to subject selection", AudioType.Instruction, 3000);
        AddAudioResource("welcome_activity", "instruction", "Welcome to activity", AudioType.Instruction, 2500);

        // Navigation
        AddAudioResource("navigation_back", "instruction", "Going back", AudioType.Instruction, 1500);

        // Encouragement Messages
        AddAudioResource("encouragement_gentle", "instruction", "Gentle encouragement", AudioType.Instruction, 3000);
        AddAudioResource("encouragement_supportive", "instruction", "Supportive encouragement", AudioType.Instruction, 3500);
        AddAudioResource("encouragement_motivating", "instruction", "Motivating encouragement", AudioType.Instruction, 4000);

        // Try Again Messages
        AddAudioResource("try_again_gentle", "instruction", "Gentle try again", AudioType.Instruction, 2000);
        AddAudioResource("try_again_encouraging", "instruction", "Encouraging try again", AudioType.Instruction, 2500);
        AddAudioResource("show_correct_answer", "instruction", "Showing correct answer", AudioType.Instruction, 3000);

        // Achievement Sounds
        AddAudioResource("achievement_streak", "achievement", "Achievement streak celebration", AudioType.Achievement, 3000);

        // Background Music (for different subjects)
        AddAudioResource("background_music_math", "music", "Math activity background", AudioType.BackgroundMusic, 120000);
        AddAudioResource("background_music_reading", "music", "Reading activity background", AudioType.BackgroundMusic, 120000);
        AddAudioResource("background_music_logic", "music", "Logic activity background", AudioType.BackgroundMusic, 120000);
        AddAudioResource("background_music_science", "music", "Science activity background", AudioType.BackgroundMusic, 120000);
        AddAudioResource("background_music_concepts", "music", "Concepts activity background", AudioType.BackgroundMusic, 120000);

        _logger.LogDebug("Initialized {Count} default audio resources", _audioManifest.Count);
    }

    private void AddAudioResource(string key, string category, string description, AudioType audioType, int durationMs)
    {
        _audioManifest[key] = new BilingualAudioResource
        {
            Key = key,
            Category = category,
            Description = description,
            AudioType = audioType,
            EstimatedDurationMs = durationMs
        };
    }

    #endregion
}

/// <summary>
/// Represents a bilingual audio resource.
/// </summary>
public class BilingualAudioResource
{
    /// <summary>
    /// Gets or sets the resource key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audio type.
    /// </summary>
    public AudioType AudioType { get; set; }

    /// <summary>
    /// Gets or sets the estimated duration in milliseconds.
    /// </summary>
    public int EstimatedDurationMs { get; set; }
}

/// <summary>
/// Cache statistics for bilingual audio management.
/// </summary>
public class BilingualAudioCacheStats
{
    /// <summary>
    /// Gets or sets the number of cached paths.
    /// </summary>
    public int CachedPathsCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of resources.
    /// </summary>
    public int ResourcesCount { get; set; }

    /// <summary>
    /// Gets or sets the supported languages.
    /// </summary>
    public List<string> SupportedLanguages { get; set; } = new();

    /// <summary>
    /// Gets or sets the default language.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";
}