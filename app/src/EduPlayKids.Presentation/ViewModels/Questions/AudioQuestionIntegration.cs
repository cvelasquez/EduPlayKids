using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Presentation.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EduPlayKids.Presentation.ViewModels.Questions;

/// <summary>
/// Comprehensive audio integration component for educational questions.
/// Handles bilingual narration, age-appropriate pacing, accessibility features,
/// and educational audio workflows for children aged 3-8.
/// </summary>
public class AudioQuestionIntegration : IDisposable
{
    #region Private Fields

    private readonly IAudioService _audioService;
    private readonly ILogger<AudioQuestionIntegration> _logger;
    private readonly List<string> _audioPlaylist;
    private readonly SemaphoreSlim _audioLock;

    private QuestionModelBase? _currentQuestion;
    private QuestionAudioModel? _questionAudio;
    private int _childAge;
    private string _language;
    private int _hintLevel;
    private int _repetitionCount;
    private DateTime _questionStartTime;
    private bool _disposed;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when question audio events occur.
    /// </summary>
    public event EventHandler<QuestionAudioEventArgs>? QuestionAudioEvent;

    /// <summary>
    /// Event raised when accessibility audio is needed.
    /// </summary>
    public event EventHandler<AccessibilityAudioEventArgs>? AccessibilityAudioEvent;

    #endregion

    #region Constructor

    public AudioQuestionIntegration(
        IAudioService audioService,
        ILogger<AudioQuestionIntegration> logger)
    {
        _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _audioPlaylist = new List<string>();
        _audioLock = new SemaphoreSlim(1, 1);
        _language = "en";
        _hintLevel = 1;
        _repetitionCount = 0;

        SubscribeToAudioServiceEvents();
    }

    #endregion

    #region Question Audio Setup

    /// <summary>
    /// Initializes audio integration for a specific question.
    /// </summary>
    /// <param name="question">Question model to integrate with</param>
    /// <param name="childAge">Child's age for appropriate audio pacing</param>
    /// <param name="language">Language preference (en/es)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task InitializeQuestionAudioAsync(
        QuestionModelBase question,
        int childAge,
        string language = "en",
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _audioLock.WaitAsync(cancellationToken);

            _currentQuestion = question;
            _childAge = childAge;
            _language = language;
            _repetitionCount = 0;
            _hintLevel = 1;
            _questionStartTime = DateTime.UtcNow;

            // Create question audio model
            _questionAudio = CreateQuestionAudioModel(question, childAge, language);

            // Preload audio for immediate playback
            await PreloadQuestionAudioAsync(cancellationToken);

            _logger.LogInformation("Question audio initialized for question {QuestionId}, child age {ChildAge}",
                question.QuestionId, childAge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing question audio");
        }
        finally
        {
            _audioLock.Release();
        }
    }

    /// <summary>
    /// Creates a comprehensive audio model for the question.
    /// </summary>
    private QuestionAudioModel CreateQuestionAudioModel(QuestionModelBase question, int childAge, string language)
    {
        var audioModel = new QuestionAudioModel
        {
            QuestionId = question.QuestionId.ToString(),
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            TargetAgeGroup = GetAgeGroup(childAge),
            Subject = GetSubjectFromQuestion(question),
            DifficultyLevel = question.DifficultyLevel ?? "medium",
            RequiresSlowSpeech = childAge <= 4,
            SupportsRepetition = true
        };

        // Add question audio paths
        if (!string.IsNullOrEmpty(question.AudioPath))
        {
            audioModel.QuestionAudio[language] = question.AudioPath;
        }

        // Add answer choice audio if applicable
        if (question is MultipleChoiceQuestionModel mcQuestion && mcQuestion.AnswerChoices.Any())
        {
            for (int i = 0; i < mcQuestion.AnswerChoices.Count; i++)
            {
                var choiceAudio = new Dictionary<string, string>();
                // In a real implementation, these would come from resource files
                choiceAudio[language] = $"audio/choices/{question.QuestionId}_choice_{i}_{language}.mp3";
                audioModel.AnswerChoiceAudio[i] = choiceAudio;
            }
        }

        // Add hint audio
        if (!string.IsNullOrEmpty(question.HintText))
        {
            for (int level = 1; level <= 3; level++)
            {
                var hintAudio = new Dictionary<string, string>();
                hintAudio[language] = $"audio/hints/{question.QuestionId}_hint_{level}_{language}.mp3";
                audioModel.HintAudio[level] = hintAudio;
            }
        }

        // Add visual description if image is present
        if (!string.IsNullOrEmpty(question.ImagePath))
        {
            audioModel.VisualDescriptionAudio[language] = $"audio/descriptions/{question.QuestionId}_description_{language}.mp3";
        }

        // Add interaction instructions
        audioModel.InteractionInstructions[language] = GetInteractionInstructions(question.QuestionType, language);

        // Optimize for child's age
        return audioModel.OptimizeForAge(childAge);
    }

    #endregion

    #region Question Audio Playback

    /// <summary>
    /// Plays the complete question audio sequence.
    /// </summary>
    /// <param name="includeChoices">Whether to include answer choices</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PlayQuestionSequenceAsync(bool includeChoices = true, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null)
            {
                _logger.LogWarning("No question audio model available");
                return;
            }

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.QuestionNarrationStarted)
            {
                QuestionId = _questionAudio.QuestionId,
                QuestionAudio = _questionAudio,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(eventArgs);

            // Play interaction instructions first
            await PlayInteractionInstructionsAsync(cancellationToken);

            // Brief pause
            await DelayForAge(500, cancellationToken);

            // Play the main question
            await PlayQuestionTextAsync(cancellationToken);

            // Play answer choices if applicable and requested
            if (includeChoices && _questionAudio.AnswerChoiceAudio.Any())
            {
                await DelayForAge(800, cancellationToken);
                await PlayAnswerChoicesSequenceAsync(cancellationToken);
            }

            // Fire completion event
            var completedEventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.QuestionNarrationCompleted)
            {
                QuestionId = _questionAudio.QuestionId,
                QuestionAudio = _questionAudio,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(completedEventArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing question sequence");
        }
    }

    /// <summary>
    /// Plays the question text audio.
    /// </summary>
    public async Task PlayQuestionTextAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null) return;

            var questionAudio = _questionAudio.GetQuestionAudio(_language);
            if (!string.IsNullOrEmpty(questionAudio))
            {
                await _audioService.PlayAudioAsync(questionAudio, AudioType.Instruction, AudioPriority.High, cancellationToken);
            }
            else
            {
                // Fallback to TTS
                await _audioService.PlayQuestionAudioAsync(_questionAudio.QuestionText, _questionAudio.QuestionType, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing question text audio");
        }
    }

    /// <summary>
    /// Plays interaction instructions for the question type.
    /// </summary>
    public async Task PlayInteractionInstructionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null) return;

            var instructions = _questionAudio.GetInteractionInstructions(_language);
            if (!string.IsNullOrEmpty(instructions))
            {
                await _audioService.PlayQuestionAudioAsync(instructions, "interaction_instruction", cancellationToken);

                var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.InteractionInstructionsPlayed)
                {
                    QuestionId = _questionAudio.QuestionId,
                    ChildAge = _childAge,
                    Language = _language
                };

                OnQuestionAudioEvent(eventArgs);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing interaction instructions");
        }
    }

    /// <summary>
    /// Plays all answer choices in sequence.
    /// </summary>
    public async Task PlayAnswerChoicesSequenceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null || !_questionAudio.AnswerChoiceAudio.Any()) return;

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.AnswerChoiceReadingStarted)
            {
                QuestionId = _questionAudio.QuestionId,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(eventArgs);

            // Use the educational audio service for answer choices
            var choices = _questionAudio.GetAllAnswerChoiceAudio(_language);
            if (choices.Any())
            {
                var choiceDelay = _questionAudio.AudioSettings.BetweenChoicesPause;
                await _audioService.PlayAnswerChoicesAsync(choices, choiceDelay, cancellationToken);
            }

            var completedEventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.AnswerChoiceReadingCompleted)
            {
                QuestionId = _questionAudio.QuestionId,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(completedEventArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing answer choices");
        }
    }

    #endregion

    #region Hint and Help Audio

    /// <summary>
    /// Plays progressive hint audio.
    /// </summary>
    public async Task PlayHintAudioAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null) return;

            var hintAudio = _questionAudio.GetHintAudio(_hintLevel, _language);
            if (!string.IsNullOrEmpty(hintAudio))
            {
                await _audioService.PlayHintAudioAsync(hintAudio, _hintLevel, cancellationToken);
            }
            else if (!string.IsNullOrEmpty(_currentQuestion?.HintText))
            {
                // Fallback to hint text
                await _audioService.PlayHintAudioAsync(_currentQuestion.HintText, _hintLevel, cancellationToken);
            }

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.HintAudioPlayed)
            {
                QuestionId = _questionAudio.QuestionId,
                HintLevel = _hintLevel,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(eventArgs);

            // Increment hint level for next time
            _hintLevel = Math.Min(_hintLevel + 1, 3);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing hint audio");
        }
    }

    /// <summary>
    /// Plays visual description for accessibility.
    /// </summary>
    public async Task PlayVisualDescriptionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null) return;

            var visualDescription = _questionAudio.GetVisualDescriptionAudio(_language);
            if (!string.IsNullOrEmpty(visualDescription))
            {
                await _audioService.PlayVisualDescriptionAsync(visualDescription, "question_image", cancellationToken);

                var eventArgs = new AccessibilityAudioEventArgs(AccessibilityAudioEventType.VisualDescriptionPlayed)
                {
                    TargetElement = "question_image",
                    ElementType = "image",
                    UserRequested = true,
                    Language = _language
                };

                OnAccessibilityAudioEvent(eventArgs);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing visual description");
        }
    }

    /// <summary>
    /// Repeats the question audio for accessibility.
    /// </summary>
    public async Task RepeatQuestionAsync(string repetitionSpeed = "normal", CancellationToken cancellationToken = default)
    {
        try
        {
            if (_questionAudio == null) return;

            _repetitionCount++;

            // Use the educational audio service for repeated questions
            await _audioService.PlayRepeatedQuestionAsync(_questionAudio.QuestionText, repetitionSpeed, cancellationToken);

            var eventArgs = new QuestionAudioEventArgs(QuestionAudioEventType.QuestionRepeated)
            {
                QuestionId = _questionAudio.QuestionId,
                IsRepetition = true,
                RepetitionCount = _repetitionCount,
                ChildAge = _childAge,
                Language = _language
            };

            OnQuestionAudioEvent(eventArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error repeating question");
        }
    }

    #endregion

    #region Feedback Audio

    /// <summary>
    /// Plays success feedback for correct answers.
    /// </summary>
    public async Task PlaySuccessFeedbackAsync(FeedbackIntensity intensity = FeedbackIntensity.Medium, CancellationToken cancellationToken = default)
    {
        try
        {
            await _audioService.PlaySuccessFeedbackAsync(intensity, _childAge, cancellationToken);
            _logger.LogDebug("Success feedback played for question {QuestionId}", _questionAudio?.QuestionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing success feedback");
        }
    }

    /// <summary>
    /// Plays encouraging feedback for incorrect answers.
    /// </summary>
    public async Task PlayEncouragementFeedbackAsync(FeedbackIntensity encouragement = FeedbackIntensity.Soft, CancellationToken cancellationToken = default)
    {
        try
        {
            await _audioService.PlayErrorFeedbackAsync(encouragement, _childAge, cancellationToken);
            _logger.LogDebug("Encouragement feedback played for question {QuestionId}", _questionAudio?.QuestionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing encouragement feedback");
        }
    }

    #endregion

    #region Helper Methods

    private async Task PreloadQuestionAudioAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_questionAudio == null) return;

            var audioKeys = new List<string>();

            // Add question audio
            var questionAudio = _questionAudio.GetQuestionAudio(_language);
            if (!string.IsNullOrEmpty(questionAudio))
                audioKeys.Add(questionAudio);

            // Add choice audio
            foreach (var choice in _questionAudio.AnswerChoiceAudio.Values)
            {
                if (choice.TryGetValue(_language, out var choiceAudio))
                    audioKeys.Add(choiceAudio);
            }

            // Add hint audio
            foreach (var hint in _questionAudio.HintAudio.Values)
            {
                if (hint.TryGetValue(_language, out var hintAudio))
                    audioKeys.Add(hintAudio);
            }

            if (audioKeys.Any())
            {
                await _audioService.PreloadAudioAsync(audioKeys, cancellationToken);
                _logger.LogDebug("Preloaded {Count} audio files for question {QuestionId}", audioKeys.Count, _questionAudio.QuestionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not preload audio (not critical): {Message}", ex.Message);
        }
    }

    private async Task DelayForAge(int baseDelayMs, CancellationToken cancellationToken)
    {
        var adjustedDelay = _childAge switch
        {
            <= 4 => (int)(baseDelayMs * 1.5),
            5 => (int)(baseDelayMs * 1.2),
            _ => baseDelayMs
        };

        await Task.Delay(adjustedDelay, cancellationToken);
    }

    private AgeGroup GetAgeGroup(int childAge)
    {
        return childAge switch
        {
            <= 4 => AgeGroup.PreK,
            5 => AgeGroup.Kindergarten,
            _ => AgeGroup.Primary
        };
    }

    private string GetSubjectFromQuestion(QuestionModelBase question)
    {
        // Extract subject from question metadata or question type
        if (!string.IsNullOrEmpty(question.Metadata))
        {
            try
            {
                var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(question.Metadata);
                return metadata?.TryGetValue("subject", out var subject) == true
                    ? subject?.ToString() ?? "general"
                    : "general";
            }
            catch (JsonException)
            {
                // Fall back to general if JSON parsing fails
                return "general";
            }
        }
        return "general";
    }

    private string GetInteractionInstructions(string questionType, string language)
    {
        return questionType.ToLower() switch
        {
            "multiplechoice" => language == "es" ? "Toca la respuesta correcta." : "Tap the correct answer.",
            "draganddrop" => language == "es" ? "Arrastra cada elemento a su lugar." : "Drag each item to its place.",
            "matching" => language == "es" ? "Conecta los elementos que van juntos." : "Connect the items that go together.",
            "tracing" => language == "es" ? "Traza la línea con tu dedo." : "Trace the line with your finger.",
            _ => language == "es" ? "Completa esta actividad." : "Complete this activity."
        };
    }

    private void SubscribeToAudioServiceEvents()
    {
        _audioService.AudioStarted += OnAudioServiceStarted;
        _audioService.AudioStopped += OnAudioServiceStopped;
        _audioService.AudioError += OnAudioServiceError;
    }

    #endregion

    #region Event Handlers

    private void OnAudioServiceStarted(object? sender, AudioPlaybackEventArgs e)
    {
        _logger.LogDebug("Audio service started: {AudioId}", e.AudioId);
    }

    private void OnAudioServiceStopped(object? sender, AudioPlaybackEventArgs e)
    {
        _logger.LogDebug("Audio service stopped: {AudioId}", e.AudioId);
    }

    private void OnAudioServiceError(object? sender, AudioErrorEventArgs e)
    {
        _logger.LogWarning("Audio service error: {ErrorMessage}", e.ErrorMessage);
    }

    protected virtual void OnQuestionAudioEvent(QuestionAudioEventArgs eventArgs)
    {
        QuestionAudioEvent?.Invoke(this, eventArgs);
    }

    protected virtual void OnAccessibilityAudioEvent(AccessibilityAudioEventArgs eventArgs)
    {
        AccessibilityAudioEvent?.Invoke(this, eventArgs);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the current question audio model.
    /// </summary>
    public QuestionAudioModel? CurrentQuestionAudio => _questionAudio;

    /// <summary>
    /// Gets the current repetition count.
    /// </summary>
    public int RepetitionCount => _repetitionCount;

    /// <summary>
    /// Gets the current hint level.
    /// </summary>
    public int CurrentHintLevel => _hintLevel;

    /// <summary>
    /// Gets whether audio is currently playing.
    /// </summary>
    public bool IsAudioPlaying => _audioService.IsAnyAudioPlaying();

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _audioService.AudioStarted -= OnAudioServiceStarted;
            _audioService.AudioStopped -= OnAudioServiceStopped;
            _audioService.AudioError -= OnAudioServiceError;

            _audioLock?.Dispose();
            _disposed = true;
        }
    }

    #endregion
}