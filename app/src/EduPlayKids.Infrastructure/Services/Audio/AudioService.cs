using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Cross-platform audio service implementation for the EduPlayKids application.
/// Provides comprehensive audio functionality with child-friendly features, bilingual support,
/// and platform-optimized performance for educational content delivery.
/// </summary>
public class AudioService : IAudioService, IDisposable
{
    #region Private Fields

    private readonly ILogger<AudioService> _logger;
    private readonly IStringLocalizer<AudioService> _localizer;
    private readonly ConcurrentDictionary<string, IAudioPlayer> _activePlayers;
    private readonly ConcurrentDictionary<string, AudioItem> _audioCache;
    private readonly AudioConfiguration _configuration;
    private readonly SemaphoreSlim _operationLock;
    private readonly Timer _cacheCleanupTimer;

    private string _currentLanguage;
    private bool _disposed;
    private AudioCacheInfo _cacheInfo;
    private readonly Dictionary<AudioType, float> _volumeLevels;
    private readonly Dictionary<AudioType, AudioPlaybackState> _playbackStates;

    #endregion

    #region Events

    public event EventHandler<AudioPlaybackEventArgs>? AudioStarted;
    public event EventHandler<AudioPlaybackEventArgs>? AudioStopped;
    public event EventHandler<AudioPlaybackEventArgs>? AudioPaused;
    public event EventHandler<AudioPlaybackEventArgs>? AudioResumed;
    public event EventHandler<AudioErrorEventArgs>? AudioError;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the AudioService class.
    /// </summary>
    /// <param name="logger">Logger for debugging and error tracking</param>
    /// <param name="localizer">String localizer for bilingual support</param>
    /// <param name="configuration">Audio configuration settings</param>
    public AudioService(
        ILogger<AudioService> logger,
        IStringLocalizer<AudioService> localizer,
        AudioConfiguration? configuration = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _configuration = configuration ?? AudioConfiguration.Default;

        _activePlayers = new ConcurrentDictionary<string, IAudioPlayer>();
        _audioCache = new ConcurrentDictionary<string, AudioItem>();
        _operationLock = new SemaphoreSlim(1, 1);
        _cacheInfo = new AudioCacheInfo();
        _volumeLevels = new Dictionary<AudioType, float>();
        _playbackStates = new Dictionary<AudioType, AudioPlaybackState>();

        // Initialize volume levels for all audio types
        InitializeVolumeSettings();

        // Set default language from system
        _currentLanguage = GetSystemLanguage();

        // Start cache cleanup timer (runs every 30 minutes)
        _cacheCleanupTimer = new Timer(PerformCacheCleanup, null,
            TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));

        _logger.LogInformation("AudioService initialized with language: {Language}", _currentLanguage);
    }

    #endregion

    #region Audio Playback Control

    public async Task<bool> PlayAudioAsync(AudioItem audioItem, CancellationToken cancellationToken = default)
    {
        if (audioItem == null)
        {
            _logger.LogWarning("Attempted to play null AudioItem");
            return false;
        }

        var validationErrors = audioItem.Validate();
        if (validationErrors.Any())
        {
            _logger.LogWarning("AudioItem validation failed: {Errors}", string.Join(", ", validationErrors));
            OnAudioError(audioItem, AudioErrorCode.ConfigurationError, "Invalid audio configuration", false);
            return false;
        }

        try
        {
            await _operationLock.WaitAsync(cancellationToken);

            // Resolve file path if using localization key
            var resolvedPath = await ResolveAudioPathAsync(audioItem, cancellationToken);
            if (string.IsNullOrEmpty(resolvedPath))
            {
                OnAudioError(audioItem, AudioErrorCode.FileNotFound, "Audio file not found", true);
                return false;
            }

            // Check if we need to interrupt lower priority audio
            if (audioItem.InterruptLowerPriority)
            {
                await InterruptLowerPriorityAudioAsync(audioItem.Priority);
            }

            // Create and configure audio player
            var player = CreateAudioPlayer(audioItem, resolvedPath);
            var playerId = GeneratePlayerId(audioItem);

            // Apply volume settings
            var effectiveVolume = CalculateEffectiveVolume(audioItem);
            await player.SetVolumeAsync(effectiveVolume);

            // Store player for management
            _activePlayers[playerId] = player;

            // Update playback state
            _playbackStates[audioItem.AudioType] = AudioPlaybackState.Playing;

            // Start playback
            var success = await player.PlayAsync(cancellationToken);

            if (success)
            {
                OnAudioStarted(audioItem, playerId);

                // Cache audio if requested
                if (audioItem.CacheAfterPlayback)
                {
                    await CacheAudioItemAsync(audioItem, resolvedPath);
                }

                _logger.LogDebug("Successfully started playback for {AudioType}: {Path}",
                    audioItem.AudioType, resolvedPath);
            }
            else
            {
                _activePlayers.TryRemove(playerId, out _);
                _playbackStates[audioItem.AudioType] = AudioPlaybackState.Error;
                OnAudioError(audioItem, AudioErrorCode.PlatformError, "Failed to start playback", true);
            }

            return success;
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Audio playback cancelled for {AudioType}", audioItem.AudioType);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing audio {AudioType}: {Path}",
                audioItem.AudioType, audioItem.FilePath);
            OnAudioError(audioItem, AudioErrorCode.Unknown, ex.Message, true);
            return false;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<bool> PlayAudioAsync(string filePath, AudioType audioType, AudioPriority priority = AudioPriority.Normal, CancellationToken cancellationToken = default)
    {
        var audioItem = new AudioItem(filePath, audioType)
        {
            Priority = priority,
            Language = _currentLanguage
        };

        return await PlayAudioAsync(audioItem, cancellationToken);
    }
    public async Task<bool> PlayWithVolumeAsync(AudioItem audioItem, float volume = 1.0f, CancellationToken cancellationToken = default)
    {
        if (audioItem != null)
        {
            // Apply child-safe volume limit (85% max for hearing protection)
            audioItem.Volume = Math.Min(volume, 0.85f);
        }
        return await PlayAudioAsync(audioItem, cancellationToken);
    }

    public async Task<bool> PlayAsync(AudioItem audioItem, CancellationToken cancellationToken = default)
    {
        return await PlayAudioAsync(audioItem, cancellationToken);
    }

    public async Task<bool> StopAsync()
    {
        await StopAudioAsync();
        return true;
    }

    public async Task StopAudioAsync(AudioType? audioType = null, int fadeOutDuration = 200)
    {
        try
        {
            await _operationLock.WaitAsync();

            var playersToStop = audioType.HasValue
                ? _activePlayers.Where(kvp => GetAudioTypeFromPlayerId(kvp.Key) == audioType.Value)
                : _activePlayers;

            var stopTasks = playersToStop.Select(async kvp =>
            {
                try
                {
                    await kvp.Value.StopAsync(fadeOutDuration);
                    _activePlayers.TryRemove(kvp.Key, out _);

                    var itemAudioType = GetAudioTypeFromPlayerId(kvp.Key);
                    _playbackStates[itemAudioType] = AudioPlaybackState.Stopped;

                    // Get audio item for event (this is simplified, in real implementation
                    // you'd track the original AudioItem)
                    var audioItem = new AudioItem("", itemAudioType);
                    OnAudioStopped(audioItem, kvp.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping audio player {PlayerId}", kvp.Key);
                }
            });

            await Task.WhenAll(stopTasks);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task PauseAudioAsync(AudioType? audioType = null)
    {
        try
        {
            await _operationLock.WaitAsync();

            var playersToPause = audioType.HasValue
                ? _activePlayers.Where(kvp => GetAudioTypeFromPlayerId(kvp.Key) == audioType.Value)
                : _activePlayers;

            foreach (var kvp in playersToPause)
            {
                try
                {
                    await kvp.Value.PauseAsync();

                    var itemAudioType = GetAudioTypeFromPlayerId(kvp.Key);
                    _playbackStates[itemAudioType] = AudioPlaybackState.Paused;

                    var audioItem = new AudioItem("", itemAudioType);
                    OnAudioPaused(audioItem, kvp.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error pausing audio player {PlayerId}", kvp.Key);
                }
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task ResumeAudioAsync(AudioType? audioType = null)
    {
        try
        {
            await _operationLock.WaitAsync();

            var playersToResume = audioType.HasValue
                ? _activePlayers.Where(kvp => GetAudioTypeFromPlayerId(kvp.Key) == audioType.Value)
                : _activePlayers.Where(kvp => kvp.Value.IsPaused);

            foreach (var kvp in playersToResume)
            {
                try
                {
                    await kvp.Value.ResumeAsync();

                    var itemAudioType = GetAudioTypeFromPlayerId(kvp.Key);
                    _playbackStates[itemAudioType] = AudioPlaybackState.Playing;

                    var audioItem = new AudioItem("", itemAudioType);
                    OnAudioResumed(audioItem, kvp.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error resuming audio player {PlayerId}", kvp.Key);
                }
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }

    #endregion

    #region Bilingual Audio Support

    public async Task<bool> PlayLocalizedAudioAsync(string audioKey, AudioType audioType, string? language = null, CancellationToken cancellationToken = default)
    {
        var audioItem = new AudioItem(audioKey, audioType)
        {
            LocalizationKey = audioKey,
            Language = language ?? _currentLanguage
        };

        return await PlayAudioAsync(audioItem, cancellationToken);
    }

    public string? GetLocalizedAudioPath(string audioKey, string? language = null)
    {
        var targetLanguage = language ?? _currentLanguage;

        // Build localized path: Assets/Audio/{language}/{audioKey}.mp3
        var localizedPath = Path.Combine("Audio", targetLanguage, $"{audioKey}.mp3");

        // Check if file exists in app bundle
        // TODO: Implement FileSystem access when moved to Presentation layer
        var fullPath = Path.Combine("mock_app_package", localizedPath);

        if (File.Exists(fullPath))
        {
            return localizedPath;
        }

        // Fallback to English if current language file not found
        if (targetLanguage != "en")
        {
            var fallbackPath = Path.Combine("Audio", "en", $"{audioKey}.mp3");
            // TODO: Implement FileSystem access when moved to Presentation layer
            var fallbackFullPath = Path.Combine("mock_app_package", fallbackPath);

            if (File.Exists(fallbackFullPath))
            {
                _logger.LogWarning("Audio file not found for language {Language}, falling back to English: {AudioKey}",
                    targetLanguage, audioKey);
                return fallbackPath;
            }
        }

        _logger.LogWarning("Localized audio file not found: {AudioKey} for language {Language}",
            audioKey, targetLanguage);
        return null;
    }

    public async Task<bool> SetAudioLanguageAsync(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            return false;
        }

        var supportedLanguages = new[] { "en", "es" };
        if (!supportedLanguages.Contains(languageCode.ToLowerInvariant()))
        {
            _logger.LogWarning("Unsupported audio language: {Language}", languageCode);
            return false;
        }

        _currentLanguage = languageCode.ToLowerInvariant();
        _logger.LogInformation("Audio language changed to: {Language}", _currentLanguage);

        // Stop any currently playing instruction audio to prevent confusion
        await StopAudioAsync(AudioType.Instruction);

        return true;
    }

    public string GetCurrentAudioLanguage()
    {
        return _currentLanguage;
    }

    #endregion

    #region Child-Friendly Audio Feedback

    public async Task PlaySuccessFeedbackAsync(FeedbackIntensity intensity = FeedbackIntensity.Medium, int? childAge = null, CancellationToken cancellationToken = default)
    {
        var audioKey = intensity switch
        {
            FeedbackIntensity.Soft => "feedback_success_soft",
            FeedbackIntensity.Medium => "feedback_success_medium",
            FeedbackIntensity.High => "feedback_success_celebration",
            _ => "feedback_success_medium"
        };

        await PlayLocalizedAudioAsync(audioKey, AudioType.SuccessFeedback, cancellationToken: cancellationToken);
    }

    public async Task PlayErrorFeedbackAsync(FeedbackIntensity encouragement = FeedbackIntensity.Soft, int? childAge = null, CancellationToken cancellationToken = default)
    {
        var audioKey = encouragement switch
        {
            FeedbackIntensity.Soft => "feedback_error_gentle",
            FeedbackIntensity.Medium => "feedback_error_encouraging",
            FeedbackIntensity.High => "feedback_error_motivating",
            _ => "feedback_error_gentle"
        };

        await PlayLocalizedAudioAsync(audioKey, AudioType.ErrorFeedback, cancellationToken: cancellationToken);
    }

    public async Task PlayActivityCompletionAsync(int starRating, string? activityType = null, CancellationToken cancellationToken = default)
    {
        var audioKey = starRating switch
        {
            1 => "completion_one_star",
            2 => "completion_two_stars",
            3 => "completion_three_stars",
            _ => "completion_general"
        };

        await PlayLocalizedAudioAsync(audioKey, AudioType.Completion, cancellationToken: cancellationToken);
    }

    public async Task PlayUIFeedbackAsync(UIInteractionType interactionType, CancellationToken cancellationToken = default)
    {
        var audioKey = interactionType switch
        {
            UIInteractionType.ButtonPress => "ui_button_press",
            UIInteractionType.PageTransition => "ui_page_transition",
            UIInteractionType.MenuToggle => "ui_menu_toggle",
            UIInteractionType.ItemSelection => "ui_item_select",
            UIInteractionType.DragDrop => "ui_drag_drop",
            UIInteractionType.LongPress => "ui_long_press",
            UIInteractionType.Swipe => "ui_swipe",
            UIInteractionType.ModalDialog => "ui_modal_appear",
            _ => "ui_button_press"
        };

        await PlayLocalizedAudioAsync(audioKey, AudioType.UIInteraction, cancellationToken: cancellationToken);
    }

    #endregion

    #region Educational Content Audio

    public async Task<bool> PlayInstructionAsync(string instructionKey, int? childAge = null, CancellationToken cancellationToken = default)
    {
        var audioItem = new AudioItem(instructionKey, AudioType.Instruction)
        {
            LocalizationKey = instructionKey,
            Language = _currentLanguage,
            Priority = AudioPriority.Critical,
            TargetAgeGroup = DetermineAgeGroup(childAge),
            PlaybackSettings = AudioPlaybackSettings.ForSpeechContent(DetermineAgeGroup(childAge))
        };

        return await PlayAudioAsync(audioItem, cancellationToken);
    }

    public async Task<bool> PlayQuestionAudioAsync(string questionText, string? questionType = null, CancellationToken cancellationToken = default)
    {
        // For now, use a generic question audio key
        // In a full implementation, this might use TTS or pre-recorded question audio
        var audioKey = $"question_{questionType ?? "general"}";

        return await PlayLocalizedAudioAsync(audioKey, AudioType.Instruction, cancellationToken: cancellationToken);
    }

    public async Task<bool> PlayBackgroundMusicAsync(string activityType, float volume = 0.3f, bool loop = true, CancellationToken cancellationToken = default)
    {
        var audioKey = $"background_music_{activityType}";

        var audioItem = new AudioItem(audioKey, AudioType.BackgroundMusic)
        {
            LocalizationKey = audioKey,
            Volume = volume,
            Loop = loop,
            Priority = AudioPriority.Low,
            PlaybackSettings = AudioPlaybackSettings.ForBackgroundMusic()
        };

        return await PlayAudioAsync(audioItem, cancellationToken);
    }

    #endregion

    #region Audio State Management

    public AudioPlaybackState GetPlaybackState(AudioType audioType)
    {
        return _playbackStates.TryGetValue(audioType, out var state) ? state : AudioPlaybackState.Stopped;
    }

    public bool IsAnyAudioPlaying()
    {
        return _activePlayers.Any(kvp => kvp.Value.IsPlaying);
    }

    public float GetVolumeLevel(AudioType audioType)
    {
        return _volumeLevels.TryGetValue(audioType, out var volume) ? volume : 1.0f;
    }

    public async Task SetVolumeLevelAsync(AudioType audioType, float volume)
    {
        var clampedVolume = Math.Clamp(volume, 0.0f, 1.0f);
        _volumeLevels[audioType] = clampedVolume;

        // Apply to any currently playing audio of this type
        var affectedPlayers = _activePlayers.Where(kvp => GetAudioTypeFromPlayerId(kvp.Key) == audioType);

        foreach (var kvp in affectedPlayers)
        {
            try
            {
                await kvp.Value.SetVolumeAsync(clampedVolume);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting volume for player {PlayerId}", kvp.Key);
            }
        }

        _logger.LogDebug("Volume set to {Volume} for audio type {AudioType}", clampedVolume, audioType);
    }

    #endregion

    #region Resource Management

    public async Task<Dictionary<string, bool>> PreloadAudioAsync(IEnumerable<string> audioKeys, CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, bool>();

        foreach (var audioKey in audioKeys)
        {
            try
            {
                var audioPath = GetLocalizedAudioPath(audioKey);
                if (!string.IsNullOrEmpty(audioPath))
                {
                    // Pre-load audio data into cache
                    var audioItem = new AudioItem(audioKey, AudioType.UIInteraction)
                    {
                        LocalizationKey = audioKey,
                        CacheAfterPlayback = true
                    };

                    await CacheAudioItemAsync(audioItem, audioPath);
                    results[audioKey] = true;
                }
                else
                {
                    results[audioKey] = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preloading audio: {AudioKey}", audioKey);
                results[audioKey] = false;
            }
        }

        _logger.LogInformation("Preloaded {Successful}/{Total} audio files",
            results.Count(r => r.Value), results.Count);

        return results;
    }

    public async Task<long> ClearAudioCacheAsync(bool keepRecent = true)
    {
        var freedBytes = 0L;
        var cutoffTime = keepRecent ? DateTime.UtcNow.AddMinutes(-30) : DateTime.MaxValue;

        var itemsToRemove = _cacheInfo.CachedItems
            .Where(item => !keepRecent || item.LastAccessed < cutoffTime)
            .ToList();

        foreach (var item in itemsToRemove)
        {
            if (_audioCache.TryRemove(item.CacheKey, out _))
            {
                freedBytes += item.SizeBytes;
                _cacheInfo.CachedItems.Remove(item);
            }
        }

        _cacheInfo.TotalCacheSizeBytes -= freedBytes;
        _cacheInfo.LastCleanup = DateTime.UtcNow;

        _logger.LogInformation("Cache cleanup freed {FreedMB}MB, removed {Count} items",
            freedBytes / (1024.0 * 1024.0), itemsToRemove.Count);

        return freedBytes;
    }

    public AudioCacheInfo GetCacheInfo()
    {
        return _cacheInfo;
    }

    #endregion

    #region Platform-Specific Operations

    public async Task HandleAudioInterruptionAsync(AudioInterruption interruption)
    {
        _logger.LogInformation("Handling audio interruption: {Interruption}", interruption);

        switch (interruption)
        {
            case AudioInterruption.PhoneCall:
            case AudioInterruption.SystemAudio:
                await PauseAudioAsync();
                break;

            case AudioInterruption.OtherApp:
                await PauseAudioAsync(AudioType.BackgroundMusic);
                break;

            case AudioInterruption.AppBackground:
                await StopAudioAsync(AudioType.BackgroundMusic);
                await PauseAudioAsync();
                break;

            case AudioInterruption.InterruptionEnded:
                // Only resume non-background music automatically
                await ResumeAudioAsync(AudioType.Instruction);
                await ResumeAudioAsync(AudioType.SuccessFeedback);
                await ResumeAudioAsync(AudioType.ErrorFeedback);
                break;

            case AudioInterruption.HardwareChange:
                // Restart audio with new hardware configuration
                await StopAudioAsync();
                break;
        }
    }

    public async Task<bool> ConfigureAudioSessionAsync(AudioSessionType sessionType)
    {
        try
        {
            // Platform-specific audio session configuration will be implemented
            // in platform-specific derived classes or through dependency injection

            _logger.LogInformation("Configuring audio session for type: {SessionType}", sessionType);

            // This is a placeholder for actual platform configuration
            await Task.Delay(100); // Simulate configuration time

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring audio session: {SessionType}", sessionType);
            return false;
        }
    }

    public async Task<bool> EnsureAudioPermissionsAsync()
    {
        try
        {
            // For Android, we typically don't need special permissions for audio playback
            // but we might need RECORD_AUDIO for voice features in the future

            // TODO: Implement Permissions access when moved to Presentation layer
            // var status = await Permissions.CheckStatusAsync<Permissions.Media>();
            // if (status != PermissionStatus.Granted)
            // {
            //     status = await Permissions.RequestAsync<Permissions.Media>();
            // }
            // return status == PermissionStatus.Granted;

            // Mock implementation for Infrastructure layer
            await Task.Delay(1); // Simulate async permission check
            return true; // Assume permissions are granted for development
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking audio permissions");
            return false;
        }
    }

    #endregion

    #region Educational Activity Audio

    public async Task<bool> PlayActivityIntroductionAsync(string activityType, string activityLevel, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var audioKey = $"activity.intro.{activityType}.{activityLevel}";
            var localizedPath = GetLocalizedAudioPath(audioKey, _currentLanguage);

            if (string.IsNullOrEmpty(localizedPath))
            {
                _logger.LogWarning("No introduction audio found for activity: {ActivityType}, level: {Level}", activityType, activityLevel);
                return false;
            }

            var audioItem = CreateEducationalAudioItem(localizedPath, AudioType.Instruction, childAge);
            return await PlayAudioAsync(audioItem, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing activity introduction for {ActivityType}", activityType);
            return false;
        }
    }

    public async Task<bool> PlayStepByStepGuidanceAsync(IEnumerable<string> stepInstructions, int stepDelay = 2000, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var instructions = stepInstructions.ToList();
            if (!instructions.Any())
            {
                _logger.LogWarning("No step instructions provided for guidance");
                return false;
            }

            var adjustedDelay = AdjustDelayForAge(stepDelay, childAge);

            for (int i = 0; i < instructions.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                var stepKey = $"guidance.step_{i + 1}";
                var success = await PlayLocalizedAudioAsync(stepKey, AudioType.Instruction, _currentLanguage, cancellationToken);

                if (!success)
                {
                    // Fallback to TTS if localized audio not available
                    success = await PlayQuestionAudioAsync(instructions[i], "step_instruction", cancellationToken);
                }

                if (!success)
                {
                    _logger.LogWarning("Failed to play step {Step} of {Total}", i + 1, instructions.Count);
                }

                // Add delay between steps, except for the last one
                if (i < instructions.Count - 1)
                {
                    await Task.Delay(adjustedDelay, cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing step-by-step guidance");
            return false;
        }
    }

    public async Task<bool> PlayAnswerChoicesAsync(IEnumerable<string> answerChoices, int choiceDelay = 1500, CancellationToken cancellationToken = default)
    {
        try
        {
            var choices = answerChoices.ToList();
            if (!choices.Any())
            {
                _logger.LogWarning("No answer choices provided for audio playback");
                return false;
            }

            // Play introductory phrase
            await PlayLocalizedAudioAsync("question.choices_intro", AudioType.Instruction, _currentLanguage, cancellationToken);
            await Task.Delay(500, cancellationToken);

            for (int i = 0; i < choices.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                var choiceText = $"Choice {i + 1}: {choices[i]}";
                var success = await PlayQuestionAudioAsync(choiceText, "answer_choice", cancellationToken);

                if (!success)
                {
                    _logger.LogWarning("Failed to play answer choice {Choice}", i + 1);
                }

                // Add delay between choices, except for the last one
                if (i < choices.Count - 1)
                {
                    await Task.Delay(choiceDelay, cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing answer choices");
            return false;
        }
    }

    public async Task<bool> PlayProgressEncouragementAsync(int progressPercentage, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var encouragementKey = GetProgressEncouragementKey(progressPercentage);
            var success = await PlayLocalizedAudioAsync(encouragementKey, AudioType.SuccessFeedback, _currentLanguage, cancellationToken);

            if (!success)
            {
                // Fallback to generic success feedback
                await PlaySuccessFeedbackAsync(FeedbackIntensity.Medium, childAge, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing progress encouragement for {Progress}%", progressPercentage);
            return false;
        }
    }

    public async Task<bool> PlayHintAudioAsync(string hintText, int hintLevel = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            // Play a gentle hint introduction sound
            await PlayUIFeedbackAsync(UIInteractionType.ButtonPress, cancellationToken);
            await Task.Delay(300, cancellationToken);

            // Play the hint text
            return await PlayQuestionAudioAsync(hintText, "hint", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing hint audio: {HintText}", hintText);
            return false;
        }
    }

    #endregion

    #region Achievement and Celebration Audio

    public async Task<bool> PlayCrownUnlockCelebrationAsync(string challengeType, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Play exciting achievement sound first
            await PlayActivityCompletionAsync(3, challengeType, cancellationToken);
            await Task.Delay(1000, cancellationToken);

            // Play crown unlock specific celebration
            var crownKey = $"celebration.crown_unlock.{challengeType}";
            var success = await PlayLocalizedAudioAsync(crownKey, AudioType.Achievement, _currentLanguage, cancellationToken);

            if (!success)
            {
                // Fallback to general crown celebration
                success = await PlayLocalizedAudioAsync("celebration.crown_unlock.general", AudioType.Achievement, _currentLanguage, cancellationToken);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing crown unlock celebration for {ChallengeType}", challengeType);
            return false;
        }
    }

    public async Task<bool> PlayMilestoneAchievementAsync(string milestoneType, int achievementLevel, string? childName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Determine celebration intensity based on achievement level
            var intensity = achievementLevel switch
            {
                >= 10 => FeedbackIntensity.High,
                >= 5 => FeedbackIntensity.Medium,
                _ => FeedbackIntensity.Soft
            };

            await PlaySuccessFeedbackAsync(intensity, null, cancellationToken);
            await Task.Delay(800, cancellationToken);

            // Play milestone-specific celebration
            var milestoneKey = $"milestone.{milestoneType}.level_{achievementLevel}";
            var success = await PlayLocalizedAudioAsync(milestoneKey, AudioType.Achievement, _currentLanguage, cancellationToken);

            if (!success)
            {
                // Fallback to generic milestone celebration
                success = await PlayLocalizedAudioAsync("milestone.general", AudioType.Achievement, _currentLanguage, cancellationToken);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing milestone achievement for {MilestoneType}", milestoneType);
            return false;
        }
    }

    public async Task<bool> PlayStreakCelebrationAsync(int streakDays, string streakType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Play celebration based on streak length
            var intensity = streakDays switch
            {
                >= 30 => FeedbackIntensity.High,
                >= 7 => FeedbackIntensity.Medium,
                _ => FeedbackIntensity.Soft
            };

            await PlaySuccessFeedbackAsync(intensity, null, cancellationToken);
            await Task.Delay(600, cancellationToken);

            // Play streak-specific celebration
            var streakMilestone = GetStreakMilestone(streakDays);
            var streakKey = $"streak.{streakType}.{streakMilestone}";
            var success = await PlayLocalizedAudioAsync(streakKey, AudioType.Achievement, _currentLanguage, cancellationToken);

            if (!success)
            {
                // Fallback to generic streak celebration
                success = await PlayLocalizedAudioAsync("streak.general", AudioType.Achievement, _currentLanguage, cancellationToken);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing streak celebration for {StreakDays} days", streakDays);
            return false;
        }
    }

    #endregion

    #region Accessibility and Support Audio

    public async Task<bool> PlayVisualDescriptionAsync(string visualDescription, string elementType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Play accessibility introduction sound
            await PlayUIFeedbackAsync(UIInteractionType.ButtonPress, cancellationToken);
            await Task.Delay(300, cancellationToken);

            // Play the visual description
            return await PlayQuestionAudioAsync(visualDescription, "visual_description", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing visual description: {Description}", visualDescription);
            return false;
        }
    }

    public async Task<bool> PlayRepeatedQuestionAsync(string questionText, string repetitionSpeed = "normal", CancellationToken cancellationToken = default)
    {
        try
        {
            // For now, play at normal speed - speed control would require platform-specific implementation
            return await PlayQuestionAudioAsync(questionText, "repeated_question", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing repeated question: {QuestionText}", questionText);
            return false;
        }
    }

    public async Task<bool> PlayHelpSystemAudioAsync(string helpContext, CancellationToken cancellationToken = default)
    {
        try
        {
            var helpKey = $"help.{helpContext}";
            var success = await PlayLocalizedAudioAsync(helpKey, AudioType.Instruction, _currentLanguage, cancellationToken);

            if (!success)
            {
                // Fallback to generic help audio
                success = await PlayLocalizedAudioAsync("help.general", AudioType.Instruction, _currentLanguage, cancellationToken);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing help system audio for context: {HelpContext}", helpContext);
            return false;
        }
    }

    #endregion

    #region Educational Helper Methods

    private AudioItem CreateEducationalAudioItem(string filePath, AudioType audioType, int? childAge)
    {
        var settings = new AudioPlaybackSettings
        {
            Volume = GetVolumeLevel(audioType),
            Priority = AudioPriority.High,
            FadeInDuration = 300,
            ChildSafeMode = true,
            MaxVolumeOverride = childAge <= 4 ? 0.7f : 0.85f
        };

        return new AudioItem
        {
            FilePath = filePath,
            AudioType = audioType,
            PlaybackSettings = settings,
            SessionId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private int AdjustDelayForAge(int baseDelay, int? childAge)
    {
        if (!childAge.HasValue) return baseDelay;

        return childAge.Value switch
        {
            <= 4 => (int)(baseDelay * 1.5), // Slower for Pre-K
            5 => (int)(baseDelay * 1.2),     // Slightly slower for Kindergarten
            _ => baseDelay                   // Normal for Primary
        };
    }

    private string GetProgressEncouragementKey(int progressPercentage)
    {
        return progressPercentage switch
        {
            >= 90 => "encouragement.nearly_done",
            >= 75 => "encouragement.great_progress",
            >= 50 => "encouragement.halfway",
            >= 25 => "encouragement.good_start",
            _ => "encouragement.keep_going"
        };
    }

    private string GetStreakMilestone(int streakDays)
    {
        return streakDays switch
        {
            >= 30 => "month",
            >= 14 => "two_weeks",
            >= 7 => "week",
            >= 3 => "three_days",
            _ => "daily"
        };
    }

    #endregion

    #region Private Helper Methods

    private void InitializeVolumeSettings()
    {
        // Set child-friendly default volumes for each audio type
        _volumeLevels[AudioType.UIInteraction] = 0.7f;
        _volumeLevels[AudioType.Instruction] = 0.9f;
        _volumeLevels[AudioType.SuccessFeedback] = 0.8f;
        _volumeLevels[AudioType.ErrorFeedback] = 0.6f;
        _volumeLevels[AudioType.Completion] = 0.9f;
        _volumeLevels[AudioType.BackgroundMusic] = 0.3f;
        _volumeLevels[AudioType.Achievement] = 0.8f;
        _volumeLevels[AudioType.Mascot] = 0.8f;
    }

    private string GetSystemLanguage()
    {
        try
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture;
            return culture.TwoLetterISOLanguageName.ToLowerInvariant() == "es" ? "es" : "en";
        }
        catch
        {
            return "en"; // Default to English
        }
    }

    private async Task<string?> ResolveAudioPathAsync(AudioItem audioItem, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(audioItem.FilePath))
        {
            return audioItem.FilePath;
        }

        if (!string.IsNullOrEmpty(audioItem.LocalizationKey))
        {
            return GetLocalizedAudioPath(audioItem.LocalizationKey, audioItem.Language);
        }

        return null;
    }

    private float CalculateEffectiveVolume(AudioItem audioItem)
    {
        var typeVolume = GetVolumeLevel(audioItem.AudioType);
        return Math.Clamp(audioItem.Volume * typeVolume, 0.0f, 1.0f);
    }

    private AgeGroup DetermineAgeGroup(int? childAge)
    {
        return childAge switch
        {
            <= 4 => AgeGroup.PreK,
            5 => AgeGroup.Kindergarten,
            >= 6 => AgeGroup.Primary,
            _ => AgeGroup.Kindergarten
        };
    }

    private string GeneratePlayerId(AudioItem audioItem)
    {
        return $"{audioItem.AudioType}_{Guid.NewGuid():N}";
    }

    private AudioType GetAudioTypeFromPlayerId(string playerId)
    {
        var parts = playerId.Split('_');
        if (parts.Length > 0 && Enum.TryParse<AudioType>(parts[0], out var audioType))
        {
            return audioType;
        }
        return AudioType.UIInteraction;
    }

    private IAudioPlayer CreateAudioPlayer(AudioItem audioItem, string filePath)
    {
        // This will be implemented with platform-specific audio players
        // For now, return a mock implementation
        return new CrossPlatformAudioPlayer(filePath, audioItem, _logger);
    }

    private async Task InterruptLowerPriorityAudioAsync(AudioPriority priority)
    {
        var playersToInterrupt = _activePlayers.Where(kvp =>
        {
            // Get priority from player (this would be stored in actual implementation)
            return GetPlayerPriority(kvp.Key) < priority;
        });

        foreach (var kvp in playersToInterrupt)
        {
            try
            {
                await kvp.Value.StopAsync(100); // Quick fade out
                _activePlayers.TryRemove(kvp.Key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interrupting lower priority audio {PlayerId}", kvp.Key);
            }
        }
    }

    private AudioPriority GetPlayerPriority(string playerId)
    {
        // In actual implementation, this would be stored with the player
        // For now, infer from audio type
        var audioType = GetAudioTypeFromPlayerId(playerId);
        return audioType switch
        {
            AudioType.Instruction => AudioPriority.Critical,
            AudioType.SuccessFeedback => AudioPriority.High,
            AudioType.ErrorFeedback => AudioPriority.High,
            AudioType.Completion => AudioPriority.High,
            AudioType.UIInteraction => AudioPriority.Normal,
            AudioType.BackgroundMusic => AudioPriority.Low,
            _ => AudioPriority.Normal
        };
    }

    private async Task CacheAudioItemAsync(AudioItem audioItem, string filePath)
    {
        try
        {
            var cacheKey = $"{audioItem.LocalizationKey ?? filePath}_{audioItem.Language}";

            if (!_audioCache.ContainsKey(cacheKey))
            {
                _audioCache[cacheKey] = audioItem.Clone();

                // Update cache info (simplified)
                _cacheInfo.CachedFileCount = _audioCache.Count;
                _cacheInfo.TotalCacheSizeBytes += 1024 * 100; // Estimate 100KB per file
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching audio item: {FilePath}", filePath);
        }
    }

    private void PerformCacheCleanup(object? state)
    {
        try
        {
            _ = Task.Run(async () => await ClearAudioCacheAsync(keepRecent: true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during scheduled cache cleanup");
        }
    }

    #endregion

    #region Event Handlers

    private void OnAudioStarted(AudioItem audioItem, string sessionId)
    {
        var eventArgs = new AudioPlaybackEventArgs(audioItem, AudioPlaybackState.Playing, sessionId: sessionId);
        AudioStarted?.Invoke(this, eventArgs);
    }

    private void OnAudioStopped(AudioItem audioItem, string sessionId)
    {
        var eventArgs = new AudioPlaybackEventArgs(audioItem, AudioPlaybackState.Stopped, sessionId: sessionId);
        AudioStopped?.Invoke(this, eventArgs);
    }

    private void OnAudioPaused(AudioItem audioItem, string sessionId)
    {
        var eventArgs = new AudioPlaybackEventArgs(audioItem, AudioPlaybackState.Paused, sessionId: sessionId);
        AudioPaused?.Invoke(this, eventArgs);
    }

    private void OnAudioResumed(AudioItem audioItem, string sessionId)
    {
        var eventArgs = new AudioPlaybackEventArgs(audioItem, AudioPlaybackState.Playing, sessionId: sessionId);
        AudioResumed?.Invoke(this, eventArgs);
    }

    private void OnAudioError(AudioItem? audioItem, AudioErrorCode errorCode, string message, bool isRecoverable)
    {
        var error = new InvalidOperationException(message);
        var eventArgs = new AudioErrorEventArgs(error, errorCode, message, isRecoverable, audioItem);
        AudioError?.Invoke(this, eventArgs);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _cacheCleanupTimer?.Dispose();

            // Stop all active players
            foreach (var player in _activePlayers.Values)
            {
                try
                {
                    player.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing audio player");
                }
            }

            _activePlayers.Clear();
            _operationLock?.Dispose();

            _disposed = true;
        }
    }

    #endregion
}