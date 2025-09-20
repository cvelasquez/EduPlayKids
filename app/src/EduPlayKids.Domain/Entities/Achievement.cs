using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents an achievement or milestone in the EduPlayKids gamification system.
/// Defines educational accomplishments, crown challenges, and motivational rewards.
/// Core component for maintaining engagement and celebrating learning progress.
/// </summary>
public class Achievement : BaseEntity
{
    /// <summary>
    /// Gets or sets the achievement name in English.
    /// Descriptive title for the accomplishment.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achievement name in Spanish.
    /// Bilingual support for Spanish-speaking families.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NameEs { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achievement description in English.
    /// Explains what the child accomplished and why it's significant.
    /// </summary>
    [StringLength(500)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// Gets or sets the achievement description in Spanish.
    /// Bilingual description for parent understanding.
    /// </summary>
    [StringLength(500)]
    public string? DescriptionEs { get; set; }

    /// <summary>
    /// Gets or sets the achievement category.
    /// Categories: Subject, Progress, Streak, Crown, Special, Milestone
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achievement type identifier.
    /// Specific achievement types within categories:
    /// - SubjectMaster: Completed all activities in a subject
    /// - StarCollector: Earned specific number of stars
    /// - StreakKeeper: Maintained learning streak
    /// - CrownChampion: Completed crown challenges
    /// - FirstStep: First activity completed
    /// - SpeedLearner: Completed activities quickly
    /// </summary>
    [Required]
    [StringLength(50)]
    public string AchievementType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the badge icon identifier.
    /// References the visual badge asset for display.
    /// </summary>
    [StringLength(50)]
    public string? BadgeIcon { get; set; }

    /// <summary>
    /// Gets or sets the badge color in hex format.
    /// Visual theming for the achievement badge.
    /// </summary>
    [StringLength(7)]
    public string? BadgeColor { get; set; }

    /// <summary>
    /// Gets or sets the points awarded for earning this achievement.
    /// Contributes to overall progress and leaderboards.
    /// </summary>
    [Range(0, 1000)]
    public int Points { get; set; } = 0;

    /// <summary>
    /// Gets or sets the achievement criteria as JSON.
    /// Flexible structure defining requirements:
    /// - Required activities or subjects
    /// - Performance thresholds
    /// - Time-based criteria
    /// - Streak requirements
    /// </summary>
    public string? Criteria { get; set; }

    /// <summary>
    /// Gets or sets the minimum age for this achievement.
    /// Age-appropriate recognition and challenges.
    /// </summary>
    [Range(3, 8)]
    public int MinAge { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum age for this achievement.
    /// Upper bound for age-appropriate content.
    /// </summary>
    [Range(3, 8)]
    public int MaxAge { get; set; } = 8;

    /// <summary>
    /// Gets or sets the rarity level of this achievement.
    /// Values: Common, Rare, Epic, Legendary
    /// Affects how special the accomplishment feels.
    /// </summary>
    [StringLength(20)]
    public string Rarity { get; set; } = "Common";

    /// <summary>
    /// Gets or sets a value indicating whether this achievement is active.
    /// Controls availability in the gamification system.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this achievement is hidden until earned.
    /// Surprise achievements for extra motivation.
    /// </summary>
    public bool IsHidden { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this is a crown challenge achievement.
    /// Special recognition for advanced learners.
    /// </summary>
    public bool IsCrownChallenge { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this achievement can be earned multiple times.
    /// For ongoing accomplishments like streaks or monthly goals.
    /// </summary>
    public bool IsRepeatable { get; set; } = false;

    /// <summary>
    /// Gets or sets the display order for achievement listing.
    /// Determines sequence in achievement galleries.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the subject this achievement is related to.
    /// Null for cross-subject or general achievements.
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the celebration message in English.
    /// Shown when the achievement is earned.
    /// </summary>
    [StringLength(200)]
    public string? CelebrationMessageEn { get; set; }

    /// <summary>
    /// Gets or sets the celebration message in Spanish.
    /// Bilingual celebration for earned achievements.
    /// </summary>
    [StringLength(200)]
    public string? CelebrationMessageEs { get; set; }

    /// <summary>
    /// Gets or sets the number of times this achievement has been earned.
    /// Analytics data for achievement effectiveness.
    /// </summary>
    public int EarnedCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets additional metadata for the achievement.
    /// JSON structure for extension data and custom properties.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Navigation property: The subject this achievement relates to (if any).
    /// </summary>
    public virtual Subject? Subject { get; set; }

    /// <summary>
    /// Navigation property: Collection of users who have earned this achievement.
    /// Tracks which children have accomplished this milestone.
    /// </summary>
    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    /// <summary>
    /// Initializes a new instance of the Achievement class.
    /// Sets default values for new achievement creation.
    /// </summary>
    public Achievement()
    {
        Points = 0;
        MinAge = 3;
        MaxAge = 8;
        Rarity = "Common";
        IsActive = true;
        IsHidden = false;
        IsCrownChallenge = false;
        IsRepeatable = false;
        EarnedCount = 0;
    }

    /// <summary>
    /// Gets the localized name based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The achievement name in the requested language.</returns>
    public string GetLocalizedName(string language)
    {
        return language.ToLower() switch
        {
            "en" => NameEn,
            "es" => NameEs,
            _ => NameEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized description based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The achievement description in the requested language.</returns>
    public string? GetLocalizedDescription(string language)
    {
        return language.ToLower() switch
        {
            "en" => DescriptionEn,
            "es" => DescriptionEs,
            _ => DescriptionEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized celebration message based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The celebration message in the requested language.</returns>
    public string? GetLocalizedCelebrationMessage(string language)
    {
        return language.ToLower() switch
        {
            "en" => CelebrationMessageEn,
            "es" => CelebrationMessageEs,
            _ => CelebrationMessageEn // Default to English
        };
    }

    /// <summary>
    /// Determines if this achievement is appropriate for the given child's age.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <returns>True if the achievement is age-appropriate, false otherwise.</returns>
    public bool IsAgeAppropriate(int childAge)
    {
        return childAge >= MinAge && childAge <= MaxAge;
    }

    /// <summary>
    /// Determines if this achievement is available for the given child.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <param name="isAdvanced">Whether the child is marked as advanced.</param>
    /// <returns>True if the achievement is available, false otherwise.</returns>
    public bool IsAvailableForChild(int childAge, bool isAdvanced)
    {
        if (!IsActive || !IsAgeAppropriate(childAge))
            return false;

        // Crown challenges only for advanced children
        if (IsCrownChallenge && !isAdvanced)
            return false;

        return true;
    }

    /// <summary>
    /// Evaluates if the achievement criteria are met based on child progress data.
    /// </summary>
    /// <param name="progressData">Child's progress and performance data.</param>
    /// <returns>True if criteria are met, false otherwise.</returns>
    public bool EvaluateCriteria(Dictionary<string, object> progressData)
    {
        if (string.IsNullOrEmpty(Criteria))
            return false;

        try
        {
            var criteriaObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(Criteria);
            if (criteriaObj == null) return false;

            return AchievementType switch
            {
                "SubjectMaster" => EvaluateSubjectMasterCriteria(criteriaObj, progressData),
                "StarCollector" => EvaluateStarCollectorCriteria(criteriaObj, progressData),
                "StreakKeeper" => EvaluateStreakKeeperCriteria(criteriaObj, progressData),
                "CrownChampion" => EvaluateCrownChampionCriteria(criteriaObj, progressData),
                "FirstStep" => EvaluateFirstStepCriteria(criteriaObj, progressData),
                "SpeedLearner" => EvaluateSpeedLearnerCriteria(criteriaObj, progressData),
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Records that this achievement has been earned.
    /// </summary>
    public void RecordEarned()
    {
        EarnedCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the rarity multiplier for point calculations.
    /// </summary>
    /// <returns>Multiplier based on rarity level.</returns>
    public decimal GetRarityMultiplier()
    {
        return Rarity switch
        {
            "Common" => 1.0m,
            "Rare" => 1.5m,
            "Epic" => 2.0m,
            "Legendary" => 3.0m,
            _ => 1.0m
        };
    }

    #region Private Criteria Evaluation Methods

    private bool EvaluateSubjectMasterCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if all activities in a subject are completed with minimum stars
        if (!criteria.ContainsKey("subjectId") || !criteria.ContainsKey("minStars"))
            return false;

        var requiredSubjectId = Convert.ToInt32(criteria["subjectId"]);
        var minStars = Convert.ToInt32(criteria["minStars"]);

        if (progressData.ContainsKey("subjectProgress"))
        {
            var subjectProgress = progressData["subjectProgress"] as Dictionary<string, object>;
            if (subjectProgress?.ContainsKey(requiredSubjectId.ToString()) == true)
            {
                var progress = subjectProgress[requiredSubjectId.ToString()] as Dictionary<string, object>;
                if (progress?.ContainsKey("completedWithMinStars") == true)
                {
                    return Convert.ToBoolean(progress["completedWithMinStars"]);
                }
            }
        }

        return false;
    }

    private bool EvaluateStarCollectorCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if child has earned minimum number of stars
        if (!criteria.ContainsKey("minStars"))
            return false;

        var requiredStars = Convert.ToInt32(criteria["minStars"]);

        if (progressData.ContainsKey("totalStars"))
        {
            var totalStars = Convert.ToInt32(progressData["totalStars"]);
            return totalStars >= requiredStars;
        }

        return false;
    }

    private bool EvaluateStreakKeeperCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if child has maintained learning streak
        if (!criteria.ContainsKey("minDays"))
            return false;

        var requiredDays = Convert.ToInt32(criteria["minDays"]);

        if (progressData.ContainsKey("learningStreak"))
        {
            var currentStreak = Convert.ToInt32(progressData["learningStreak"]);
            return currentStreak >= requiredDays;
        }

        return false;
    }

    private bool EvaluateCrownChampionCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if child has completed crown challenges
        if (!criteria.ContainsKey("minCrownChallenges"))
            return false;

        var requiredChallenges = Convert.ToInt32(criteria["minCrownChallenges"]);

        if (progressData.ContainsKey("crownChallengesCompleted"))
        {
            var completed = Convert.ToInt32(progressData["crownChallengesCompleted"]);
            return completed >= requiredChallenges;
        }

        return false;
    }

    private bool EvaluateFirstStepCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if child has completed their first activity
        if (progressData.ContainsKey("activitiesCompleted"))
        {
            var completedCount = Convert.ToInt32(progressData["activitiesCompleted"]);
            return completedCount >= 1;
        }

        return false;
    }

    private bool EvaluateSpeedLearnerCriteria(Dictionary<string, object> criteria, Dictionary<string, object> progressData)
    {
        // Check if child completes activities faster than average
        if (!criteria.ContainsKey("maxAverageTime"))
            return false;

        var maxTime = Convert.ToInt32(criteria["maxAverageTime"]);

        if (progressData.ContainsKey("averageCompletionTime"))
        {
            var avgTime = Convert.ToInt32(progressData["averageCompletionTime"]);
            return avgTime <= maxTime;
        }

        return false;
    }

    #endregion
}