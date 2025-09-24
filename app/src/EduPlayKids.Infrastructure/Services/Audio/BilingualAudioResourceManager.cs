using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Comprehensive bilingual audio resource management system for EduPlayKids.
/// Handles Spanish/English audio resources with intelligent caching, preloading,
/// and age-appropriate content selection for children aged 3-8.
/// </summary>
public class BilingualAudioResourceManager : IDisposable
{
    #region Private Fields

    private readonly ILogger<BilingualAudioResourceManager> _logger;
    private readonly ConcurrentDictionary<string, AudioResourceEntry> _audioResources;
    private readonly ConcurrentDictionary<string, byte[]> _audioCache;
    private readonly SemaphoreSlim _resourceLock;
    private readonly Timer _cacheCleanupTimer;

    private readonly Dictionary<string, LanguageAudioPack> _languagePacks;
    private readonly Dictionary<AgeGroup, AgeGroupAudioSettings> _ageGroupSettings;
    private readonly HashSet<string> _preloadedResources;
    private readonly AudioResourceConfiguration _configuration;

    private string _currentLanguage;
    private AgeGroup _currentAgeGroup;
    private bool _disposed;
    private long _totalCacheSize;
    private int _cacheHits;
    private int _cacheMisses;
    private const long MAX_CACHE_SIZE = 50 * 1024 * 1024; // 50MB max cache

    #endregion

    #region Constructor

    public BilingualAudioResourceManager(ILogger<BilingualAudioResourceManager> logger, AudioResourceConfiguration? configuration = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? AudioResourceConfiguration.Default;

        _audioResources = new ConcurrentDictionary<string, AudioResourceEntry>();
        _audioCache = new ConcurrentDictionary<string, byte[]>();
        _resourceLock = new SemaphoreSlim(1, 1);
        _languagePacks = new Dictionary<string, LanguageAudioPack>();
        _ageGroupSettings = new Dictionary<AgeGroup, AgeGroupAudioSettings>();
        _preloadedResources = new HashSet<string>();

        _currentLanguage = GetSystemLanguage();
        _currentAgeGroup = AgeGroup.Primary;

        InitializeLanguagePacks();
        InitializeAgeGroupSettings();
        InitializeResourceCatalog();

        // Start cache cleanup timer (runs every 10 minutes)
        _cacheCleanupTimer = new Timer(PerformCacheCleanup, null,
            TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));

        _logger.LogInformation("Bilingual audio resource manager initialized with language: {Language}", _currentLanguage);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the current active language.
    /// </summary>
    public string CurrentLanguage => _currentLanguage;

    /// <summary>
    /// Gets the current age group setting.
    /// </summary>
    public AgeGroup CurrentAgeGroup => _currentAgeGroup;

    /// <summary>
    /// Gets the total number of cached audio resources.
    /// </summary>
    public int CachedResourceCount => _audioCache.Count;

    /// <summary>
    /// Gets the total cache size in bytes.
    /// </summary>
    public long TotalCacheSize => _totalCacheSize;

    /// <summary>
    /// Gets the available languages.
    /// </summary>
    public IEnumerable<string> AvailableLanguages => _languagePacks.Keys;

    /// <summary>
    /// Gets whether the resource manager is ready.
    /// </summary>
    public bool IsReady => _languagePacks.Any() && !_disposed;

    #endregion

    #region Language Management

    /// <summary>
    /// Sets the active language for audio resource selection.
    /// </summary>
    /// <param name="languageCode">ISO language code (e.g., "en", "es")</param>
    /// <returns>True if language was set successfully, false if unsupported</returns>
    public async Task<bool> SetLanguageAsync(string languageCode)
    {
        try
        {
            await _resourceLock.WaitAsync();

            if (!_languagePacks.ContainsKey(languageCode))
            {
                _logger.LogWarning("Unsupported language: {Language}", languageCode);
                return false;
            }

            var oldLanguage = _currentLanguage;
            _currentLanguage = languageCode;

            // Preload essential resources for new language
            await PreloadEssentialResourcesAsync();

            _logger.LogInformation("Language changed from {OldLanguage} to {NewLanguage}", oldLanguage, languageCode);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting language to {Language}", languageCode);
            return false;
        }
        finally
        {
            _resourceLock.Release();
        }
    }

    /// <summary>
    /// Sets the age group for appropriate content selection.
    /// </summary>
    /// <param name="childAge">Child's age in years</param>
    public void SetAgeGroup(int childAge)
    {
        var newAgeGroup = childAge switch
        {
            <= 4 => AgeGroup.PreK,
            5 => AgeGroup.Kindergarten,
            _ => AgeGroup.Primary
        };

        if (newAgeGroup != _currentAgeGroup)
        {
            var oldAgeGroup = _currentAgeGroup;
            _currentAgeGroup = newAgeGroup;
            _logger.LogDebug("Age group changed from {OldGroup} to {NewGroup} for child age {Age}",
                oldAgeGroup, newAgeGroup, childAge);
        }
    }

    #endregion

    #region Resource Retrieval

    /// <summary>
    /// Gets the audio file path for a specific resource key and language.
    /// </summary>
    /// <param name="resourceKey">Resource key (e.g., "activity.intro.math.easy")</param>
    /// <param name="languageCode">Optional language override</param>
    /// <returns>Audio file path or null if not found</returns>
    public string? GetAudioResourcePath(string resourceKey, string? languageCode = null)
    {
        try
        {
            var language = languageCode ?? _currentLanguage;
            var fullKey = $"{language}.{resourceKey}.{_currentAgeGroup}";

            if (_audioResources.TryGetValue(fullKey, out var entry))
            {
                entry.LastAccessed = DateTime.UtcNow;
                entry.AccessCount++;
                return entry.FilePath;
            }

            // Try fallback without age group
            var fallbackKey = $"{language}.{resourceKey}";
            if (_audioResources.TryGetValue(fallbackKey, out var fallbackEntry))
            {
                fallbackEntry.LastAccessed = DateTime.UtcNow;
                fallbackEntry.AccessCount++;
                return fallbackEntry.FilePath;
            }

            _logger.LogDebug("Audio resource not found: {ResourceKey} for language {Language}", resourceKey, language);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audio resource path for {ResourceKey}", resourceKey);
            return null;
        }
    }

    /// <summary>
    /// Gets educational audio content with full metadata.
    /// </summary>
    /// <param name="resourceKey">Resource key</param>
    /// <param name="languageCode">Optional language override</param>
    /// <returns>Educational audio content or null if not found</returns>
    public EducationalAudioContent? GetEducationalAudioContent(string resourceKey, string? languageCode = null)
    {
        try
        {
            var language = languageCode ?? _currentLanguage;
            var audioPath = GetAudioResourcePath(resourceKey, language);

            if (string.IsNullOrEmpty(audioPath))
                return null;

            // Extract subject and activity type from resource key
            var keyParts = resourceKey.Split('.');
            var subject = keyParts.Length > 1 ? keyParts[1] : "general";
            var activityType = keyParts.Length > 2 ? keyParts[2] : "general";

            var content = new EducationalAudioContent
            {
                Id = Guid.NewGuid().ToString(),
                LocalizationKey = resourceKey,
                ContentType = DetermineContentType(resourceKey),
                TargetAgeGroup = _currentAgeGroup,
                Subject = subject,
                ActivityType = activityType,
                Priority = DeterminePriority(resourceKey)
            };

            // Add audio paths for available languages
            foreach (var availableLanguage in AvailableLanguages)
            {
                var langPath = GetAudioResourcePath(resourceKey, availableLanguage);
                if (!string.IsNullOrEmpty(langPath))
                {
                    content.AudioPaths[availableLanguage] = langPath;
                }
            }

            // Add transcripts if available
            content.Transcripts[language] = GetTranscript(resourceKey, language);

            // Add metadata tags
            content.Tags.Add(subject);
            content.Tags.Add(activityType);
            content.Tags.Add(_currentAgeGroup.ToString());

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting educational audio content for {ResourceKey}", resourceKey);
            return null;
        }
    }

    /// <summary>
    /// Gets multiple audio resources efficiently.
    /// </summary>
    /// <param name="resourceKeys">Collection of resource keys</param>
    /// <param name="languageCode">Optional language override</param>
    /// <returns>Dictionary of resource key to audio path</returns>
    public async Task<Dictionary<string, string>> GetMultipleAudioResourcesAsync(
        IEnumerable<string> resourceKeys,
        string? languageCode = null)
    {
        try
        {
            var results = new Dictionary<string, string>();
            var language = languageCode ?? _currentLanguage;

            await Task.Run(() =>
            {
                foreach (var key in resourceKeys)
                {
                    var path = GetAudioResourcePath(key, language);
                    if (!string.IsNullOrEmpty(path))
                    {
                        results[key] = path;
                    }
                }
            });

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple audio resources");
            return new Dictionary<string, string>();
        }
    }

    #endregion

    #region Resource Caching

    /// <summary>
    /// Preloads essential audio resources for immediate playback.
    /// </summary>
    /// <param name="resourceKeys">Optional specific resources to preload</param>
    /// <returns>Number of resources successfully preloaded</returns>
    public async Task<int> PreloadAudioResourcesAsync(IEnumerable<string>? resourceKeys = null)
    {
        try
        {
            await _resourceLock.WaitAsync();

            var keysToPreload = resourceKeys?.ToList() ?? GetEssentialResourceKeys();
            var preloadedCount = 0;

            foreach (var key in keysToPreload)
            {
                if (await PreloadSingleResourceAsync(key))
                {
                    preloadedCount++;
                }
            }

            _logger.LogInformation("Preloaded {Count} audio resources for language {Language}",
                preloadedCount, _currentLanguage);

            return preloadedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preloading audio resources");
            return 0;
        }
        finally
        {
            _resourceLock.Release();
        }
    }

    private async Task<bool> PreloadSingleResourceAsync(string resourceKey)
    {
        try
        {
            var audioPath = GetAudioResourcePath(resourceKey);
            if (string.IsNullOrEmpty(audioPath))
                return false;

            var cacheKey = $"{_currentLanguage}.{resourceKey}";
            if (_audioCache.ContainsKey(cacheKey) || _preloadedResources.Contains(cacheKey))
                return true; // Already cached

            // In a real implementation, this would load the actual audio file
            // For now, simulate loading
            await Task.Delay(10); // Simulate I/O

            var simulatedAudioData = System.Text.Encoding.UTF8.GetBytes($"Audio data for {resourceKey}");

            if (_totalCacheSize + simulatedAudioData.Length <= MAX_CACHE_SIZE)
            {
                _audioCache[cacheKey] = simulatedAudioData;
                _preloadedResources.Add(cacheKey);
                Interlocked.Add(ref _totalCacheSize, simulatedAudioData.Length);
                return true;
            }

            return false; // Cache full
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preloading resource {ResourceKey}", resourceKey);
            return false;
        }
    }

    /// <summary>
    /// Clears audio cache to free memory.
    /// </summary>
    /// <param name="keepEssential">Whether to keep essential resources</param>
    /// <returns>Amount of memory freed in bytes</returns>
    public async Task<long> ClearAudioCacheAsync(bool keepEssential = true)
    {
        try
        {
            await _resourceLock.WaitAsync();

            var freedMemory = 0L;
            var essentialKeys = keepEssential ? GetEssentialResourceKeys() : new List<string>();

            var keysToRemove = new List<string>();

            foreach (var kvp in _audioCache)
            {
                var resourceKey = ExtractResourceKey(kvp.Key);
                if (!keepEssential || !essentialKeys.Contains(resourceKey))
                {
                    keysToRemove.Add(kvp.Key);
                    freedMemory += kvp.Value.Length;
                }
            }

            foreach (var key in keysToRemove)
            {
                _audioCache.TryRemove(key, out _);
                _preloadedResources.Remove(key);
            }

            Interlocked.Add(ref _totalCacheSize, -freedMemory);

            _logger.LogInformation("Cleared {Count} cached resources, freed {Memory} bytes",
                keysToRemove.Count, freedMemory);

            return freedMemory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing audio cache");
            return 0;
        }
        finally
        {
            _resourceLock.Release();
        }
    }

    #endregion

    #region Resource Discovery

    /// <summary>
    /// Finds all audio resources matching a pattern.
    /// </summary>
    /// <param name="pattern">Search pattern (e.g., "activity.intro.*")</param>
    /// <param name="languageCode">Optional language filter</param>
    /// <returns>List of matching resource keys</returns>
    public List<string> FindAudioResources(string pattern, string? languageCode = null)
    {
        try
        {
            var language = languageCode ?? _currentLanguage;
            var results = new List<string>();

            foreach (var kvp in _audioResources)
            {
                if (kvp.Key.StartsWith($"{language}.") && MatchesPattern(ExtractResourceKey(kvp.Key), pattern))
                {
                    results.Add(ExtractResourceKey(kvp.Key));
                }
            }

            return results.Distinct().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding audio resources with pattern {Pattern}", pattern);
            return new List<string>();
        }
    }

    /// <summary>
    /// Gets audio resources by subject.
    /// </summary>
    /// <param name="subject">Subject name (math, reading, etc.)</param>
    /// <param name="languageCode">Optional language filter</param>
    /// <returns>List of resource keys for the subject</returns>
    public List<string> GetResourcesBySubject(string subject, string? languageCode = null)
    {
        return FindAudioResources($"*.{subject}.*", languageCode);
    }

    /// <summary>
    /// Gets audio resources by activity type.
    /// </summary>
    /// <param name="activityType">Activity type (multiple_choice, drag_drop, etc.)</param>
    /// <param name="languageCode">Optional language filter</param>
    /// <returns>List of resource keys for the activity type</returns>
    public List<string> GetResourcesByActivityType(string activityType, string? languageCode = null)
    {
        return FindAudioResources($"*.*.{activityType}*", languageCode);
    }

    #endregion

    #region Cache Statistics

    /// <summary>
    /// Gets comprehensive cache statistics.
    /// </summary>
    /// <returns>Audio cache information</returns>
    public AudioCacheInfo GetCacheStatistics()
    {
        try
        {
            var mostAccessed = _audioResources
                .OrderByDescending(x => x.Value.AccessCount)
                .Take(10)
                .ToDictionary(x => ExtractResourceKey(x.Key), x => x.Value.AccessCount);

            var recentlyAccessed = _audioResources
                .Where(x => x.Value.LastAccessed > DateTime.UtcNow.AddHours(-1))
                .Count();

            var cacheInfo = new AudioCacheInfo
            {
                CachedFileCount = _audioCache.Count,
                TotalCacheSizeBytes = _totalCacheSize,
                MaxCacheSizeBytes = MAX_CACHE_SIZE,
                CacheHits = _cacheHits,
                CacheMisses = _cacheMisses,
                CurrentLanguage = _currentLanguage,
                CurrentAgeGroup = _currentAgeGroup.ToString()
            };

            // Add cached items
            cacheInfo.CachedItems.AddRange(mostAccessed.Select(item => new CachedAudioInfo
            {
                CacheKey = item.Key,
                OriginalPath = item.Key,
                AccessCount = item.Value,
                LastAccessed = DateTime.UtcNow,
                CachedAt = DateTime.UtcNow.AddMinutes(-5), // Placeholder
                SizeBytes = 1024 // Placeholder
            }));

            return cacheInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return new AudioCacheInfo();
        }
    }

    #endregion

    #region Resource Validation

    /// <summary>
    /// Validates that all essential resources are available.
    /// </summary>
    /// <param name="languageCode">Optional language to validate</param>
    /// <returns>Validation result with missing resources</returns>
    public async Task<AudioResourceValidationResult> ValidateResourcesAsync(string? languageCode = null)
    {
        try
        {
            var language = languageCode ?? _currentLanguage;
            var essentialKeys = GetEssentialResourceKeys();
            var missingResources = new List<string>();
            var availableResources = new List<string>();

            foreach (var key in essentialKeys)
            {
                var path = GetAudioResourcePath(key, language);
                if (string.IsNullOrEmpty(path))
                {
                    missingResources.Add(key);
                }
                else
                {
                    availableResources.Add(key);
                }
            }

            var result = new AudioResourceValidationResult
            {
                Language = language,
                TotalEssentialResources = essentialKeys.Count,
                AvailableResources = availableResources.Count,
                MissingResources = missingResources,
                IsValid = !missingResources.Any(),
                ValidationTime = DateTime.UtcNow
            };

            if (!result.IsValid)
            {
                _logger.LogWarning("Missing {Count} essential audio resources for language {Language}: {Missing}",
                    missingResources.Count, language, string.Join(", ", missingResources));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating audio resources");
            return new AudioResourceValidationResult
            {
                Language = languageCode ?? _currentLanguage,
                IsValid = false,
                MissingResources = new List<string> { "validation_error" }
            };
        }
    }

    #endregion

    #region Helper Methods

    private void InitializeLanguagePacks()
    {
        // Initialize English language pack
        _languagePacks["en"] = new LanguageAudioPack
        {
            LanguageCode = "en",
            LanguageName = "English",
            IsComplete = true,
            ResourceCount = 0, // Will be updated during resource loading
            LastUpdated = DateTime.UtcNow
        };

        // Initialize Spanish language pack
        _languagePacks["es"] = new LanguageAudioPack
        {
            LanguageCode = "es",
            LanguageName = "Español",
            IsComplete = true,
            ResourceCount = 0, // Will be updated during resource loading
            LastUpdated = DateTime.UtcNow
        };
    }

    private void InitializeAgeGroupSettings()
    {
        _ageGroupSettings[AgeGroup.PreK] = new AgeGroupAudioSettings
        {
            AgeGroup = AgeGroup.PreK,
            PreferredSpeechRate = 0.8f,
            MaxVolumeLevel = 0.7f,
            RequiresSlowSpeech = true,
            EnableRepetition = true,
            SimplifiedVocabulary = true
        };

        _ageGroupSettings[AgeGroup.Kindergarten] = new AgeGroupAudioSettings
        {
            AgeGroup = AgeGroup.Kindergarten,
            PreferredSpeechRate = 0.9f,
            MaxVolumeLevel = 0.8f,
            RequiresSlowSpeech = false,
            EnableRepetition = true,
            SimplifiedVocabulary = true
        };

        _ageGroupSettings[AgeGroup.Primary] = new AgeGroupAudioSettings
        {
            AgeGroup = AgeGroup.Primary,
            PreferredSpeechRate = 1.0f,
            MaxVolumeLevel = 0.85f,
            RequiresSlowSpeech = false,
            EnableRepetition = false,
            SimplifiedVocabulary = false
        };
    }

    private void InitializeResourceCatalog()
    {
        // In a real implementation, this would load from configuration files or databases
        // For now, add some sample resources
        AddSampleResources();
    }

    private void AddSampleResources()
    {
        var sampleResources = new[]
        {
            // Activity introductions
            "activity.intro.math.easy",
            "activity.intro.math.medium",
            "activity.intro.math.hard",
            "activity.intro.reading.easy",
            "activity.intro.reading.medium",
            "activity.intro.reading.hard",

            // UI feedback
            "ui.button_press",
            "ui.success",
            "ui.error",
            "ui.navigation",

            // Celebrations
            "celebration.perfect_score",
            "celebration.crown_unlock.math",
            "celebration.crown_unlock.reading",

            // Encouragement
            "encouragement.keep_going",
            "encouragement.great_progress",
            "encouragement.nearly_done",

            // Instructions
            "instruction.multiple_choice",
            "instruction.drag_drop",
            "instruction.matching"
        };

        foreach (var resource in sampleResources)
        {
            foreach (var language in AvailableLanguages)
            {
                foreach (var ageGroup in Enum.GetValues<AgeGroup>())
                {
                    var key = $"{language}.{resource}.{ageGroup}";
                    var filePath = $"audio/{language}/{ageGroup}/{resource.Replace('.', '/')}.mp3";

                    _audioResources[key] = new AudioResourceEntry
                    {
                        ResourceKey = resource,
                        Language = language,
                        AgeGroup = ageGroup,
                        FilePath = filePath,
                        FileSize = 1024 * Random.Shared.Next(10, 200), // Random size 10-200KB
                        Duration = TimeSpan.FromSeconds(Random.Shared.Next(2, 10)),
                        LastAccessed = DateTime.UtcNow,
                        AccessCount = 0
                    };
                }

                // Also add general version without age group
                var generalKey = $"{language}.{resource}";
                var generalFilePath = $"audio/{language}/general/{resource.Replace('.', '/')}.mp3";

                _audioResources[generalKey] = new AudioResourceEntry
                {
                    ResourceKey = resource,
                    Language = language,
                    AgeGroup = AgeGroup.Primary, // Default
                    FilePath = generalFilePath,
                    FileSize = 1024 * Random.Shared.Next(10, 200),
                    Duration = TimeSpan.FromSeconds(Random.Shared.Next(2, 10)),
                    LastAccessed = DateTime.UtcNow,
                    AccessCount = 0
                };
            }
        }

        // Update language pack resource counts
        foreach (var language in _languagePacks.Keys.ToList())
        {
            var pack = _languagePacks[language];
            pack.ResourceCount = _audioResources.Count(x => x.Key.StartsWith($"{language}."));
            _languagePacks[language] = pack;
        }
    }

    private async Task PreloadEssentialResourcesAsync()
    {
        var essentialKeys = GetEssentialResourceKeys();
        await PreloadAudioResourcesAsync(essentialKeys);
    }

    private List<string> GetEssentialResourceKeys()
    {
        return new List<string>
        {
            "ui.button_press",
            "ui.success",
            "ui.error",
            "activity.intro.math.easy",
            "activity.intro.reading.easy",
            "encouragement.keep_going",
            "instruction.multiple_choice"
        };
    }

    private EducationalContentType DetermineContentType(string resourceKey)
    {
        return resourceKey switch
        {
            var key when key.Contains(".intro") => EducationalContentType.ActivityIntroduction,
            var key when key.Contains(".instruction") => EducationalContentType.StepByStepGuidance,
            var key when key.Contains(".question") => EducationalContentType.QuestionNarration,
            var key when key.Contains(".hint") => EducationalContentType.HintAndHelp,
            var key when key.Contains(".encouragement") => EducationalContentType.ProgressEncouragement,
            var key when key.Contains(".description") => EducationalContentType.VisualDescription,
            _ => EducationalContentType.ActivityIntroduction
        };
    }

    private AudioPriority DeterminePriority(string resourceKey)
    {
        return resourceKey switch
        {
            var key when key.Contains(".intro") || key.Contains(".instruction") => AudioPriority.High,
            var key when key.Contains(".celebration") || key.Contains(".achievement") => AudioPriority.High,
            var key when key.Contains(".encouragement") => AudioPriority.Normal,
            var key when key.Contains(".ui.") => AudioPriority.Low,
            _ => AudioPriority.Normal
        };
    }

    private string GetTranscript(string resourceKey, string language)
    {
        // In a real implementation, this would load transcripts from files
        // For now, return a sample transcript
        return $"[Transcript for {resourceKey} in {language}]";
    }

    private bool MatchesPattern(string resourceKey, string pattern)
    {
        // Simple pattern matching with wildcards
        var regexPattern = pattern.Replace("*", ".*").Replace("?", ".");
        return System.Text.RegularExpressions.Regex.IsMatch(resourceKey, regexPattern);
    }

    private string ExtractResourceKey(string fullKey)
    {
        // Remove language prefix and age group suffix
        var parts = fullKey.Split('.');
        if (parts.Length < 2) return fullKey;

        // Skip language (first part) and potentially age group (last part if it's an enum)
        var endIndex = parts.Length;
        if (Enum.TryParse<AgeGroup>(parts[^1], out _))
        {
            endIndex--;
        }

        return string.Join(".", parts.Skip(1).Take(endIndex - 1));
    }

    private double CalculateCacheHitRate()
    {
        var totalAccess = _audioResources.Values.Sum(x => x.AccessCount);
        var cacheHits = _audioResources.Values.Where(x => _audioCache.ContainsKey($"{x.Language}.{x.ResourceKey}")).Sum(x => x.AccessCount);

        return totalAccess > 0 ? (double)cacheHits / totalAccess : 0.0;
    }

    private string GetSystemLanguage()
    {
        try
        {
            var culture = CultureInfo.CurrentCulture;
            return culture.TwoLetterISOLanguageName switch
            {
                "es" => "es",
                _ => "en"
            };
        }
        catch
        {
            return "en"; // Default to English
        }
    }

    private void PerformCacheCleanup(object? state)
    {
        try
        {
            if (_totalCacheSize > MAX_CACHE_SIZE * 0.8) // 80% threshold
            {
                Task.Run(async () => await ClearAudioCacheAsync(keepEssential: true));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache cleanup");
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _cacheCleanupTimer?.Dispose();
            _resourceLock?.Dispose();
            _audioCache.Clear();
            _audioResources.Clear();
            _preloadedResources.Clear();
            _disposed = true;
        }
    }

    #endregion
}

#region Supporting Classes

/// <summary>
/// Configuration for audio resource management.
/// </summary>
public class AudioResourceConfiguration
{
    public long MaxCacheSize { get; set; } = 50 * 1024 * 1024; // 50MB
    public TimeSpan CacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(10);
    public bool EnablePreloading { get; set; } = true;
    public bool EnableAgeGroupSpecificContent { get; set; } = true;
    public List<string> SupportedLanguages { get; set; } = new() { "en", "es" };
    public string DefaultLanguage { get; set; } = "en";

    public static AudioResourceConfiguration Default => new();
}

/// <summary>
/// Audio resource entry with metadata.
/// </summary>
public class AudioResourceEntry
{
    public string ResourceKey { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public AgeGroup AgeGroup { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime LastAccessed { get; set; }
    public int AccessCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Language-specific audio pack information.
/// </summary>
public class LanguageAudioPack
{
    public string LanguageCode { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public int ResourceCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Version { get; set; } = "1.0.0";
}

/// <summary>
/// Age group specific audio settings.
/// </summary>
public class AgeGroupAudioSettings
{
    public AgeGroup AgeGroup { get; set; }
    public float PreferredSpeechRate { get; set; } = 1.0f;
    public float MaxVolumeLevel { get; set; } = 0.85f;
    public bool RequiresSlowSpeech { get; set; }
    public bool EnableRepetition { get; set; }
    public bool SimplifiedVocabulary { get; set; }
    public TimeSpan DefaultPauseDuration { get; set; } = TimeSpan.FromMilliseconds(1000);
}

/// <summary>
/// Audio resource validation result.
/// </summary>
public class AudioResourceValidationResult
{
    public string Language { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public int TotalEssentialResources { get; set; }
    public int AvailableResources { get; set; }
    public List<string> MissingResources { get; set; } = new();
    public DateTime ValidationTime { get; set; }
    public string? ErrorMessage { get; set; }
}

#endregion