namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Model representing educational audio content with bilingual support and age-appropriate configuration.
/// Used for activity instructions, question narration, and educational guidance.
/// </summary>
public class EducationalAudioContent
{
    /// <summary>
    /// Unique identifier for the educational audio content.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Localization key for finding the appropriate audio file.
    /// Format: "activity_type.instruction_type" (e.g., "math.addition_intro")
    /// </summary>
    public string LocalizationKey { get; set; } = string.Empty;

    /// <summary>
    /// Type of educational content this audio represents.
    /// </summary>
    public EducationalContentType ContentType { get; set; }

    /// <summary>
    /// Target age group for this educational content.
    /// </summary>
    public AgeGroup TargetAgeGroup { get; set; }

    /// <summary>
    /// Language-specific audio file paths.
    /// Key: language code (e.g., "en", "es"), Value: file path
    /// </summary>
    public Dictionary<string, string> AudioPaths { get; set; } = new();

    /// <summary>
    /// Duration of the audio content in milliseconds.
    /// </summary>
    public int DurationMs { get; set; }

    /// <summary>
    /// Transcript of the audio content for accessibility and testing.
    /// </summary>
    public Dictionary<string, string> Transcripts { get; set; } = new();

    /// <summary>
    /// Educational subject this audio content belongs to.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Activity type this audio supports (e.g., "multiple_choice", "drag_drop").
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty level this audio is appropriate for.
    /// </summary>
    public string DifficultyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Whether this audio requires special accessibility considerations.
    /// </summary>
    public bool RequiresAccessibilitySupport { get; set; }

    /// <summary>
    /// Metadata tags for organizing and searching educational audio content.
    /// </summary>
    public HashSet<string> Tags { get; set; } = new();

    /// <summary>
    /// Priority level for this educational content within an activity.
    /// </summary>
    public AudioPriority Priority { get; set; } = AudioPriority.Normal;

    /// <summary>
    /// Optional parameters for dynamic audio generation or customization.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Gets the audio file path for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Audio file path or null if not available</returns>
    public string? GetAudioPath(string languageCode)
    {
        return AudioPaths.TryGetValue(languageCode, out var path) ? path : null;
    }

    /// <summary>
    /// Gets the transcript for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Transcript text or empty string if not available</returns>
    public string GetTranscript(string languageCode)
    {
        return Transcripts.TryGetValue(languageCode, out var transcript) ? transcript : string.Empty;
    }

    /// <summary>
    /// Checks if this content is appropriate for the specified age.
    /// </summary>
    /// <param name="childAge">Child's age in years</param>
    /// <returns>True if content is age-appropriate, false otherwise</returns>
    public bool IsAppropriateForAge(int childAge)
    {
        return TargetAgeGroup switch
        {
            AgeGroup.PreK => childAge >= 3 && childAge <= 4,
            AgeGroup.Kindergarten => childAge == 5,
            AgeGroup.Primary => childAge >= 6 && childAge <= 8,
            _ => true
        };
    }

    /// <summary>
    /// Creates a copy of this educational audio content with different parameters.
    /// </summary>
    /// <param name="newParameters">New parameters to apply</param>
    /// <returns>New instance with updated parameters</returns>
    public EducationalAudioContent WithParameters(Dictionary<string, object> newParameters)
    {
        return new EducationalAudioContent
        {
            Id = Id,
            LocalizationKey = LocalizationKey,
            ContentType = ContentType,
            TargetAgeGroup = TargetAgeGroup,
            AudioPaths = new Dictionary<string, string>(AudioPaths),
            DurationMs = DurationMs,
            Transcripts = new Dictionary<string, string>(Transcripts),
            Subject = Subject,
            ActivityType = ActivityType,
            DifficultyLevel = DifficultyLevel,
            RequiresAccessibilitySupport = RequiresAccessibilitySupport,
            Tags = new HashSet<string>(Tags),
            Priority = Priority,
            Parameters = new Dictionary<string, object>(newParameters)
        };
    }
}

/// <summary>
/// Defines the types of educational audio content in the EduPlayKids application.
/// </summary>
public enum EducationalContentType
{
    /// <summary>
    /// Activity introduction and overview narration.
    /// </summary>
    ActivityIntroduction,

    /// <summary>
    /// Step-by-step instruction guidance.
    /// </summary>
    StepByStepGuidance,

    /// <summary>
    /// Question text narration for educational activities.
    /// </summary>
    QuestionNarration,

    /// <summary>
    /// Answer choice reading for multiple choice questions.
    /// </summary>
    AnswerChoiceReading,

    /// <summary>
    /// Hint and help system audio content.
    /// </summary>
    HintAndHelp,

    /// <summary>
    /// Progress encouragement and motivation.
    /// </summary>
    ProgressEncouragement,

    /// <summary>
    /// Visual element description for accessibility.
    /// </summary>
    VisualDescription,

    /// <summary>
    /// Educational concept explanation and clarification.
    /// </summary>
    ConceptExplanation,

    /// <summary>
    /// Activity summary and review content.
    /// </summary>
    ActivitySummary,

    /// <summary>
    /// Transition audio between different activity sections.
    /// </summary>
    Transition
}