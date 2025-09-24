namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Event arguments for educational audio events in the EduPlayKids application.
/// Provides context-specific information for educational audio playback events.
/// </summary>
public class EducationalAudioEventArgs : EventArgs
{
    /// <summary>
    /// Type of educational audio event.
    /// </summary>
    public EducationalAudioEventType EventType { get; set; }

    /// <summary>
    /// Unique identifier for the audio content.
    /// </summary>
    public string AudioId { get; set; } = string.Empty;

    /// <summary>
    /// Educational content that triggered the event.
    /// </summary>
    public EducationalAudioContent? Content { get; set; }

    /// <summary>
    /// Question audio model if the event is question-related.
    /// </summary>
    public QuestionAudioModel? QuestionAudio { get; set; }

    /// <summary>
    /// Child's age for age-appropriate event handling.
    /// </summary>
    public int? ChildAge { get; set; }

    /// <summary>
    /// Language used for the audio playback.
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Educational context information.
    /// </summary>
    public EducationalContext Context { get; set; } = new();

    /// <summary>
    /// Timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the audio content in milliseconds.
    /// </summary>
    public int DurationMs { get; set; }

    /// <summary>
    /// Whether the event was successful.
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Error message if the event was not successful.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional metadata for the event.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    public EducationalAudioEventArgs(EducationalAudioEventType eventType)
    {
        EventType = eventType;
    }
}

/// <summary>
/// Event arguments specifically for question audio events.
/// </summary>
public class QuestionAudioEventArgs : EducationalAudioEventArgs
{
    /// <summary>
    /// Question ID that the audio event relates to.
    /// </summary>
    public string QuestionId { get; set; } = string.Empty;

    /// <summary>
    /// Type of question audio event.
    /// </summary>
    public QuestionAudioEventType QuestionEventType { get; set; }

    /// <summary>
    /// Index of the answer choice if applicable.
    /// </summary>
    public int? AnswerChoiceIndex { get; set; }

    /// <summary>
    /// Hint level if this is a hint audio event.
    /// </summary>
    public int? HintLevel { get; set; }

    /// <summary>
    /// Whether this is a repeated playback of the same content.
    /// </summary>
    public bool IsRepetition { get; set; }

    /// <summary>
    /// Number of times this content has been repeated for the child.
    /// </summary>
    public int RepetitionCount { get; set; }

    public QuestionAudioEventArgs(QuestionAudioEventType questionEventType)
        : base(EducationalAudioEventType.QuestionAudio)
    {
        QuestionEventType = questionEventType;
    }
}

/// <summary>
/// Event arguments for activity audio events.
/// </summary>
public class ActivityAudioEventArgs : EducationalAudioEventArgs
{
    /// <summary>
    /// Activity ID that the audio event relates to.
    /// </summary>
    public string ActivityId { get; set; } = string.Empty;

    /// <summary>
    /// Type of activity audio event.
    /// </summary>
    public ActivityAudioEventType ActivityEventType { get; set; }

    /// <summary>
    /// Progress percentage when the event occurred (0-100).
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Current step in a step-by-step activity.
    /// </summary>
    public int? CurrentStep { get; set; }

    /// <summary>
    /// Total number of steps in the activity.
    /// </summary>
    public int? TotalSteps { get; set; }

    /// <summary>
    /// Achievement data if this event relates to an achievement.
    /// </summary>
    public AchievementAudioData? Achievement { get; set; }

    public ActivityAudioEventArgs(ActivityAudioEventType activityEventType)
        : base(EducationalAudioEventType.ActivityAudio)
    {
        ActivityEventType = activityEventType;
    }
}

/// <summary>
/// Event arguments for accessibility audio events.
/// </summary>
public class AccessibilityAudioEventArgs : EducationalAudioEventArgs
{
    /// <summary>
    /// Type of accessibility audio event.
    /// </summary>
    public AccessibilityAudioEventType AccessibilityEventType { get; set; }

    /// <summary>
    /// Element that requires accessibility support.
    /// </summary>
    public string TargetElement { get; set; } = string.Empty;

    /// <summary>
    /// Type of visual element being described.
    /// </summary>
    public string ElementType { get; set; } = string.Empty;

    /// <summary>
    /// Description text being narrated.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether this accessibility audio was requested by the child.
    /// </summary>
    public bool UserRequested { get; set; }

    public AccessibilityAudioEventArgs(AccessibilityAudioEventType accessibilityEventType)
        : base(EducationalAudioEventType.AccessibilityAudio)
    {
        AccessibilityEventType = accessibilityEventType;
    }
}

/// <summary>
/// Educational context information for audio events.
/// </summary>
public class EducationalContext
{
    /// <summary>
    /// Current subject being studied.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Activity type currently active.
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty level of current content.
    /// </summary>
    public string DifficultyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Learning objective for the current activity.
    /// </summary>
    public string LearningObjective { get; set; } = string.Empty;

    /// <summary>
    /// Session information for analytics.
    /// </summary>
    public SessionContext Session { get; set; } = new();

    /// <summary>
    /// Child's performance metrics in current session.
    /// </summary>
    public PerformanceMetrics Performance { get; set; } = new();
}

/// <summary>
/// Session context for educational audio events.
/// </summary>
public class SessionContext
{
    /// <summary>
    /// Unique session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Child ID for this session.
    /// </summary>
    public int ChildId { get; set; }

    /// <summary>
    /// When the session started.
    /// </summary>
    public DateTime SessionStart { get; set; }

    /// <summary>
    /// Duration of current session in minutes.
    /// </summary>
    public int SessionDurationMinutes { get; set; }

    /// <summary>
    /// Number of activities completed in this session.
    /// </summary>
    public int ActivitiesCompleted { get; set; }
}

/// <summary>
/// Performance metrics for the child in the current context.
/// </summary>
public class PerformanceMetrics
{
    /// <summary>
    /// Accuracy percentage for current activity (0-100).
    /// </summary>
    public int AccuracyPercentage { get; set; }

    /// <summary>
    /// Number of correct answers in current activity.
    /// </summary>
    public int CorrectAnswers { get; set; }

    /// <summary>
    /// Total number of questions attempted.
    /// </summary>
    public int TotalQuestions { get; set; }

    /// <summary>
    /// Number of hints used in current activity.
    /// </summary>
    public int HintsUsed { get; set; }

    /// <summary>
    /// Number of times audio was repeated.
    /// </summary>
    public int AudioRepetitions { get; set; }

    /// <summary>
    /// Average response time in seconds.
    /// </summary>
    public double AverageResponseTimeSeconds { get; set; }
}

/// <summary>
/// Achievement data for audio events.
/// </summary>
public class AchievementAudioData
{
    /// <summary>
    /// Achievement identifier.
    /// </summary>
    public string AchievementId { get; set; } = string.Empty;

    /// <summary>
    /// Type of achievement (milestone, streak, completion, etc.).
    /// </summary>
    public string AchievementType { get; set; } = string.Empty;

    /// <summary>
    /// Level or value of the achievement.
    /// </summary>
    public int AchievementLevel { get; set; }

    /// <summary>
    /// Stars earned (1-3) if applicable.
    /// </summary>
    public int? StarsEarned { get; set; }

    /// <summary>
    /// Whether this unlocks a crown challenge.
    /// </summary>
    public bool UnlocksCrownChallenge { get; set; }

    /// <summary>
    /// Celebration intensity for this achievement.
    /// </summary>
    public FeedbackIntensity CelebrationIntensity { get; set; } = FeedbackIntensity.Medium;
}

/// <summary>
/// Types of educational audio events.
/// </summary>
public enum EducationalAudioEventType
{
    /// <summary>
    /// Question-related audio event.
    /// </summary>
    QuestionAudio,

    /// <summary>
    /// Activity-related audio event.
    /// </summary>
    ActivityAudio,

    /// <summary>
    /// Accessibility-related audio event.
    /// </summary>
    AccessibilityAudio,

    /// <summary>
    /// Achievement or celebration audio event.
    /// </summary>
    AchievementAudio,

    /// <summary>
    /// Instruction or guidance audio event.
    /// </summary>
    InstructionAudio,

    /// <summary>
    /// Feedback audio event (success or error).
    /// </summary>
    FeedbackAudio,

    /// <summary>
    /// Background or ambient audio event.
    /// </summary>
    BackgroundAudio
}

/// <summary>
/// Types of question audio events.
/// </summary>
public enum QuestionAudioEventType
{
    /// <summary>
    /// Question text narration started.
    /// </summary>
    QuestionNarrationStarted,

    /// <summary>
    /// Question text narration completed.
    /// </summary>
    QuestionNarrationCompleted,

    /// <summary>
    /// Answer choice reading started.
    /// </summary>
    AnswerChoiceReadingStarted,

    /// <summary>
    /// Answer choice reading completed.
    /// </summary>
    AnswerChoiceReadingCompleted,

    /// <summary>
    /// Hint audio played.
    /// </summary>
    HintAudioPlayed,

    /// <summary>
    /// Visual description narrated.
    /// </summary>
    VisualDescriptionNarrated,

    /// <summary>
    /// Question audio repeated.
    /// </summary>
    QuestionRepeated,

    /// <summary>
    /// Interaction instructions played.
    /// </summary>
    InteractionInstructionsPlayed
}

/// <summary>
/// Types of activity audio events.
/// </summary>
public enum ActivityAudioEventType
{
    /// <summary>
    /// Activity introduction played.
    /// </summary>
    IntroductionPlayed,

    /// <summary>
    /// Step-by-step guidance started.
    /// </summary>
    StepGuidanceStarted,

    /// <summary>
    /// Progress encouragement played.
    /// </summary>
    ProgressEncouragementPlayed,

    /// <summary>
    /// Activity completion celebration.
    /// </summary>
    CompletionCelebration,

    /// <summary>
    /// Crown challenge unlocked celebration.
    /// </summary>
    CrownChallengeUnlocked,

    /// <summary>
    /// Milestone achievement celebration.
    /// </summary>
    MilestoneAchieved,

    /// <summary>
    /// Learning streak celebration.
    /// </summary>
    StreakCelebration
}

/// <summary>
/// Types of accessibility audio events.
/// </summary>
public enum AccessibilityAudioEventType
{
    /// <summary>
    /// Visual element description played.
    /// </summary>
    VisualDescriptionPlayed,

    /// <summary>
    /// Audio repeated for accessibility.
    /// </summary>
    AccessibilityRepetition,

    /// <summary>
    /// Help system audio played.
    /// </summary>
    HelpSystemAudioPlayed,

    /// <summary>
    /// Navigation assistance provided.
    /// </summary>
    NavigationAssistance,

    /// <summary>
    /// Accessibility feature activated.
    /// </summary>
    AccessibilityFeatureActivated
}