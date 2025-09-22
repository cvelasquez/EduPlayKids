namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Provides information about the current state and usage of the audio cache system.
/// Used for monitoring memory usage, optimizing performance, and managing storage resources.
/// </summary>
public class AudioCacheInfo
{
    /// <summary>
    /// Gets or sets the total number of audio files currently cached in memory.
    /// </summary>
    public int CachedFileCount { get; set; }

    /// <summary>
    /// Gets or sets the total size of cached audio data in bytes.
    /// </summary>
    public long TotalCacheSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed cache size in bytes.
    /// </summary>
    public long MaxCacheSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the number of cache hits (successful retrievals from cache).
    /// </summary>
    public long CacheHits { get; set; }

    /// <summary>
    /// Gets or sets the number of cache misses (files not found in cache).
    /// </summary>
    public long CacheMisses { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last cache cleanup operation.
    /// </summary>
    public DateTime LastCleanup { get; set; }

    /// <summary>
    /// Gets or sets information about individual cached audio items.
    /// </summary>
    public List<CachedAudioInfo> CachedItems { get; set; }

    /// <summary>
    /// Gets or sets cache performance metrics.
    /// </summary>
    public CachePerformanceMetrics Performance { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioCacheInfo class.
    /// </summary>
    public AudioCacheInfo()
    {
        CachedItems = new List<CachedAudioInfo>();
        Performance = new CachePerformanceMetrics();
        LastCleanup = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the cache hit ratio as a percentage (0.0 to 1.0).
    /// Higher values indicate better cache performance.
    /// </summary>
    public double CacheHitRatio
    {
        get
        {
            var totalRequests = CacheHits + CacheMisses;
            return totalRequests > 0 ? (double)CacheHits / totalRequests : 0.0;
        }
    }

    /// <summary>
    /// Gets the cache utilization as a percentage (0.0 to 1.0).
    /// Indicates how much of the available cache space is being used.
    /// </summary>
    public double CacheUtilization
    {
        get
        {
            return MaxCacheSizeBytes > 0 ? (double)TotalCacheSizeBytes / MaxCacheSizeBytes : 0.0;
        }
    }

    /// <summary>
    /// Gets the available cache space in bytes.
    /// </summary>
    public long AvailableCacheBytes => Math.Max(0, MaxCacheSizeBytes - TotalCacheSizeBytes);

    /// <summary>
    /// Gets whether the cache is nearly full (>90% utilization).
    /// </summary>
    public bool IsNearlyFull => CacheUtilization > 0.9;

    /// <summary>
    /// Gets whether cache cleanup is recommended based on usage patterns.
    /// </summary>
    public bool CleanupRecommended
    {
        get
        {
            // Recommend cleanup if cache is nearly full or hasn't been cleaned in over an hour
            return IsNearlyFull || DateTime.UtcNow - LastCleanup > TimeSpan.FromHours(1);
        }
    }

    /// <summary>
    /// Gets the most frequently accessed audio items.
    /// </summary>
    /// <param name="count">Number of top items to return</param>
    /// <returns>Audio items sorted by access frequency</returns>
    public IEnumerable<CachedAudioInfo> GetMostFrequentlyAccessed(int count = 10)
    {
        return CachedItems
            .OrderByDescending(item => item.AccessCount)
            .Take(count);
    }

    /// <summary>
    /// Gets the largest cached audio items by size.
    /// </summary>
    /// <param name="count">Number of largest items to return</param>
    /// <returns>Audio items sorted by size descending</returns>
    public IEnumerable<CachedAudioInfo> GetLargestItems(int count = 10)
    {
        return CachedItems
            .OrderByDescending(item => item.SizeBytes)
            .Take(count);
    }

    /// <summary>
    /// Gets cached items that haven't been accessed recently.
    /// </summary>
    /// <param name="olderThan">Time threshold for considering items stale</param>
    /// <returns>Audio items that haven't been accessed within the specified time</returns>
    public IEnumerable<CachedAudioInfo> GetStaleItems(TimeSpan olderThan)
    {
        var cutoffTime = DateTime.UtcNow - olderThan;
        return CachedItems
            .Where(item => item.LastAccessed < cutoffTime)
            .OrderBy(item => item.LastAccessed);
    }

    /// <summary>
    /// Gets cached items filtered by audio type.
    /// </summary>
    /// <param name="audioType">Type of audio to filter by</param>
    /// <returns>Cached items of the specified audio type</returns>
    public IEnumerable<CachedAudioInfo> GetItemsByType(AudioType audioType)
    {
        return CachedItems.Where(item => item.AudioType == audioType);
    }

    /// <summary>
    /// Estimates potential memory savings from cache cleanup.
    /// </summary>
    /// <param name="staleThreshold">Age threshold for considering items for cleanup</param>
    /// <returns>Estimated bytes that could be freed</returns>
    public long EstimateCleanupSavings(TimeSpan staleThreshold)
    {
        return GetStaleItems(staleThreshold).Sum(item => item.SizeBytes);
    }

    /// <summary>
    /// Gets a summary of cache usage by audio type.
    /// </summary>
    /// <returns>Dictionary with audio type usage statistics</returns>
    public Dictionary<AudioType, CacheTypeStatistics> GetUsageByType()
    {
        return CachedItems
            .GroupBy(item => item.AudioType)
            .ToDictionary(
                group => group.Key,
                group => new CacheTypeStatistics
                {
                    ItemCount = group.Count(),
                    TotalSizeBytes = group.Sum(item => item.SizeBytes),
                    TotalAccessCount = group.Sum(item => item.AccessCount),
                    AverageFileSize = group.Average(item => item.SizeBytes),
                    LastAccessed = group.Max(item => item.LastAccessed)
                });
    }

    /// <summary>
    /// Returns a human-readable summary of cache status.
    /// </summary>
    /// <returns>String description of cache status</returns>
    public override string ToString()
    {
        var sizeMB = TotalCacheSizeBytes / (1024.0 * 1024.0);
        var maxSizeMB = MaxCacheSizeBytes / (1024.0 * 1024.0);

        return $"AudioCache: {CachedFileCount} files, {sizeMB:F1}MB/{maxSizeMB:F1}MB ({CacheUtilization:P1}), " +
               $"Hit ratio: {CacheHitRatio:P1}";
    }
}

/// <summary>
/// Information about an individual cached audio item.
/// </summary>
public class CachedAudioInfo
{
    /// <summary>
    /// Gets or sets the cache key for this audio item.
    /// </summary>
    public string CacheKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original file path or localization key.
    /// </summary>
    public string OriginalPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of audio content.
    /// </summary>
    public AudioType AudioType { get; set; }

    /// <summary>
    /// Gets or sets the size of this cached item in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the duration of the audio in milliseconds.
    /// </summary>
    public int DurationMs { get; set; }

    /// <summary>
    /// Gets or sets when this item was first cached.
    /// </summary>
    public DateTime CachedAt { get; set; }

    /// <summary>
    /// Gets or sets when this item was last accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; }

    /// <summary>
    /// Gets or sets the number of times this item has been accessed.
    /// </summary>
    public long AccessCount { get; set; }

    /// <summary>
    /// Gets or sets the language of this audio content (if applicable).
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets whether this item is marked as high priority for caching.
    /// </summary>
    public bool HighPriority { get; set; }

    /// <summary>
    /// Gets the age of this cached item.
    /// </summary>
    public TimeSpan Age => DateTime.UtcNow - CachedAt;

    /// <summary>
    /// Gets the time since this item was last accessed.
    /// </summary>
    public TimeSpan TimeSinceLastAccess => DateTime.UtcNow - LastAccessed;

    /// <summary>
    /// Gets the access frequency (accesses per hour since cached).
    /// </summary>
    public double AccessFrequency
    {
        get
        {
            var hoursAlive = Math.Max(1, Age.TotalHours);
            return AccessCount / hoursAlive;
        }
    }

    /// <summary>
    /// Updates access statistics for this cached item.
    /// </summary>
    public void RecordAccess()
    {
        LastAccessed = DateTime.UtcNow;
        AccessCount++;
    }

    /// <summary>
    /// Returns a string representation of this cached audio item.
    /// </summary>
    /// <returns>String description of the cached item</returns>
    public override string ToString()
    {
        var sizeMB = SizeBytes / (1024.0 * 1024.0);
        return $"CachedAudio[{AudioType}]: {Path.GetFileName(OriginalPath)} " +
               $"({sizeMB:F2}MB, {AccessCount} accesses, {TimeSinceLastAccess.TotalMinutes:F0}min ago)";
    }
}

/// <summary>
/// Performance metrics for cache operations.
/// </summary>
public class CachePerformanceMetrics
{
    /// <summary>
    /// Gets or sets the average time to load an audio file from cache in milliseconds.
    /// </summary>
    public double AverageCacheLoadTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the average time to load an audio file from disk in milliseconds.
    /// </summary>
    public double AverageDiskLoadTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the total number of cache operations performed.
    /// </summary>
    public long TotalCacheOperations { get; set; }

    /// <summary>
    /// Gets or sets the total time spent on cache operations in milliseconds.
    /// </summary>
    public long TotalCacheTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the number of cache evictions (items removed due to memory pressure).
    /// </summary>
    public long CacheEvictions { get; set; }

    /// <summary>
    /// Gets or sets the peak cache size reached during this session.
    /// </summary>
    public long PeakCacheSizeBytes { get; set; }

    /// <summary>
    /// Gets the cache performance factor (how much faster cache is vs disk).
    /// </summary>
    public double PerformanceFactor
    {
        get
        {
            return AverageCacheLoadTimeMs > 0 && AverageDiskLoadTimeMs > 0
                ? AverageDiskLoadTimeMs / AverageCacheLoadTimeMs
                : 1.0;
        }
    }

    /// <summary>
    /// Gets the average cache operation time in milliseconds.
    /// </summary>
    public double AverageOperationTimeMs
    {
        get
        {
            return TotalCacheOperations > 0
                ? (double)TotalCacheTimeMs / TotalCacheOperations
                : 0.0;
        }
    }

    /// <summary>
    /// Records a cache operation for performance tracking.
    /// </summary>
    /// <param name="operationTimeMs">Time taken for the operation in milliseconds</param>
    /// <param name="wasFromCache">Whether the operation retrieved data from cache</param>
    public void RecordOperation(double operationTimeMs, bool wasFromCache)
    {
        TotalCacheOperations++;
        TotalCacheTimeMs += (long)operationTimeMs;

        if (wasFromCache)
        {
            // Update running average for cache load time
            var currentWeight = Math.Min(TotalCacheOperations - 1, 100);
            AverageCacheLoadTimeMs = (AverageCacheLoadTimeMs * currentWeight + operationTimeMs) / (currentWeight + 1);
        }
        else
        {
            // Update running average for disk load time
            var currentWeight = Math.Min(TotalCacheOperations - 1, 100);
            AverageDiskLoadTimeMs = (AverageDiskLoadTimeMs * currentWeight + operationTimeMs) / (currentWeight + 1);
        }
    }
}

/// <summary>
/// Statistics for cached items grouped by audio type.
/// </summary>
public class CacheTypeStatistics
{
    /// <summary>
    /// Gets or sets the number of cached items of this type.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Gets or sets the total size in bytes for this audio type.
    /// </summary>
    public long TotalSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the total number of accesses for this audio type.
    /// </summary>
    public long TotalAccessCount { get; set; }

    /// <summary>
    /// Gets or sets the average file size for this audio type.
    /// </summary>
    public double AverageFileSize { get; set; }

    /// <summary>
    /// Gets or sets when items of this type were last accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; }

    /// <summary>
    /// Gets the average access count per item.
    /// </summary>
    public double AverageAccessCount => ItemCount > 0 ? (double)TotalAccessCount / ItemCount : 0.0;

    /// <summary>
    /// Gets the percentage of total cache space used by this audio type.
    /// </summary>
    /// <param name="totalCacheSize">Total cache size in bytes</param>
    /// <returns>Percentage of cache space used (0.0 to 1.0)</returns>
    public double GetCacheSpacePercentage(long totalCacheSize)
    {
        return totalCacheSize > 0 ? (double)TotalSizeBytes / totalCacheSize : 0.0;
    }
}