using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Manages audio resources for the EduPlayKids application with child-friendly optimization.
/// Handles audio file discovery, caching, preloading, and resource cleanup for educational content.
/// </summary>
public class AudioResourceManager : IDisposable
{
    #region Private Fields

    private readonly ILogger<AudioResourceManager> _logger;
    private readonly AudioConfiguration _configuration;
    private readonly ConcurrentDictionary<string, AudioResourceInfo> _audioResources;
    private readonly ConcurrentDictionary<string, byte[]> _audioCache;
    private readonly SemaphoreSlim _resourceLock;
    private readonly Timer _cacheMaintenanceTimer;

    private bool _disposed;
    private long _totalCacheSize;
    private DateTime _lastCacheCleanup;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the AudioResourceManager class.
    /// </summary>
    /// <param name="logger">Logger for debugging and monitoring</param>
    /// <param name="configuration">Audio configuration settings</param>
    public AudioResourceManager(
        ILogger<AudioResourceManager> logger,
        AudioConfiguration? configuration = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? AudioConfiguration.Default;

        _audioResources = new ConcurrentDictionary<string, AudioResourceInfo>();
        _audioCache = new ConcurrentDictionary<string, byte[]>();
        _resourceLock = new SemaphoreSlim(1, 1);
        _lastCacheCleanup = DateTime.UtcNow;

        // Start cache maintenance timer (runs every 5 minutes)
        _cacheMaintenanceTimer = new Timer(PerformCacheMaintenance, null,
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

        _logger.LogInformation("AudioResourceManager initialized with max cache size: {MaxSize}MB",
            _configuration.MaxCacheSizeBytes / (1024.0 * 1024.0));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the audio resource catalog by scanning available audio files.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of audio resources discovered</returns>
    public async Task<int> InitializeResourceCatalogAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _resourceLock.WaitAsync(cancellationToken);

            _logger.LogInformation("Initializing audio resource catalog...");

            var resourceCount = 0;

            // Scan embedded audio resources
            resourceCount += await ScanEmbeddedAudioResourcesAsync(cancellationToken);

            // Scan local audio files if any
            resourceCount += await ScanLocalAudioFilesAsync(cancellationToken);

            // Load resource metadata if available
            await LoadResourceMetadataAsync(cancellationToken);

            _logger.LogInformation("Audio resource catalog initialized with {Count} resources", resourceCount);

            return resourceCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing audio resource catalog");
            return 0;
        }
        finally
        {
            _resourceLock.Release();
        }
    }

    /// <summary>
    /// Gets information about a specific audio resource.
    /// </summary>
    /// <param name="audioKey">Audio resource key</param>
    /// <param name="language">Optional language preference</param>
    /// <returns>Audio resource information if found</returns>
    public AudioResourceInfo? GetAudioResource(string audioKey, string? language = null)
    {
        if (string.IsNullOrEmpty(audioKey)) return null;

        var resourceKey = BuildResourceKey(audioKey, language ?? "en");

        if (_audioResources.TryGetValue(resourceKey, out var resource))
        {
            resource.LastAccessed = DateTime.UtcNow;
            resource.AccessCount++;
            return resource;
        }

        // Try fallback to English if requested language not found
        if (!string.IsNullOrEmpty(language) && language != "en")
        {
            var fallbackKey = BuildResourceKey(audioKey, "en");
            if (_audioResources.TryGetValue(fallbackKey, out var fallbackResource))
            {
                fallbackResource.LastAccessed = DateTime.UtcNow;
                fallbackResource.AccessCount++;
                _logger.LogDebug("Using English fallback for audio key: {AudioKey}", audioKey);
                return fallbackResource;
            }
        }

        _logger.LogWarning("Audio resource not found: {AudioKey} (language: {Language})", audioKey, language);
        return null;
    }

    /// <summary>
    /// Preloads audio resources into cache for faster access.
    /// </summary>
    /// <param name="audioKeys">Collection of audio keys to preload</param>
    /// <param name="language">Language preference for preloading</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary indicating which resources were successfully preloaded</returns>
    public async Task<Dictionary<string, bool>> PreloadAudioResourcesAsync(
        IEnumerable<string> audioKeys,
        string language = "en",
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, bool>();

        try
        {
            var preloadTasks = audioKeys.Select(async audioKey =>
            {
                try
                {
                    var success = await LoadResourceIntoCacheAsync(audioKey, language, cancellationToken);
                    results[audioKey] = success;
                    return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error preloading audio resource: {AudioKey}", audioKey);
                    results[audioKey] = false;
                    return false;
                }
            });

            var preloadResults = await Task.WhenAll(preloadTasks);
            var successCount = preloadResults.Count(r => r);

            _logger.LogInformation("Preloaded {Success}/{Total} audio resources for language: {Language}",
                successCount, audioKeys.Count(), language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during audio resource preloading");
        }

        return results;
    }

    /// <summary>
    /// Gets cached audio data for a resource.
    /// </summary>
    /// <param name="audioKey">Audio resource key</param>
    /// <param name="language">Language preference</param>
    /// <returns>Cached audio data if available</returns>
    public byte[]? GetCachedAudioData(string audioKey, string language = "en")
    {
        var cacheKey = BuildCacheKey(audioKey, language);

        if (_audioCache.TryGetValue(cacheKey, out var audioData))
        {
            // Update access statistics
            var resourceKey = BuildResourceKey(audioKey, language);
            if (_audioResources.TryGetValue(resourceKey, out var resource))
            {
                resource.LastAccessed = DateTime.UtcNow;
                resource.AccessCount++;
            }

            _logger.LogDebug("Retrieved cached audio data: {AudioKey} ({Size} bytes)", audioKey, audioData.Length);
            return audioData;
        }

        return null;
    }

    /// <summary>
    /// Loads audio data for a resource, either from cache or file system.
    /// </summary>
    /// <param name="audioKey">Audio resource key</param>
    /// <param name="language">Language preference</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audio data if successfully loaded</returns>
    public async Task<byte[]?> LoadAudioDataAsync(
        string audioKey,
        string language = "en",
        CancellationToken cancellationToken = default)
    {
        // Try cache first
        var cachedData = GetCachedAudioData(audioKey, language);
        if (cachedData != null)
        {
            return cachedData;
        }

        // Load from file system
        var resource = GetAudioResource(audioKey, language);
        if (resource == null)
        {
            return null;
        }

        try
        {
            byte[] audioData;

            if (resource.IsEmbedded)
            {
                audioData = await LoadEmbeddedAudioDataAsync(resource.FilePath, cancellationToken);
            }
            else
            {
                audioData = await File.ReadAllBytesAsync(resource.FilePath, cancellationToken);
            }

            // Cache the data if enabled and within size limits
            if (_configuration.EnableAudioPreloading && ShouldCacheResource(resource, audioData.Length))
            {
                await CacheAudioDataAsync(audioKey, language, audioData);
            }

            _logger.LogDebug("Loaded audio data: {AudioKey} ({Size} bytes, cached: {Cached})",
                audioKey, audioData.Length, _audioCache.ContainsKey(BuildCacheKey(audioKey, language)));

            return audioData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading audio data: {AudioKey}", audioKey);
            return null;
        }
    }

    /// <summary>
    /// Clears the audio cache to free up memory.
    /// </summary>
    /// <param name="keepFrequentlyUsed">Whether to keep frequently accessed items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Amount of memory freed in bytes</returns>
    public async Task<long> ClearAudioCacheAsync(bool keepFrequentlyUsed = true, CancellationToken cancellationToken = default)
    {
        try
        {
            await _resourceLock.WaitAsync(cancellationToken);

            var freedBytes = 0L;
            var itemsToRemove = new List<string>();

            foreach (var kvp in _audioCache)
            {
                var shouldRemove = true;

                if (keepFrequentlyUsed)
                {
                    // Keep frequently accessed items (accessed more than 5 times or within last hour)
                    var parts = kvp.Key.Split('_');
                    if (parts.Length >= 2)
                    {
                        var resourceKey = string.Join("_", parts.Take(parts.Length - 1));
                        if (_audioResources.TryGetValue(resourceKey, out var resource))
                        {
                            if (resource.AccessCount > 5 || DateTime.UtcNow - resource.LastAccessed < TimeSpan.FromHours(1))
                            {
                                shouldRemove = false;
                            }
                        }
                    }
                }

                if (shouldRemove)
                {
                    freedBytes += kvp.Value.Length;
                    itemsToRemove.Add(kvp.Key);
                }
            }

            // Remove items from cache
            foreach (var key in itemsToRemove)
            {
                _audioCache.TryRemove(key, out _);
            }

            _totalCacheSize -= freedBytes;
            _lastCacheCleanup = DateTime.UtcNow;

            _logger.LogInformation("Audio cache cleared: {FreedMB}MB freed, {RemainingItems} items remaining",
                freedBytes / (1024.0 * 1024.0), _audioCache.Count);

            return freedBytes;
        }
        finally
        {
            _resourceLock.Release();
        }
    }

    /// <summary>
    /// Gets comprehensive cache statistics.
    /// </summary>
    /// <returns>Audio cache information</returns>
    public AudioCacheInfo GetCacheInfo()
    {
        var cacheInfo = new AudioCacheInfo
        {
            CachedFileCount = _audioCache.Count,
            TotalCacheSizeBytes = _totalCacheSize,
            MaxCacheSizeBytes = _configuration.MaxCacheSizeBytes,
            LastCleanup = _lastCacheCleanup,
            CachedItems = new List<CachedAudioInfo>()
        };

        // Build detailed cache information
        foreach (var kvp in _audioCache)
        {
            var parts = kvp.Key.Split('_');
            if (parts.Length >= 2)
            {
                var language = parts[^1];
                var audioKey = string.Join("_", parts.Take(parts.Length - 1));
                var resourceKey = BuildResourceKey(audioKey, language);

                var cachedInfo = new CachedAudioInfo
                {
                    CacheKey = kvp.Key,
                    OriginalPath = audioKey,
                    SizeBytes = kvp.Value.Length,
                    Language = language,
                    CachedAt = DateTime.UtcNow // Simplified - would track actual cache time
                };

                if (_audioResources.TryGetValue(resourceKey, out var resource))
                {
                    cachedInfo.LastAccessed = resource.LastAccessed;
                    cachedInfo.AccessCount = resource.AccessCount;
                    cachedInfo.DurationMs = resource.EstimatedDurationMs;
                    cachedInfo.AudioType = resource.AudioType;
                    cachedInfo.HighPriority = resource.Priority == AudioPriority.Critical || resource.Priority == AudioPriority.High;
                }

                cacheInfo.CachedItems.Add(cachedInfo);
            }
        }

        return cacheInfo;
    }

    /// <summary>
    /// Gets all available audio resources for a specific type.
    /// </summary>
    /// <param name="audioType">Type of audio to filter by</param>
    /// <param name="language">Language preference</param>
    /// <returns>Collection of matching audio resources</returns>
    public IEnumerable<AudioResourceInfo> GetResourcesByType(AudioType audioType, string language = "en")
    {
        return _audioResources.Values
            .Where(r => r.AudioType == audioType && r.Language == language)
            .OrderBy(r => r.AudioKey);
    }

    /// <summary>
    /// Validates that a resource exists and is accessible.
    /// </summary>
    /// <param name="audioKey">Audio resource key</param>
    /// <param name="language">Language preference</param>
    /// <returns>True if resource is valid and accessible</returns>
    public async Task<bool> ValidateResourceAsync(string audioKey, string language = "en")
    {
        var resource = GetAudioResource(audioKey, language);
        if (resource == null) return false;

        try
        {
            if (resource.IsEmbedded)
            {
                using var stream = await GetEmbeddedResourceStreamAsync(resource.FilePath);
                return stream != null && stream.Length > 0;
            }
            else
            {
                return File.Exists(resource.FilePath) && new FileInfo(resource.FilePath).Length > 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating audio resource: {AudioKey}", audioKey);
            return false;
        }
    }

    #endregion

    #region Private Methods

    private async Task<int> ScanEmbeddedAudioResourcesAsync(CancellationToken cancellationToken)
    {
        var resourceCount = 0;

        try
        {
            // This would scan embedded resources in the application assembly
            // For now, we'll simulate some common educational audio resources

            var commonResources = new[]
            {
                // UI Sounds
                ("ui_button_press", AudioType.UIInteraction, 200),
                ("ui_page_transition", AudioType.UIInteraction, 300),
                ("ui_item_select", AudioType.UIInteraction, 150),

                // Success Feedback
                ("feedback_success_soft", AudioType.SuccessFeedback, 1000),
                ("feedback_success_medium", AudioType.SuccessFeedback, 1500),
                ("feedback_success_celebration", AudioType.SuccessFeedback, 2000),

                // Error Feedback
                ("feedback_error_gentle", AudioType.ErrorFeedback, 800),
                ("feedback_error_encouraging", AudioType.ErrorFeedback, 1200),

                // Instructions
                ("age_selection_welcome", AudioType.Instruction, 3000),
                ("activity_intro_mathematics", AudioType.Instruction, 4000),
                ("activity_intro_reading", AudioType.Instruction, 4000),

                // Completion
                ("completion_one_star", AudioType.Completion, 2000),
                ("completion_two_stars", AudioType.Completion, 2500),
                ("completion_three_stars", AudioType.Completion, 3000),

                // Background Music
                ("background_music_mathematics", AudioType.BackgroundMusic, 120000),
                ("background_music_reading", AudioType.BackgroundMusic, 120000)
            };

            foreach (var language in new[] { "en", "es" })
            {
                foreach (var (audioKey, audioType, duration) in commonResources)
                {
                    var resourceKey = BuildResourceKey(audioKey, language);
                    var filePath = $"Audio/{language}/{audioKey}.mp3";

                    var resource = new AudioResourceInfo
                    {
                        AudioKey = audioKey,
                        Language = language,
                        FilePath = filePath,
                        AudioType = audioType,
                        IsEmbedded = true,
                        EstimatedDurationMs = duration,
                        Priority = GetDefaultPriority(audioType),
                        EstimatedSizeBytes = EstimateFileSize(audioType, duration),
                        CreatedAt = DateTime.UtcNow,
                        LastAccessed = DateTime.UtcNow
                    };

                    _audioResources[resourceKey] = resource;
                    resourceCount++;
                }
            }

            _logger.LogDebug("Scanned {Count} embedded audio resources", resourceCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning embedded audio resources");
        }

        return resourceCount;
    }

    private async Task<int> ScanLocalAudioFilesAsync(CancellationToken cancellationToken)
    {
        // This would scan for local audio files in the app's documents directory
        // For educational apps, this might include user-generated content or downloaded assets

        var resourceCount = 0;

        try
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var audioPath = Path.Combine(documentsPath, "Audio");

            if (Directory.Exists(audioPath))
            {
                var audioFiles = Directory.GetFiles(audioPath, "*.mp3", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(audioPath, "*.wav", SearchOption.AllDirectories))
                    .Concat(Directory.GetFiles(audioPath, "*.m4a", SearchOption.AllDirectories));

                foreach (var filePath in audioFiles)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    try
                    {
                        var fileInfo = new FileInfo(filePath);
                        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        var language = DetermineLanguageFromPath(filePath);
                        var audioType = DetermineAudioTypeFromFileName(fileName);

                        var resourceKey = BuildResourceKey(fileName, language);

                        var resource = new AudioResourceInfo
                        {
                            AudioKey = fileName,
                            Language = language,
                            FilePath = filePath,
                            AudioType = audioType,
                            IsEmbedded = false,
                            EstimatedSizeBytes = fileInfo.Length,
                            Priority = GetDefaultPriority(audioType),
                            CreatedAt = fileInfo.CreationTime,
                            LastAccessed = DateTime.UtcNow
                        };

                        _audioResources[resourceKey] = resource;
                        resourceCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing audio file: {FilePath}", filePath);
                    }
                }
            }

            _logger.LogDebug("Scanned {Count} local audio files", resourceCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning local audio files");
        }

        return resourceCount;
    }

    private async Task LoadResourceMetadataAsync(CancellationToken cancellationToken)
    {
        try
        {
            // This would load metadata about audio resources from a manifest file
            // For now, we'll simulate loading additional metadata

            foreach (var resource in _audioResources.Values)
            {
                // Update estimated duration for more accurate values
                if (resource.AudioType == AudioType.BackgroundMusic)
                {
                    resource.EstimatedDurationMs = 120000; // 2 minutes
                }
                else if (resource.AudioType == AudioType.Instruction)
                {
                    resource.EstimatedDurationMs = resource.AudioKey.Contains("intro") ? 5000 : 3000;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Resource metadata loading cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading resource metadata");
        }
    }

    private async Task<bool> LoadResourceIntoCacheAsync(string audioKey, string language, CancellationToken cancellationToken)
    {
        try
        {
            var audioData = await LoadAudioDataAsync(audioKey, language, cancellationToken);
            return audioData != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading resource into cache: {AudioKey}", audioKey);
            return false;
        }
    }

    private async Task<byte[]> LoadEmbeddedAudioDataAsync(string resourcePath, CancellationToken cancellationToken)
    {
        // This would load data from embedded resources
        // For simulation, we'll create placeholder data
        var estimatedSize = 50 * 1024; // 50KB placeholder
        return new byte[estimatedSize];
    }

    private async Task<Stream?> GetEmbeddedResourceStreamAsync(string resourcePath)
    {
        try
        {
            // This would get an embedded resource stream
            // For simulation, return a memory stream with placeholder data
            var data = new byte[50 * 1024]; // 50KB placeholder
            return new MemoryStream(data);
        }
        catch
        {
            return null;
        }
    }

    private async Task CacheAudioDataAsync(string audioKey, string language, byte[] audioData)
    {
        var cacheKey = BuildCacheKey(audioKey, language);

        // Check cache size limits
        if (_totalCacheSize + audioData.Length > _configuration.MaxCacheSizeBytes)
        {
            await ClearAudioCacheAsync(keepFrequentlyUsed: true);
        }

        _audioCache[cacheKey] = audioData;
        _totalCacheSize += audioData.Length;

        _logger.LogDebug("Cached audio data: {AudioKey} ({Size} bytes)", audioKey, audioData.Length);
    }

    private bool ShouldCacheResource(AudioResourceInfo resource, int dataSize)
    {
        // Don't cache if it would exceed size limits
        if (_totalCacheSize + dataSize > _configuration.MaxCacheSizeBytes)
        {
            return false;
        }

        // Always cache high-priority small files
        if (resource.Priority == AudioPriority.Critical && dataSize < 100 * 1024)
        {
            return true;
        }

        // Cache frequently accessed files
        if (resource.AccessCount > 3)
        {
            return true;
        }

        // Cache recently accessed files under 500KB
        if (DateTime.UtcNow - resource.LastAccessed < TimeSpan.FromMinutes(30) && dataSize < 500 * 1024)
        {
            return true;
        }

        return false;
    }

    private void PerformCacheMaintenance(object? state)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                if (DateTime.UtcNow - _lastCacheCleanup > TimeSpan.FromMinutes(_configuration.CacheCleanupIntervalMinutes))
                {
                    await ClearAudioCacheAsync(keepFrequentlyUsed: true);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache maintenance");
        }
    }

    private static string BuildResourceKey(string audioKey, string language)
    {
        return $"{audioKey}_{language}";
    }

    private static string BuildCacheKey(string audioKey, string language)
    {
        return $"{audioKey}_{language}";
    }

    private static string DetermineLanguageFromPath(string filePath)
    {
        if (filePath.Contains("/en/") || filePath.Contains("\\en\\"))
            return "en";
        if (filePath.Contains("/es/") || filePath.Contains("\\es\\"))
            return "es";
        return "en"; // Default
    }

    private static AudioType DetermineAudioTypeFromFileName(string fileName)
    {
        var lowerName = fileName.ToLowerInvariant();

        if (lowerName.Contains("ui_") || lowerName.Contains("button") || lowerName.Contains("click"))
            return AudioType.UIInteraction;
        if (lowerName.Contains("success") || lowerName.Contains("correct") || lowerName.Contains("good"))
            return AudioType.SuccessFeedback;
        if (lowerName.Contains("error") || lowerName.Contains("wrong") || lowerName.Contains("try"))
            return AudioType.ErrorFeedback;
        if (lowerName.Contains("complete") || lowerName.Contains("finish") || lowerName.Contains("star"))
            return AudioType.Completion;
        if (lowerName.Contains("background") || lowerName.Contains("music"))
            return AudioType.BackgroundMusic;
        if (lowerName.Contains("instruction") || lowerName.Contains("intro") || lowerName.Contains("welcome"))
            return AudioType.Instruction;
        if (lowerName.Contains("achievement") || lowerName.Contains("unlock"))
            return AudioType.Achievement;
        if (lowerName.Contains("leo") || lowerName.Contains("mascot"))
            return AudioType.Mascot;

        return AudioType.UIInteraction; // Default
    }

    private static AudioPriority GetDefaultPriority(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.Instruction => AudioPriority.Critical,
            AudioType.SuccessFeedback => AudioPriority.High,
            AudioType.ErrorFeedback => AudioPriority.High,
            AudioType.Completion => AudioPriority.High,
            AudioType.Achievement => AudioPriority.High,
            AudioType.UIInteraction => AudioPriority.Normal,
            AudioType.Mascot => AudioPriority.Normal,
            AudioType.BackgroundMusic => AudioPriority.Low,
            _ => AudioPriority.Normal
        };
    }

    private static long EstimateFileSize(AudioType audioType, int durationMs)
    {
        // Rough estimates for MP3 files at different quality levels
        var bytesPerSecond = audioType switch
        {
            AudioType.BackgroundMusic => 16000, // 128 kbps
            AudioType.Instruction => 12000,     // 96 kbps
            AudioType.Mascot => 12000,          // 96 kbps
            _ => 8000                           // 64 kbps for short sounds
        };

        return (long)(durationMs / 1000.0 * bytesPerSecond);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _cacheMaintenanceTimer?.Dispose();
            _resourceLock?.Dispose();

            _audioCache.Clear();
            _audioResources.Clear();

            _disposed = true;
            _logger.LogDebug("AudioResourceManager disposed");
        }
    }

    #endregion
}

/// <summary>
/// Information about an audio resource in the system.
/// </summary>
public class AudioResourceInfo
{
    /// <summary>
    /// Gets or sets the audio resource key.
    /// </summary>
    public string AudioKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the language of this resource.
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Gets or sets the file path to the audio resource.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of audio content.
    /// </summary>
    public AudioType AudioType { get; set; } = AudioType.UIInteraction;

    /// <summary>
    /// Gets or sets whether this is an embedded resource.
    /// </summary>
    public bool IsEmbedded { get; set; } = true;

    /// <summary>
    /// Gets or sets the estimated duration in milliseconds.
    /// </summary>
    public int EstimatedDurationMs { get; set; }

    /// <summary>
    /// Gets or sets the estimated file size in bytes.
    /// </summary>
    public long EstimatedSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the priority of this audio resource.
    /// </summary>
    public AudioPriority Priority { get; set; } = AudioPriority.Normal;

    /// <summary>
    /// Gets or sets when this resource was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this resource was last accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of times this resource has been accessed.
    /// </summary>
    public long AccessCount { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about this resource.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}