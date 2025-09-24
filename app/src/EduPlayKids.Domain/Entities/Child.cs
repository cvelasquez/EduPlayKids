using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents an individual child learner in the EduPlayKids application.
/// Tracks learning progress, preferences, and age-specific configurations.
/// Core entity for educational content delivery and progress monitoring.
/// </summary>
public class Child : AuditableEntity
{
    /// <summary>
    /// Gets or sets the child's first name.
    /// Used for personalization and progress reports.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the child's age in years.
    /// Determines curriculum level and content difficulty.
    /// Range: 3-8 years for target demographic.
    /// </summary>
    [Required]
    [Range(3, 8)]
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the child's date of birth.
    /// Used for precise age calculation and content adaptation.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the child's academic grade level.
    /// Values: PreK, Kindergarten, Grade1, Grade2
    /// </summary>
    [Required]
    [StringLength(20)]
    public string GradeLevel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the child's preferred language for learning.
    /// Inherits from parent but can be overridden per child.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "es";

    /// <summary>
    /// Gets or sets the child's language preference (alias for PreferredLanguage for test compatibility).
    /// </summary>
    public string LanguagePreference
    {
        get => PreferredLanguage;
        set => PreferredLanguage = value;
    }

    /// <summary>
    /// Gets or sets the child's learning profile.
    /// Tracks strengths, preferences, and adaptive learning settings.
    /// Stored as JSON for flexibility.
    /// </summary>
    public string? LearningProfile { get; set; }

    /// <summary>
    /// Gets or sets the child's current learning streak in days.
    /// Used for motivation and gamification features.
    /// </summary>
    public int LearningStreak { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of stars earned by the child.
    /// Aggregate score across all completed activities.
    /// </summary>
    public int TotalStarsEarned { get; set; } = 0;

    /// <summary>
    /// Gets or sets the child's favorite subject identifier.
    /// Based on usage patterns and performance analytics.
    /// </summary>
    public int? FavoriteSubjectId { get; set; }

    /// <summary>
    /// Gets or sets the date when the child last accessed the app.
    /// Used for engagement tracking and streak calculations.
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Gets or sets the total time spent learning in minutes.
    /// Accumulated across all sessions for progress reporting.
    /// </summary>
    public int TotalLearningTimeMinutes { get; set; } = 0;

    /// <summary>
    /// Gets or sets the child's current level in the learning progression.
    /// Determines content difficulty and available activities.
    /// </summary>
    public int CurrentLevel { get; set; } = 1;

    /// <summary>
    /// Gets or sets the child's avatar or character selection.
    /// Personalization feature for engagement.
    /// </summary>
    [StringLength(50)]
    public string? AvatarId { get; set; }

    /// <summary>
    /// Gets or sets the child's avatar URL (alias for AvatarId for test compatibility).
    /// </summary>
    public string? AvatarUrl
    {
        get => AvatarId;
        set => AvatarId = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether audio instructions are enabled.
    /// Critical for non-readers in the 3-5 age group.
    /// </summary>
    public bool AudioInstructionsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the child needs extra help.
    /// Triggers adaptive learning features and simplified content.
    /// </summary>
    public bool NeedsExtraHelp { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the child is advanced for their age.
    /// Enables crown challenges and advanced content.
    /// </summary>
    public bool IsAdvanced { get; set; } = false;

    /// <summary>
    /// Gets or sets the child's difficulty preference.
    /// Values: Easy, Medium, Hard, Adaptive
    /// </summary>
    [StringLength(20)]
    public string DifficultyPreference { get; set; } = "Adaptive";

    /// <summary>
    /// Gets or sets the foreign key to the parent user.
    /// Establishes the child-parent relationship.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property: The parent user who manages this child.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation property: Collection of progress records for this child.
    /// Tracks completion status and performance across all activities.
    /// </summary>
    public virtual ICollection<UserProgress> ProgressRecords { get; set; } = new List<UserProgress>();

    /// <summary>
    /// Navigation property: Collection of achievements earned by this child.
    /// Tracks gamification progress and motivational rewards.
    /// </summary>
    public virtual ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();

    /// <summary>
    /// Navigation property: Collection of learning sessions for this child.
    /// Tracks usage patterns and engagement metrics.
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Navigation property: Child's favorite subject reference.
    /// </summary>
    public virtual Subject? FavoriteSubject { get; set; }
    /// <summary>
    /// Gets or sets whether this child profile is active.
    /// Used for soft-delete functionality and profile management.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the progress collection (alias for ProgressRecords for test compatibility).
    /// </summary>
    public virtual ICollection<UserProgress> Progress => ProgressRecords;

    /// <summary>
    /// Gets the user achievements collection (alias for Achievements for test compatibility).
    /// </summary>
    public virtual ICollection<UserAchievement> UserAchievements => Achievements;

    /// <summary>
    /// Initializes a new instance of the Child class.
    /// Sets default values for new child profiles.
    /// </summary>
    public Child()
    {
        AudioInstructionsEnabled = true;
        DifficultyPreference = "Adaptive";
        CurrentLevel = 1;
        LearningStreak = 0;
        TotalStarsEarned = 0;
        PreferredLanguage = "es";
    }

    /// <summary>
    /// Gets the child's age group for content categorization.
    /// </summary>
    /// <returns>Age group: PreK, Kindergarten, or Primary</returns>
    public string GetAgeGroup()
    {
        return Age switch
        {
            3 or 4 => "PreK",
            5 => "Kindergarten",
            6 or 7 or 8 => "Primary",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Determines if the child should receive crown challenges.
    /// Based on performance metrics and advancement indicators.
    /// </summary>
    /// <returns>True if crown challenges should be offered.</returns>
    public bool ShouldReceiveCrownChallenges()
    {
        return IsAdvanced && TotalStarsEarned > 50 && CurrentLevel >= 3;
    }

    /// <summary>
    /// Calculates the child's overall progress percentage.
    /// Based on completed activities relative to their age group.
    /// </summary>
    /// <param name="totalActivitiesForAge">Total activities available for the child's age.</param>
    /// <returns>Progress percentage (0-100).</returns>
    public double CalculateOverallProgress(int totalActivitiesForAge)
    {
        if (totalActivitiesForAge == 0) return 0;
        var completedActivities = ProgressRecords.Count(p => p.IsCompleted);
        return Math.Round((double)completedActivities / totalActivitiesForAge * 100, 1);
    }

    /// <summary>
    /// Updates the learning streak based on last activity date.
    /// </summary>
    public void UpdateLearningStreak()
    {
        if (LastActivityAt.HasValue)
        {
            var daysSinceLastActivity = (DateTime.UtcNow.Date - LastActivityAt.Value.Date).Days;

            if (daysSinceLastActivity == 1)
            {
                // Consecutive day - increment streak
                LearningStreak++;
            }
            else if (daysSinceLastActivity > 1)
            {
                // Streak broken - reset to 1 for today's activity
                LearningStreak = 1;
            }
            // Same day - no change to streak
        }
        else
        {
            // First activity ever
            LearningStreak = 1;
        }

        LastActivityAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the avatar image path for ViewModel compatibility.
    /// </summary>
    public string AvatarImagePath => AvatarId ?? "default_child_avatar.png";
}