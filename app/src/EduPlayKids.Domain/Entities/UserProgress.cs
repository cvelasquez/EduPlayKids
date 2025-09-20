using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents a child's progress on a specific activity.
/// Tracks completion status, performance metrics, and learning analytics.
/// Core entity for monitoring educational progress and adaptive learning.
/// </summary>
public class UserProgress : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the child this progress belongs to.
    /// Links progress data to the specific learner.
    /// </summary>
    [Required]
    public int ChildId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the activity this progress tracks.
    /// Links progress data to the specific learning activity.
    /// </summary>
    [Required]
    public int ActivityId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the activity has been completed.
    /// Completion means the child has successfully finished all questions.
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of stars earned (1-3).
    /// Rating based on performance: 0 errors = 3⭐, 1-2 errors = 2⭐, 3+ errors = 1⭐
    /// </summary>
    [Range(0, 3)]
    public int StarsEarned { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total score achieved in this activity.
    /// Sum of points from correctly answered questions.
    /// </summary>
    public int TotalScore { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum possible score for this activity.
    /// Total points available from all questions.
    /// </summary>
    public int MaxPossibleScore { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of attempts made on this activity.
    /// Includes both partial and complete attempts.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of correct answers given.
    /// Used for calculating accuracy and performance metrics.
    /// </summary>
    public int CorrectAnswers { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of questions in the activity.
    /// Used for calculating completion percentage and accuracy.
    /// </summary>
    public int TotalQuestions { get; set; } = 0;

    /// <summary>
    /// Gets or sets the time spent on this activity in seconds.
    /// Tracks engagement and helps identify struggling areas.
    /// </summary>
    public int TimeSpentSeconds { get; set; } = 0;

    /// <summary>
    /// Gets or sets the date when the activity was first started.
    /// Tracks initial engagement with the content.
    /// </summary>
    public DateTime? FirstAttemptAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the activity was completed.
    /// Null if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the date of the most recent attempt.
    /// Tracks ongoing engagement and practice patterns.
    /// </summary>
    public DateTime? LastAttemptAt { get; set; }

    /// <summary>
    /// Gets or sets the number of hints used during the activity.
    /// Indicates level of support needed for completion.
    /// </summary>
    public int HintsUsed { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the child needed extra help.
    /// Triggers adaptive learning adjustments and support recommendations.
    /// </summary>
    public bool NeededExtraHelp { get; set; } = false;

    /// <summary>
    /// Gets or sets the difficulty level when this progress was recorded.
    /// Important for understanding performance in context.
    /// Values: Easy, Medium, Hard
    /// </summary>
    [StringLength(20)]
    public string DifficultyLevel { get; set; } = "Easy";

    /// <summary>
    /// Gets or sets detailed progress data as JSON.
    /// Flexible structure for storing:
    /// - Individual question responses
    /// - Time spent per question
    /// - Error patterns
    /// - Learning path taken
    /// </summary>
    public string? ProgressData { get; set; }

    /// <summary>
    /// Gets or sets performance analytics data as JSON.
    /// Stores computed metrics for adaptive learning:
    /// - Response time patterns
    /// - Error types and frequencies
    /// - Learning style indicators
    /// - Recommendation triggers
    /// </summary>
    public string? AnalyticsData { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this was a crown challenge attempt.
    /// Tracks performance on advanced content for high achievers.
    /// </summary>
    public bool IsCrownChallenge { get; set; } = false;

    /// <summary>
    /// Gets or sets the child's emotional state during the activity.
    /// Optional tracking for engagement and motivation analysis.
    /// Values: Happy, Frustrated, Excited, Confused, etc.
    /// </summary>
    [StringLength(20)]
    public string? EmotionalState { get; set; }

    /// <summary>
    /// Gets or sets notes about this progress session.
    /// For recording observations, issues, or special circumstances.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this progress should be synced.
    /// Used for future cloud synchronization features.
    /// </summary>
    public bool NeedsSync { get; set; } = false;

    /// <summary>
    /// Navigation property: The child this progress belongs to.
    /// </summary>
    public virtual Child Child { get; set; } = null!;

    /// <summary>
    /// Navigation property: The activity this progress tracks.
    /// </summary>
    public virtual Activity Activity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the UserProgress class.
    /// Sets default values for new progress tracking.
    /// </summary>
    public UserProgress()
    {
        IsCompleted = false;
        StarsEarned = 0;
        TotalScore = 0;
        AttemptCount = 0;
        CorrectAnswers = 0;
        TimeSpentSeconds = 0;
        HintsUsed = 0;
        NeededExtraHelp = false;
        DifficultyLevel = "Easy";
        IsCrownChallenge = false;
        NeedsSync = false;
    }

    /// <summary>
    /// Calculates the accuracy percentage for this activity.
    /// </summary>
    /// <returns>Accuracy as a percentage (0-100).</returns>
    public double GetAccuracy()
    {
        if (TotalQuestions == 0) return 0;
        return Math.Round((double)CorrectAnswers / TotalQuestions * 100, 1);
    }

    /// <summary>
    /// Calculates the completion percentage for this activity.
    /// </summary>
    /// <returns>Completion percentage (0-100).</returns>
    public double GetCompletionPercentage()
    {
        if (TotalQuestions == 0) return 0;
        var answeredQuestions = CorrectAnswers + (TotalQuestions - CorrectAnswers);
        return Math.Round((double)answeredQuestions / TotalQuestions * 100, 1);
    }

    /// <summary>
    /// Calculates the score percentage achieved.
    /// </summary>
    /// <returns>Score percentage (0-100).</returns>
    public double GetScorePercentage()
    {
        if (MaxPossibleScore == 0) return 0;
        return Math.Round((double)TotalScore / MaxPossibleScore * 100, 1);
    }

    /// <summary>
    /// Determines the star rating based on current performance.
    /// Uses the standard: 0 errors = 3⭐, 1-2 errors = 2⭐, 3+ errors = 1⭐
    /// </summary>
    /// <returns>Number of stars (0-3).</returns>
    public int CalculateStarRating()
    {
        if (!IsCompleted) return 0;

        var errors = TotalQuestions - CorrectAnswers;
        return errors switch
        {
            0 => 3,
            1 or 2 => 2,
            _ => 1
        };
    }

    /// <summary>
    /// Updates the progress with a new attempt.
    /// </summary>
    /// <param name="questionsAnswered">Number of questions answered in this attempt.</param>
    /// <param name="correctInThisAttempt">Number of correct answers in this attempt.</param>
    /// <param name="timeSpent">Time spent in this attempt (seconds).</param>
    /// <param name="hintsUsedInAttempt">Number of hints used in this attempt.</param>
    public void UpdateProgress(int questionsAnswered, int correctInThisAttempt, int timeSpent, int hintsUsedInAttempt = 0)
    {
        AttemptCount++;
        LastAttemptAt = DateTime.UtcNow;

        if (FirstAttemptAt == null)
        {
            FirstAttemptAt = DateTime.UtcNow;
        }

        // Update cumulative stats
        TimeSpentSeconds += timeSpent;
        HintsUsed += hintsUsedInAttempt;

        // Update answers (this could be refined based on whether it's a retry)
        CorrectAnswers = Math.Max(CorrectAnswers, correctInThisAttempt);

        // Check if completed
        if (questionsAnswered >= TotalQuestions && !IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            StarsEarned = CalculateStarRating();
        }

        // Determine if extra help was needed
        if (HintsUsed > TotalQuestions / 2 || AttemptCount > 2)
        {
            NeededExtraHelp = true;
        }

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Marks the activity as completed with final statistics.
    /// </summary>
    /// <param name="finalScore">Final score achieved.</param>
    /// <param name="maxScore">Maximum possible score.</param>
    public void CompleteActivity(int finalScore, int maxScore)
    {
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        TotalScore = finalScore;
        MaxPossibleScore = maxScore;
        StarsEarned = CalculateStarRating();
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Determines if the child performed exceptionally well and should receive crown challenges.
    /// </summary>
    /// <returns>True if crown challenges should be offered.</returns>
    public bool ShouldOfferCrownChallenge()
    {
        return IsCompleted &&
               StarsEarned >= 3 &&
               GetAccuracy() >= 90 &&
               !NeededExtraHelp &&
               HintsUsed == 0;
    }

    /// <summary>
    /// Determines if the child struggled and needs additional support.
    /// </summary>
    /// <returns>True if additional support is recommended.</returns>
    public bool NeedsAdditionalSupport()
    {
        return AttemptCount > 3 ||
               HintsUsed > TotalQuestions ||
               (IsCompleted && GetAccuracy() < 60) ||
               NeededExtraHelp;
    }

    /// <summary>
    /// Gets a performance summary for reporting.
    /// </summary>
    /// <returns>Performance summary object.</returns>
    public object GetPerformanceSummary()
    {
        return new
        {
            IsCompleted,
            StarsEarned,
            Accuracy = GetAccuracy(),
            ScorePercentage = GetScorePercentage(),
            TimeSpent = TimeSpentSeconds,
            AttemptsNeeded = AttemptCount,
            HintsUsed,
            NeededExtraHelp,
            CompletedAt,
            ShouldOfferCrownChallenge = ShouldOfferCrownChallenge(),
            NeedsAdditionalSupport = NeedsAdditionalSupport()
        };
    }
}