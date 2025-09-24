using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents the relationship between a child and an earned achievement.
/// Tracks when achievements were earned, progress towards goals, and celebration status.
/// Core entity for gamification and motivational features in the learning experience.
/// </summary>
public class UserAchievement : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the child who earned this achievement.
    /// Links the achievement to the specific learner.
    /// </summary>
    [Required]
    public int ChildId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the achievement that was earned.
    /// Links to the specific accomplishment or milestone.
    /// </summary>
    [Required]
    public int AchievementId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the achievement was earned.
    /// Records the exact moment of accomplishment for historical tracking.
    /// </summary>
    [Required]
    public DateTime EarnedAt { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage towards earning this achievement.
    /// For achievements with measurable criteria (e.g., star collection, streaks).
    /// Range: 0-100, where 100 means earned.
    /// </summary>
    [Range(0, 100)]
    public int ProgressPercentage { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current progress value towards the achievement goal.
    /// Raw progress number (e.g., current stars, current streak days).
    /// </summary>
    public int CurrentProgress { get; set; } = 0;

    /// <summary>
    /// Gets or sets the target value required to earn the achievement.
    /// The goal that needs to be reached (e.g., total stars needed, streak days required).
    /// </summary>
    public int TargetProgress { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the achievement has been earned.
    /// True when the child has successfully completed all requirements.
    /// </summary>
    public bool IsEarned { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the achievement celebration has been shown.
    /// Ensures children see their accomplishments and feel recognized.
    /// </summary>
    public bool CelebrationShown { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this achievement is currently in progress.
    /// Helps track active goals the child is working towards.
    /// </summary>
    public bool IsInProgress { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this achievement is visible to the child.
    /// Hidden achievements (surprises) are not visible until earned or close to completion.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of times this achievement has been earned (for repeatable achievements).
    /// Tracks multiple instances of accomplishments like weekly streaks.
    /// </summary>
    public int EarnedCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the date when progress tracking started for this achievement.
    /// Important for time-based achievements and progress analytics.
    /// </summary>
    public DateTime? ProgressStartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the achievement will expire (for time-sensitive achievements).
    /// Null for permanent achievements.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets contextual data about how the achievement was earned.
    /// JSON structure containing:
    /// - Specific activities completed
    /// - Performance metrics at time of earning
    /// - Special circumstances or conditions
    /// </summary>
    public string? EarnedContext { get; set; }

    /// <summary>
    /// Gets or sets the child's emotional reaction to earning the achievement.
    /// Optional tracking for engagement and motivation analysis.
    /// Values: Excited, Happy, Proud, Surprised, etc.
    /// </summary>
    [StringLength(20)]
    public string? EmotionalReaction { get; set; }

    /// <summary>
    /// Gets or sets notes about this achievement instance.
    /// For recording special circumstances, observations, or memorable moments.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this achievement should be synced.
    /// Used for future cloud synchronization features.
    /// </summary>
    public bool NeedsSync { get; set; } = false;

    /// <summary>
    /// Gets or sets the points earned from this achievement.
    /// May differ from base achievement points due to bonuses or multipliers.
    /// </summary>
    public int PointsEarned { get; set; } = 0;

    /// <summary>
    /// Gets or sets the bonus multiplier applied when this achievement was earned.
    /// Used for special events, streaks, or exceptional performance.
    /// </summary>
    public decimal BonusMultiplier { get; set; } = 1.0m;

    /// <summary>
    /// Navigation property: The child who earned this achievement.
    /// </summary>
    public virtual Child Child { get; set; } = null!;

    /// <summary>
    /// Navigation property: The achievement that was earned.
    /// </summary>
    public virtual Achievement Achievement { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the UserAchievement class.
    /// Sets default values for new achievement tracking.
    /// </summary>
    public UserAchievement()
    {
        ProgressPercentage = 0;
        CurrentProgress = 0;
        TargetProgress = 0;
        IsEarned = false;
        CelebrationShown = false;
        IsInProgress = false;
        IsVisible = true;
        EarnedCount = 0;
        NeedsSync = false;
        PointsEarned = 0;
        BonusMultiplier = 1.0m;
        EarnedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the user ID (alias for ChildId for ViewModel compatibility).
    /// </summary>
    public int UserId => ChildId;

    /// <summary>
    /// Updates the progress towards earning this achievement.
    /// </summary>
    /// <param name="newProgress">The new progress value.</param>
    /// <param name="target">The target value needed to earn the achievement.</param>
    public void UpdateProgress(int newProgress, int target)
    {
        CurrentProgress = newProgress;
        TargetProgress = target;

        if (target > 0)
        {
            ProgressPercentage = Math.Min(100, (int)Math.Round((double)newProgress / target * 100));
        }

        // Mark as in progress if we've made some progress but haven't earned it yet
        IsInProgress = ProgressPercentage > 0 && !IsEarned;

        // Start tracking progress if this is the first progress
        if (ProgressStartedAt == null && newProgress > 0)
        {
            ProgressStartedAt = DateTime.UtcNow;
        }

        // Check if achievement should be earned
        if (ProgressPercentage >= 100 && !IsEarned)
        {
            EarnAchievement();
        }

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Marks the achievement as earned and sets up celebration.
    /// </summary>
    /// <param name="bonusMultiplier">Optional bonus multiplier for points.</param>
    /// <param name="context">Optional context about how it was earned.</param>
    public void EarnAchievement(decimal bonusMultiplier = 1.0m, string? context = null)
    {
        if (IsEarned && !Achievement?.IsRepeatable == true)
            return; // Already earned and not repeatable

        IsEarned = true;
        EarnedAt = DateTime.UtcNow;
        EarnedCount++;
        BonusMultiplier = bonusMultiplier;
        EarnedContext = context;
        CelebrationShown = false; // Reset to show celebration
        IsInProgress = false;
        ProgressPercentage = 100;

        // Calculate points earned
        if (Achievement != null)
        {
            PointsEarned = (int)(Achievement.Points * bonusMultiplier * Achievement.GetRarityMultiplier());
        }

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Marks the celebration as shown to the child.
    /// </summary>
    /// <param name="emotionalReaction">The child's reaction to the achievement.</param>
    public void MarkCelebrationShown(string? emotionalReaction = null)
    {
        CelebrationShown = true;
        EmotionalReaction = emotionalReaction;
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Determines if this achievement should be visible to the child.
    /// Considers hiding rules and progress thresholds.
    /// </summary>
    /// <returns>True if the achievement should be visible, false otherwise.</returns>
    public bool ShouldBeVisible()
    {
        if (Achievement == null) return false;

        // Always show earned achievements
        if (IsEarned) return true;

        // Show hidden achievements only when close to completion (80%+) or earned
        if (Achievement.IsHidden && ProgressPercentage < 80)
            return false;

        // Show if explicitly set as visible or has progress
        return IsVisible || IsInProgress;
    }

    /// <summary>
    /// Determines if the achievement celebration should be shown.
    /// </summary>
    /// <returns>True if celebration should be displayed, false otherwise.</returns>
    public bool ShouldShowCelebration()
    {
        return IsEarned && !CelebrationShown;
    }

    /// <summary>
    /// Gets the estimated time to earn this achievement based on current progress.
    /// </summary>
    /// <param name="averageProgressPerDay">Average progress made per day.</param>
    /// <returns>Estimated days to completion, or null if can't be calculated.</returns>
    public int? GetEstimatedDaysToCompletion(double averageProgressPerDay)
    {
        if (IsEarned || averageProgressPerDay <= 0 || TargetProgress <= 0)
            return null;

        var remainingProgress = TargetProgress - CurrentProgress;
        if (remainingProgress <= 0) return 0;

        return (int)Math.Ceiling(remainingProgress / averageProgressPerDay);
    }

    /// <summary>
    /// Determines if this achievement is time-sensitive and at risk of expiring.
    /// </summary>
    /// <param name="warningDays">Number of days before expiry to show warning.</param>
    /// <returns>True if achievement expires soon, false otherwise.</returns>
    public bool IsExpiringSoon(int warningDays = 3)
    {
        if (ExpiresAt == null || IsEarned)
            return false;

        return DateTime.UtcNow.AddDays(warningDays) >= ExpiresAt;
    }

    /// <summary>
    /// Gets a progress summary for display in achievement UI.
    /// </summary>
    /// <returns>Progress summary object.</returns>
    public object GetProgressSummary()
    {
        return new
        {
            IsEarned,
            ProgressPercentage,
            CurrentProgress,
            TargetProgress,
            IsInProgress,
            ShouldBeVisible = ShouldBeVisible(),
            ShouldShowCelebration = ShouldShowCelebration(),
            EarnedAt = IsEarned ? (DateTime?)EarnedAt : null,
            PointsEarned,
            EarnedCount,
            IsExpiringSoon = IsExpiringSoon(),
            ExpiresAt,
            EmotionalReaction
        };
    }

    /// <summary>
    /// Resets progress for repeatable achievements.
    /// Used when starting a new cycle (e.g., monthly challenges).
    /// </summary>
    public void ResetProgress()
    {
        if (Achievement?.IsRepeatable != true)
            return;

        CurrentProgress = 0;
        ProgressPercentage = 0;
        IsInProgress = false;
        ProgressStartedAt = null;
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Calculates bonus points for exceptional achievement earning.
    /// </summary>
    /// <param name="speedBonus">Bonus for earning quickly.</param>
    /// <param name="streakBonus">Bonus for achievement streaks.</param>
    /// <returns>Total bonus points.</returns>
    public int CalculateBonusPoints(int speedBonus = 0, int streakBonus = 0)
    {
        var basePoints = PointsEarned;
        var totalBonus = speedBonus + streakBonus;

        return (int)(basePoints * (1 + totalBonus / 100.0));
    }
}