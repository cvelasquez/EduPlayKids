using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace EduPlayKids.Infrastructure.Services.Audio;

/// <summary>
/// Educational audio service that extends the base audio service with specialized functionality
/// for children's educational content, including bilingual narration, age-appropriate pacing,
/// and comprehensive educational audio workflows.
/// </summary>
public class EducationalAudioService : IEducationalAudioService
{
    #region Private Fields

    private readonly IAudioService _baseAudioService;
    private readonly ILogger<EducationalAudioService> _logger;
    private readonly EducationalAudioConfiguration _configuration;
    private readonly ConcurrentDictionary<string, EducationalAudioContent> _educationalCache;
    private readonly ConcurrentDictionary<string, QuestionAudioModel> _questionCache;
    private readonly SemaphoreSlim _operationLock;

    // Age-based audio settings
    private readonly Dictionary<AgeGroup, EducationalAudioSettings> _ageGroupSettings;

    // Subject-specific audio resources
    private readonly Dictionary<string, SubjectAudioResources> _subjectResources;

    // Current educational context
    private EducationalContext? _currentContext;
    private readonly List<string> _currentPlaylist;
    private int _currentPlaylistIndex;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when educational audio content starts playing.
    /// </summary>
    public event EventHandler<EducationalAudioEventArgs>? EducationalAudioStarted;

    /// <summary>
    /// Event raised when educational audio content completes.
    /// </summary>
    public event EventHandler<EducationalAudioEventArgs>? EducationalAudioCompleted;

    /// <summary>
    /// Event raised when question audio events occur.
    /// </summary>
    public event EventHandler<QuestionAudioEventArgs>? QuestionAudioEvent;

    /// <summary>
    /// Event raised when activity audio events occur.
    /// </summary>
    public event EventHandler<ActivityAudioEventArgs>? ActivityAudioEvent;

    /// <summary>
    /// Event raised when accessibility audio events occur.
    /// </summary>
    public event EventHandler<AccessibilityAudioEventArgs>? AccessibilityAudioEvent;

    #endregion

    #region Constructor

    public EducationalAudioService(
        IAudioService baseAudioService,
        ILogger<EducationalAudioService> logger,
        EducationalAudioConfiguration? configuration = null)
    {
        _baseAudioService = baseAudioService ?? throw new ArgumentNullException(nameof(baseAudioService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? EducationalAudioConfiguration.Default;

        _educationalCache = new ConcurrentDictionary<string, EducationalAudioContent>();
        _questionCache = new ConcurrentDictionary<string, QuestionAudioModel>();
        _operationLock = new SemaphoreSlim(1, 1);
        _ageGroupSettings = new Dictionary<AgeGroup, EducationalAudioSettings>();
        _subjectResources = new Dictionary<string, SubjectAudioResources>();
        _currentPlaylist = new List<string>();

        InitializeAgeGroupSettings();
        InitializeSubjectResources();
        SubscribeToBaseAudioEvents();

        _logger.LogInformation("EducationalAudioService initialized successfully");
    }

    #endregion

    #region Educational Activity Audio Implementation

    public async Task<bool> PlayActivityIntroductionAsync(string activityType, string activityLevel, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var audioKey = $"activity.intro.{activityType}.{activityLevel}";
            var content = await GetEducationalContentAsync(audioKey, childAge);

            if (content == null)
            {
                _logger.LogWarning("No introduction audio found for activity type: {ActivityType}, level: {Level}", activityType, activityLevel);
                return false;
            }

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.IntroductionPlayed)
            {
                ActivityId = activityType,
                Content = content,
                ChildAge = childAge,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnActivityAudioEvent(eventArgs);

            var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
            if (string.IsNullOrEmpty(audioPath))
            {
                _logger.LogWarning("No audio path found for language: {Language}", _baseAudioService.GetCurrentAudioLanguage());
                return false;
            }

            return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.Instruction, AudioPriority.High, cancellationToken);
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

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.StepGuidanceStarted)
            {
                ChildAge = childAge,
                TotalSteps = instructions.Count,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnActivityAudioEvent(eventArgs);

            var settings = GetAgeGroupSettings(childAge);
            var adjustedDelay = AdjustDelayForAge(stepDelay, childAge);

            for (int i = 0; i < instructions.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                var stepKey = $"guidance.step.{i + 1}";
                var success = await PlayLocalizedInstructionAsync(stepKey, instructions[i], childAge, cancellationToken);

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

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.AnswerChoiceReadingStarted)
            {
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnQuestionAudioEvent(eventArgs);

            // Play introductory phrase
            await PlayLocalizedInstructionAsync("question.choices_intro", "Here are your choices:", null, cancellationToken);
            await Task.Delay(500, cancellationToken);

            for (int i = 0; i < choices.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                var choiceText = $"Choice {i + 1}: {choices[i]}";
                var success = await _baseAudioService.PlayQuestionAudioAsync(choiceText, "answer_choice", cancellationToken);

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

            var completedEventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.AnswerChoiceReadingCompleted)
            {
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnQuestionAudioEvent(completedEventArgs);
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
            var content = await GetEducationalContentAsync(encouragementKey, childAge);

            if (content == null)
            {
                _logger.LogDebug("No specific encouragement audio for progress: {Progress}%", progressPercentage);
                return false;
            }

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.ProgressEncouragementPlayed)
            {
                ProgressPercentage = progressPercentage,
                ChildAge = childAge,
                Content = content,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnActivityAudioEvent(eventArgs);

            var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
            if (string.IsNullOrEmpty(audioPath))
            {
                return false;
            }

            return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.SuccessFeedback, AudioPriority.Normal, cancellationToken);
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
            var hintKey = $"hint.level_{hintLevel}";
            var intensity = hintLevel switch
            {
                1 => FeedbackIntensity.Soft,
                2 => FeedbackIntensity.Medium,
                _ => FeedbackIntensity.High
            };

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.HintAudioPlayed)
            {
                HintLevel = hintLevel,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext(),
                Metadata = new Dictionary<string, object> { { "hintText", hintText } }
            };

            OnQuestionAudioEvent(eventArgs);

            // Play hint introduction sound
            await _baseAudioService.PlayUIFeedbackAsync(UIInteractionType.ButtonPress, cancellationToken);

            // Play the actual hint text
            return await _baseAudioService.PlayQuestionAudioAsync(hintText, "hint", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing hint audio: {HintText}", hintText);
            return false;
        }
    }

    #endregion

    #region Achievement and Celebration Audio Implementation

    public async Task<bool> PlayCrownUnlockCelebrationAsync(string challengeType, int? childAge = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var celebrationKey = $"celebration.crown_unlock.{challengeType}";
            var content = await GetEducationalContentAsync(celebrationKey, childAge);

            if (content == null)
            {
                celebrationKey = "celebration.crown_unlock.general";
                content = await GetEducationalContentAsync(celebrationKey, childAge);
            }

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.CrownChallengeUnlocked)
            {
                ChildAge = childAge,
                Content = content,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext(),
                Achievement = new AchievementAudioData
                {
                    AchievementType = "crown_challenge",
                    UnlocksCrownChallenge = true,
                    CelebrationIntensity = FeedbackIntensity.High
                }
            };

            OnActivityAudioEvent(eventArgs);

            // Play exciting crown unlock celebration
            await _baseAudioService.PlayActivityCompletionAsync(3, challengeType, cancellationToken);

            if (content != null)
            {
                var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
                if (!string.IsNullOrEmpty(audioPath))
                {
                    await Task.Delay(1000, cancellationToken); // Brief pause
                    return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.Achievement, AudioPriority.High, cancellationToken);
                }
            }

            return true;
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
            var milestoneKey = $"milestone.{milestoneType}.level_{achievementLevel}";
            var content = await GetEducationalContentAsync(milestoneKey, null);

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.MilestoneAchieved)
            {
                Content = content,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext(),
                Achievement = new AchievementAudioData
                {
                    AchievementType = milestoneType,
                    AchievementLevel = achievementLevel,
                    CelebrationIntensity = achievementLevel > 5 ? FeedbackIntensity.High : FeedbackIntensity.Medium
                }
            };

            OnActivityAudioEvent(eventArgs);

            // Determine celebration intensity based on achievement level
            var intensity = achievementLevel switch
            {
                >= 10 => FeedbackIntensity.High,
                >= 5 => FeedbackIntensity.Medium,
                _ => FeedbackIntensity.Soft
            };

            await _baseAudioService.PlaySuccessFeedbackAsync(intensity, null, cancellationToken);

            if (content != null)
            {
                var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
                if (!string.IsNullOrEmpty(audioPath))
                {
                    await Task.Delay(800, cancellationToken);
                    return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.Achievement, AudioPriority.High, cancellationToken);
                }
            }

            return true;
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
            var streakKey = $"streak.{streakType}.{GetStreakMilestone(streakDays)}";
            var content = await GetEducationalContentAsync(streakKey, null);

            var eventArgs = new ActivityAudioEventArgs(ActivityAudioEventType.StreakCelebration)
            {
                Content = content,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext(),
                Achievement = new AchievementAudioData
                {
                    AchievementType = "streak",
                    AchievementLevel = streakDays,
                    CelebrationIntensity = streakDays >= 7 ? FeedbackIntensity.High : FeedbackIntensity.Medium
                }
            };

            OnActivityAudioEvent(eventArgs);

            // Play celebration based on streak length
            var intensity = streakDays switch
            {
                >= 30 => FeedbackIntensity.High,
                >= 7 => FeedbackIntensity.Medium,
                _ => FeedbackIntensity.Soft
            };

            await _baseAudioService.PlaySuccessFeedbackAsync(intensity, null, cancellationToken);

            if (content != null)
            {
                var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
                if (!string.IsNullOrEmpty(audioPath))
                {
                    await Task.Delay(600, cancellationToken);
                    return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.Achievement, AudioPriority.Normal, cancellationToken);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing streak celebration for {StreakDays} days", streakDays);
            return false;
        }
    }

    #endregion

    #region Accessibility and Support Audio Implementation

    public async Task<bool> PlayVisualDescriptionAsync(string visualDescription, string elementType, CancellationToken cancellationToken = default)
    {
        try
        {
            var eventArgs = new AccessibilityAudioEventArgs(AccessibilityAudioEventType.VisualDescriptionPlayed)
            {
                Description = visualDescription,
                ElementType = elementType,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnAccessibilityAudioEvent(eventArgs);

            // Play accessibility introduction sound
            await _baseAudioService.PlayUIFeedbackAsync(UIInteractionType.ButtonPress, cancellationToken);
            await Task.Delay(300, cancellationToken);

            // Play the visual description
            return await _baseAudioService.PlayQuestionAudioAsync(visualDescription, "visual_description", cancellationToken);
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
            var eventArgs = new AccessibilityAudioEventArgs(AccessibilityAudioEventType.AccessibilityRepetition)
            {
                Description = questionText,
                UserRequested = true,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext(),
                Metadata = new Dictionary<string, object> { { "repetitionSpeed", repetitionSpeed } }
            };

            OnAccessibilityAudioEvent(eventArgs);

            // Adjust playback speed based on repetition speed setting
            // This would require extending the base audio service with speed control
            return await _baseAudioService.PlayQuestionAudioAsync(questionText, "repeated_question", cancellationToken);
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
            var content = await GetEducationalContentAsync(helpKey, null);

            var eventArgs = new AccessibilityAudioEventArgs(AccessibilityAudioEventType.HelpSystemAudioPlayed)
            {
                TargetElement = helpContext,
                Language = _baseAudioService.GetCurrentAudioLanguage(),
                Context = _currentContext ?? new EducationalContext()
            };

            OnAccessibilityAudioEvent(eventArgs);

            if (content != null)
            {
                var audioPath = content.GetAudioPath(_baseAudioService.GetCurrentAudioLanguage());
                if (!string.IsNullOrEmpty(audioPath))
                {
                    return await _baseAudioService.PlayAudioAsync(audioPath, AudioType.Instruction, AudioPriority.High, cancellationToken);
                }
            }

            // Fallback to generic help audio
            return await _baseAudioService.PlayLocalizedAudioAsync("help.general", AudioType.Instruction, null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing help system audio for context: {HelpContext}", helpContext);
            return false;
        }
    }

    #endregion

    #region Helper Methods

    private async Task<EducationalAudioContent?> GetEducationalContentAsync(string contentKey, int? childAge)
    {
        if (_educationalCache.TryGetValue(contentKey, out var cachedContent))
        {
            return childAge.HasValue ? cachedContent.WithParameters(new Dictionary<string, object> { { "childAge", childAge.Value } }) : cachedContent;
        }

        // In a real implementation, this would load from a resource file or database
        // For now, return null to indicate content not found
        _logger.LogDebug("Educational content not found for key: {ContentKey}", contentKey);
        return null;
    }

    private async Task<bool> PlayLocalizedInstructionAsync(string instructionKey, string fallbackText, int? childAge, CancellationToken cancellationToken)
    {
        // Try to play localized audio first
        var success = await _baseAudioService.PlayLocalizedAudioAsync(instructionKey, AudioType.Instruction, null, cancellationToken);

        if (!success)
        {
            // Fallback to playing the text directly
            return await _baseAudioService.PlayQuestionAudioAsync(fallbackText, "instruction", cancellationToken);
        }

        return true;
    }

    private EducationalAudioSettings GetAgeGroupSettings(int? childAge)
    {
        if (!childAge.HasValue)
            return _ageGroupSettings[AgeGroup.Primary]; // Default to primary

        var ageGroup = childAge.Value switch
        {
            <= 4 => AgeGroup.PreK,
            5 => AgeGroup.Kindergarten,
            _ => AgeGroup.Primary
        };

        return _ageGroupSettings[ageGroup];
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

    private void InitializeAgeGroupSettings()
    {
        _ageGroupSettings[AgeGroup.PreK] = new EducationalAudioSettings
        {
            SpeechRate = 0.8f,
            PauseBetweenSentences = 1500,
            RepetitionEnabled = true,
            MaxVolumeLevel = 0.7f
        };

        _ageGroupSettings[AgeGroup.Kindergarten] = new EducationalAudioSettings
        {
            SpeechRate = 0.9f,
            PauseBetweenSentences = 1200,
            RepetitionEnabled = true,
            MaxVolumeLevel = 0.8f
        };

        _ageGroupSettings[AgeGroup.Primary] = new EducationalAudioSettings
        {
            SpeechRate = 1.0f,
            PauseBetweenSentences = 1000,
            RepetitionEnabled = false,
            MaxVolumeLevel = 0.85f
        };
    }

    private void InitializeSubjectResources()
    {
        var subjects = new[] { "math", "reading", "science", "logic", "concepts" };

        foreach (var subject in subjects)
        {
            _subjectResources[subject] = new SubjectAudioResources
            {
                Subject = subject,
                IntroductionKeys = new List<string> { $"{subject}.intro.easy", $"{subject}.intro.medium", $"{subject}.intro.hard" },
                EncouragementKeys = new List<string> { $"{subject}.encouragement.positive", $"{subject}.encouragement.motivational" },
                CompletionKeys = new List<string> { $"{subject}.completion.celebration", $"{subject}.completion.achievement" }
            };
        }
    }

    private void SubscribeToBaseAudioEvents()
    {
        _baseAudioService.AudioStarted += OnBaseAudioStarted;
        _baseAudioService.AudioStopped += OnBaseAudioStopped;
        _baseAudioService.AudioError += OnBaseAudioError;
    }

    #endregion

    #region Event Handlers

    private void OnBaseAudioStarted(object? sender, AudioPlaybackEventArgs e)
    {
        _logger.LogDebug("Base audio started: {AudioId}", e.AudioId);
    }

    private void OnBaseAudioStopped(object? sender, AudioPlaybackEventArgs e)
    {
        _logger.LogDebug("Base audio stopped: {AudioId}", e.AudioId);
    }

    private void OnBaseAudioError(object? sender, AudioErrorEventArgs e)
    {
        _logger.LogWarning("Base audio error: {ErrorMessage}", e.ErrorMessage);
    }

    private void OnEducationalAudioStarted(EducationalAudioEventArgs eventArgs)
    {
        EducationalAudioStarted?.Invoke(this, eventArgs);
    }

    private void OnEducationalAudioCompleted(EducationalAudioEventArgs eventArgs)
    {
        EducationalAudioCompleted?.Invoke(this, eventArgs);
    }

    private void OnQuestionAudioEvent(QuestionAudioEventArgs eventArgs)
    {
        QuestionAudioEvent?.Invoke(this, eventArgs);
    }

    private void OnActivityAudioEvent(ActivityAudioEventArgs eventArgs)
    {
        ActivityAudioEvent?.Invoke(this, eventArgs);
    }

    private void OnAccessibilityAudioEvent(AccessibilityAudioEventArgs eventArgs)
    {
        AccessibilityAudioEvent?.Invoke(this, eventArgs);
    }

    #endregion

    #region Educational Context Management

    /// <summary>
    /// Sets the current educational context for audio personalization.
    /// </summary>
    /// <param name="context">Educational context information</param>
    public void SetEducationalContext(EducationalContext context)
    {
        _currentContext = context;
        _logger.LogDebug("Educational context updated: {Subject}, {ActivityType}", context.Subject, context.ActivityType);
    }

    /// <summary>
    /// Gets the current educational context.
    /// </summary>
    /// <returns>Current educational context or null if not set</returns>
    public EducationalContext? GetEducationalContext()
    {
        return _currentContext;
    }

    #endregion
}

/// <summary>
/// Configuration settings for educational audio features.
/// </summary>
public class EducationalAudioConfiguration
{
    public bool EnableProgressEncouragement { get; set; } = true;
    public bool EnableRepetitionSupport { get; set; } = true;
    public bool EnableAccessibilityFeatures { get; set; } = true;
    public int DefaultStepDelay { get; set; } = 2000;
    public int DefaultChoiceDelay { get; set; } = 1500;
    public float ChildSafeMaxVolume { get; set; } = 0.85f;

    public static EducationalAudioConfiguration Default => new();
}

/// <summary>
/// Audio settings specific to different age groups.
/// </summary>
public class EducationalAudioSettings
{
    public float SpeechRate { get; set; } = 1.0f;
    public int PauseBetweenSentences { get; set; } = 1000;
    public bool RepetitionEnabled { get; set; } = false;
    public float MaxVolumeLevel { get; set; } = 0.85f;
}

/// <summary>
/// Audio resources organized by educational subject.
/// </summary>
public class SubjectAudioResources
{
    public string Subject { get; set; } = string.Empty;
    public List<string> IntroductionKeys { get; set; } = new();
    public List<string> EncouragementKeys { get; set; } = new();
    public List<string> CompletionKeys { get; set; } = new();
}

/// <summary>
/// Interface for educational audio service functionality.
/// Extends base audio service with educational-specific methods.
/// </summary>
public interface IEducationalAudioService
{
    // Educational Activity Audio
    Task<bool> PlayActivityIntroductionAsync(string activityType, string activityLevel, int? childAge = null, CancellationToken cancellationToken = default);
    Task<bool> PlayStepByStepGuidanceAsync(IEnumerable<string> stepInstructions, int stepDelay = 2000, int? childAge = null, CancellationToken cancellationToken = default);
    Task<bool> PlayAnswerChoicesAsync(IEnumerable<string> answerChoices, int choiceDelay = 1500, CancellationToken cancellationToken = default);
    Task<bool> PlayProgressEncouragementAsync(int progressPercentage, int? childAge = null, CancellationToken cancellationToken = default);
    Task<bool> PlayHintAudioAsync(string hintText, int hintLevel = 1, CancellationToken cancellationToken = default);

    // Achievement and Celebration Audio
    Task<bool> PlayCrownUnlockCelebrationAsync(string challengeType, int? childAge = null, CancellationToken cancellationToken = default);
    Task<bool> PlayMilestoneAchievementAsync(string milestoneType, int achievementLevel, string? childName = null, CancellationToken cancellationToken = default);
    Task<bool> PlayStreakCelebrationAsync(int streakDays, string streakType, CancellationToken cancellationToken = default);

    // Accessibility and Support Audio
    Task<bool> PlayVisualDescriptionAsync(string visualDescription, string elementType, CancellationToken cancellationToken = default);
    Task<bool> PlayRepeatedQuestionAsync(string questionText, string repetitionSpeed = "normal", CancellationToken cancellationToken = default);
    Task<bool> PlayHelpSystemAudioAsync(string helpContext, CancellationToken cancellationToken = default);

    // Context Management
    void SetEducationalContext(EducationalContext context);
    EducationalContext? GetEducationalContext();

    // Events
    event EventHandler<EducationalAudioEventArgs>? EducationalAudioStarted;
    event EventHandler<EducationalAudioEventArgs>? EducationalAudioCompleted;
    event EventHandler<QuestionAudioEventArgs>? QuestionAudioEvent;
    event EventHandler<ActivityAudioEventArgs>? ActivityAudioEvent;
    event EventHandler<AccessibilityAudioEventArgs>? AccessibilityAudioEvent;
}