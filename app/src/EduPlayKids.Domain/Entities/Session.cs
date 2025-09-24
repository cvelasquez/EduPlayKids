using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents a learning session for a child in the EduPlayKids application.
/// Tracks usage patterns, engagement metrics, and screen time for COPPA-compliant analytics.
/// Core entity for monitoring healthy learning habits and parental controls.
/// </summary>
public class Session : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the child this session belongs to.
    /// Links session data to the specific learner.
    /// </summary>
    [Required]
    public int ChildId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session started.
    /// Precise tracking for usage analytics and screen time management.
    /// </summary>
    [Required]
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session ended.
    /// Null for active sessions, set when session concludes.
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Gets or sets the total duration of the session in seconds.
    /// Calculated when session ends, used for screen time tracking.
    /// </summary>
    public int DurationSeconds { get; set; } = 0;

    /// <summary>
    /// Gets or sets the session type identifier.
    /// Categories: Learning, Practice, Assessment, Free_Play, Crown_Challenge
    /// </summary>
    [Required]
    [StringLength(50)]
    public string SessionType { get; set; } = "Learning";

    /// <summary>
    /// Gets or sets a value indicating whether the session is currently active.
    /// True for ongoing sessions, false for completed sessions.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the device identifier used for this session.
    /// COPPA-compliant device tracking without personal information.
    /// </summary>
    [StringLength(100)]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the application version used during this session.
    /// Important for analytics and bug tracking across app versions.
    /// </summary>
    [StringLength(20)]
    public string? AppVersion { get; set; }

    /// <summary>
    /// Gets or sets the number of activities completed during this session.
    /// Key engagement metric for learning effectiveness.
    /// </summary>
    public int ActivitiesCompleted { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of questions answered during this session.
    /// Detailed engagement tracking for content optimization.
    /// </summary>
    public int QuestionsAnswered { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of correct answers given during this session.
    /// Performance metric for learning assessment.
    /// </summary>
    public int CorrectAnswers { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total stars earned during this session.
    /// Motivation and achievement tracking metric.
    /// </summary>
    public int StarsEarned { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total points scored during this session.
    /// Gamification and progress tracking metric.
    /// </summary>
    public int PointsScored { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of achievements earned during this session.
    /// Special accomplishment tracking for motivation.
    /// </summary>
    public int AchievementsEarned { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of hints used during this session.
    /// Indicates level of support needed for content difficulty.
    /// </summary>
    public int HintsUsed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the subjects accessed during this session.
    /// JSON array of subject IDs for curriculum coverage tracking.
    /// </summary>
    public string? SubjectsAccessed { get; set; }

    /// <summary>
    /// Gets or sets the primary language used during this session.
    /// Tracks bilingual usage patterns and preferences.
    /// </summary>
    [StringLength(10)]
    public string? LanguageUsed { get; set; }

    /// <summary>
    /// Gets or sets the number of times audio instructions were played.
    /// Accessibility and engagement metric for non-readers.
    /// </summary>
    public int AudioPlaysCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the interaction patterns during this session.
    /// JSON structure tracking:
    /// - Screen touches/clicks
    /// - Navigation patterns
    /// - Pause/resume events
    /// - Help-seeking behavior
    /// </summary>
    public string? InteractionPatterns { get; set; }

    /// <summary>
    /// Gets or sets the emotional states observed during this session.
    /// JSON array of emotion tracking for engagement analysis.
    /// Values: Happy, Frustrated, Excited, Confused, Focused, etc.
    /// </summary>
    public string? EmotionalStates { get; set; }

    /// <summary>
    /// Gets or sets the performance metrics for this session.
    /// JSON structure containing:
    /// - Average response time
    /// - Accuracy trends
    /// - Difficulty progression
    /// - Learning velocity
    /// </summary>
    public string? PerformanceMetrics { get; set; }

    /// <summary>
    /// Gets or sets the reason the session ended.
    /// Values: User_Exit, Time_Limit, Parent_Stop, App_Close, Crash, etc.
    /// Important for understanding session completion patterns.
    /// </summary>
    [StringLength(50)]
    public string? EndReason { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parental controls were active.
    /// COPPA compliance tracking for supervised usage.
    /// </summary>
    public bool ParentalControlsActive { get; set; } = false;

    /// <summary>
    /// Gets or sets the daily usage limit that was active during this session.
    /// Screen time management and healthy usage tracking.
    /// </summary>
    public int? DailyLimitMinutes { get; set; }

    /// <summary>
    /// Gets or sets the total screen time used today before this session.
    /// Cumulative daily usage for limit enforcement.
    /// </summary>
    public int PreviousDailyUsageMinutes { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether this session exceeded time limits.
    /// Flag for parental notification and usage pattern analysis.
    /// </summary>
    public bool ExceededTimeLimit { get; set; } = false;

    /// <summary>
    /// Gets or sets technical metadata for this session.
    /// JSON structure for debugging and optimization:
    /// - Memory usage patterns
    /// - Performance metrics
    /// - Error logs
    /// - Feature usage statistics
    /// </summary>
    public string? TechnicalMetadata { get; set; }

    /// <summary>
    /// Gets or sets COPPA-compliant privacy settings for this session.
    /// JSON structure defining data collection permissions and restrictions.
    /// </summary>
    public string? PrivacySettings { get; set; }

    /// <summary>
    /// Navigation property: The child this session belongs to.
    /// </summary>
    public virtual Child Child { get; set; } = null!;

    /// <summary>
    /// Gets the duration in minutes for convenience (calculated from DurationSeconds).
    /// </summary>
    public double DurationMinutes => DurationSeconds / 60.0;

    /// <summary>
    /// Gets the completion time (alias for EndedAt for ViewModel compatibility).
    /// </summary>
    public DateTime? CompletedAt => EndedAt;

    /// <summary>
    /// Initializes a new instance of the Session class.
    /// Sets default values for new session tracking.
    /// </summary>
    public Session()
    {
        StartedAt = DateTime.UtcNow;
        IsActive = true;
        SessionType = "Learning";
        DurationSeconds = 0;
        ActivitiesCompleted = 0;
        QuestionsAnswered = 0;
        CorrectAnswers = 0;
        StarsEarned = 0;
        PointsScored = 0;
        AchievementsEarned = 0;
        HintsUsed = 0;
        AudioPlaysCount = 0;
        ParentalControlsActive = false;
        PreviousDailyUsageMinutes = 0;
        ExceededTimeLimit = false;
    }

    /// <summary>
    /// Ends the current session and calculates final metrics.
    /// </summary>
    /// <param name="endReason">The reason the session ended.</param>
    public void EndSession(string endReason = "User_Exit")
    {
        if (!IsActive) return;

        EndedAt = DateTime.UtcNow;
        IsActive = false;
        EndReason = endReason;
        DurationSeconds = (int)(EndedAt.Value - StartedAt).TotalSeconds;

        // Check if time limit was exceeded
        if (DailyLimitMinutes.HasValue)
        {
            var totalDailyUsage = PreviousDailyUsageMinutes + (DurationSeconds / 60);
            ExceededTimeLimit = totalDailyUsage > DailyLimitMinutes.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates session statistics with new activity completion.
    /// </summary>
    /// <param name="questionsInActivity">Number of questions in the completed activity.</param>
    /// <param name="correctAnswersInActivity">Number of correct answers in the activity.</param>
    /// <param name="starsEarnedInActivity">Stars earned in the activity.</param>
    /// <param name="pointsEarnedInActivity">Points earned in the activity.</param>
    /// <param name="hintsUsedInActivity">Hints used in the activity.</param>
    public void UpdateActivityStats(int questionsInActivity, int correctAnswersInActivity,
        int starsEarnedInActivity, int pointsEarnedInActivity, int hintsUsedInActivity = 0)
    {
        ActivitiesCompleted++;
        QuestionsAnswered += questionsInActivity;
        CorrectAnswers += correctAnswersInActivity;
        StarsEarned += starsEarnedInActivity;
        PointsScored += pointsEarnedInActivity;
        HintsUsed += hintsUsedInActivity;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records an achievement earned during this session.
    /// </summary>
    public void RecordAchievementEarned()
    {
        AchievementsEarned++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records audio instruction usage for accessibility tracking.
    /// </summary>
    public void RecordAudioPlay()
    {
        AudioPlaysCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the accuracy percentage for this session.
    /// </summary>
    /// <returns>Accuracy as a percentage (0-100).</returns>
    public double GetAccuracy()
    {
        if (QuestionsAnswered == 0) return 0;
        return Math.Round((double)CorrectAnswers / QuestionsAnswered * 100, 1);
    }

    /// <summary>
    /// Gets the current session duration in minutes.
    /// </summary>
    /// <returns>Duration in minutes.</returns>
    public int GetDurationMinutes()
    {
        if (IsActive)
        {
            return (int)(DateTime.UtcNow - StartedAt).TotalMinutes;
        }
        return DurationSeconds / 60;
    }

    /// <summary>
    /// Determines if the session is approaching the daily time limit.
    /// </summary>
    /// <param name="warningMinutes">Minutes before limit to trigger warning.</param>
    /// <returns>True if approaching limit, false otherwise.</returns>
    public bool IsApproachingTimeLimit(int warningMinutes = 5)
    {
        if (!DailyLimitMinutes.HasValue || !IsActive) return false;

        var currentDailyUsage = PreviousDailyUsageMinutes + GetDurationMinutes();
        var remainingTime = DailyLimitMinutes.Value - currentDailyUsage;

        return remainingTime <= warningMinutes && remainingTime > 0;
    }

    /// <summary>
    /// Determines if the daily time limit has been reached.
    /// </summary>
    /// <returns>True if time limit reached, false otherwise.</returns>
    public bool HasReachedTimeLimit()
    {
        if (!DailyLimitMinutes.HasValue) return false;

        var currentDailyUsage = PreviousDailyUsageMinutes + GetDurationMinutes();
        return currentDailyUsage >= DailyLimitMinutes.Value;
    }

    /// <summary>
    /// Gets the remaining daily usage time in minutes.
    /// </summary>
    /// <returns>Remaining minutes, or null if no limit set.</returns>
    public int? GetRemainingDailyMinutes()
    {
        if (!DailyLimitMinutes.HasValue) return null;

        var currentDailyUsage = PreviousDailyUsageMinutes + GetDurationMinutes();
        var remaining = DailyLimitMinutes.Value - currentDailyUsage;

        return Math.Max(0, remaining);
    }

    /// <summary>
    /// Adds a subject to the list of subjects accessed during this session.
    /// </summary>
    /// <param name="subjectId">The ID of the subject being accessed.</param>
    public void AddSubjectAccessed(int subjectId)
    {
        try
        {
            var subjects = string.IsNullOrEmpty(SubjectsAccessed)
                ? new List<int>()
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(SubjectsAccessed) ?? new List<int>();

            if (!subjects.Contains(subjectId))
            {
                subjects.Add(subjectId);
                SubjectsAccessed = System.Text.Json.JsonSerializer.Serialize(subjects);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        catch
        {
            // If JSON parsing fails, create new list
            SubjectsAccessed = System.Text.Json.JsonSerializer.Serialize(new List<int> { subjectId });
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Gets a comprehensive session summary for analytics and reporting.
    /// </summary>
    /// <returns>Session summary object.</returns>
    public object GetSessionSummary()
    {
        return new
        {
            SessionId = Id,
            ChildId,
            StartedAt,
            EndedAt,
            DurationMinutes = GetDurationMinutes(),
            SessionType,
            IsActive,
            ActivitiesCompleted,
            QuestionsAnswered,
            Accuracy = GetAccuracy(),
            StarsEarned,
            PointsScored,
            AchievementsEarned,
            HintsUsed,
            AudioPlaysCount,
            LanguageUsed,
            EndReason,
            ExceededTimeLimit,
            RemainingDailyMinutes = GetRemainingDailyMinutes(),
            SubjectsAccessedCount = GetSubjectsAccessedCount()
        };
    }

    /// <summary>
    /// Gets the number of unique subjects accessed during this session.
    /// </summary>
    /// <returns>Count of unique subjects.</returns>
    private int GetSubjectsAccessedCount()
    {
        if (string.IsNullOrEmpty(SubjectsAccessed)) return 0;

        try
        {
            var subjects = System.Text.Json.JsonSerializer.Deserialize<List<int>>(SubjectsAccessed);
            return subjects?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}