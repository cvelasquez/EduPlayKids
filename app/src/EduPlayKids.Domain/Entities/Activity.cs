using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;
using EduPlayKids.Domain.Enums;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents a learning activity within a subject in the EduPlayKids curriculum.
/// Contains educational content with difficulty levels, curriculum alignment, and sequential unlocking.
/// Core entity for delivering interactive learning experiences to children.
/// </summary>
public class Activity : BaseEntity
{
    /// <summary>
    /// Gets or sets the activity title in English.
    /// Descriptive name for the learning activity.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string TitleEn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the activity title in Spanish.
    /// Bilingual support for Spanish-speaking families.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string TitleEs { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the activity description in English.
    /// Explains the learning objective and activity mechanics.
    /// </summary>
    [StringLength(1000)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// Gets or sets the activity description in Spanish.
    /// Bilingual description for parent understanding.
    /// </summary>
    [StringLength(1000)]
    public string? DescriptionEs { get; set; }

    /// <summary>
    /// Gets or sets the activity type identifier.
    /// Categories: MultipleChoice, DragDrop, Matching, Tracing, etc.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the difficulty level of the activity.
    /// Values: Easy, Medium, Hard
    /// </summary>
    [Required]
    [StringLength(20)]
    public string DifficultyLevel { get; set; } = "Easy";

    /// <summary>
    /// Gets or sets the difficulty level as enum (for test compatibility).
    /// </summary>
    public Enums.DifficultyLevel DifficultyLevelEnum
    {
        get => Enum.TryParse<Enums.DifficultyLevel>(DifficultyLevel, true, out var result) ? result : Enums.DifficultyLevel.Easy;
        set => DifficultyLevel = value.ToString();
    }

    /// <summary>
    /// Gets or sets the activity type as enum (for test compatibility).
    /// </summary>
    public Enums.ActivityType ActivityTypeEnum
    {
        get => Enum.TryParse<Enums.ActivityType>(ActivityType, true, out var result) ? result : Enums.ActivityType.MultipleChoice;
        set => ActivityType = value.ToString();
    }

    /// <summary>
    /// Gets or sets the minimum age for this activity.
    /// Determines age-appropriate content delivery.
    /// </summary>
    [Range(3, 8)]
    public int MinAge { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum age for this activity.
    /// Upper bound for age-appropriate content.
    /// </summary>
    [Range(3, 8)]
    public int MaxAge { get; set; } = 8;
    /// <summary>
    /// Gets the target age group for this activity (calculated property).
    /// Used for activity categorization and test compatibility.
    /// </summary>
    public string TargetAgeGroup => $"{MinAge}-{MaxAge}";

    /// <summary>
    /// Gets or sets the target age group as enum (for test compatibility).
    /// </summary>
    public Enums.AgeGroup TargetAgeGroupEnum
    {
        get
        {
            // Calculate age group based on age range
            if (MinAge <= 4 && MaxAge <= 4) return Enums.AgeGroup.PreK;
            if (MinAge <= 5 && MaxAge <= 5) return Enums.AgeGroup.Kindergarten;
            if (MinAge <= 6 && MaxAge <= 6) return Enums.AgeGroup.Grade1;
            if (MinAge >= 7) return Enums.AgeGroup.Grade2;
            if (MinAge <= 6 && MaxAge >= 7) return Enums.AgeGroup.Primary;
            return Enums.AgeGroup.All;
        }
        set
        {
            // Update age range based on enum
            switch (value)
            {
                case Enums.AgeGroup.PreK:
                    MinAge = 3; MaxAge = 4; break;
                case Enums.AgeGroup.Kindergarten:
                    MinAge = 5; MaxAge = 5; break;
                case Enums.AgeGroup.Grade1:
                    MinAge = 6; MaxAge = 6; break;
                case Enums.AgeGroup.Grade2:
                    MinAge = 7; MaxAge = 8; break;
                case Enums.AgeGroup.Primary:
                    MinAge = 6; MaxAge = 8; break;
                default:
                    MinAge = 3; MaxAge = 8; break;
            }
        }
    }

    /// <summary>
    /// Gets or sets the display order within the subject.
    /// Determines the sequence of activities in the curriculum.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the order in sequence (alias for DisplayOrder for test compatibility).
    /// </summary>
    public int OrderInSequence
    {
        get => DisplayOrder;
        set => DisplayOrder = value;
    }

    /// <summary>
    /// Gets or sets the estimated completion time in minutes.
    /// Helps with session planning and progress tracking.
    /// </summary>
    [Range(1, 30)]
    public int EstimatedMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the estimated completion time in minutes (alias for test compatibility).
    /// </summary>
    public int EstimatedCompletionTimeMinutes
    {
        get => EstimatedMinutes;
        set => EstimatedMinutes = value;
    }

    /// <summary>
    /// Gets or sets the learning objectives for this activity.
    /// What the child should learn or practice.
    /// Stored as JSON array for multiple objectives.
    /// </summary>
    public string? LearningObjectives { get; set; }

    /// <summary>
    /// Gets or sets the prerequisite activities for unlocking this activity.
    /// Implements sequential learning progression.
    /// Stored as JSON array of activity IDs.
    /// </summary>
    public string? Prerequisites { get; set; }

    /// <summary>
    /// Gets or sets the instruction text in English.
    /// What the child should do to complete the activity.
    /// </summary>
    [StringLength(500)]
    public string? InstructionEn { get; set; }

    /// <summary>
    /// Gets or sets the instruction text in Spanish.
    /// Bilingual instructions for Spanish-speaking children.
    /// </summary>
    [StringLength(500)]
    public string? InstructionEs { get; set; }

    /// <summary>
    /// Gets or sets the audio instruction file path in English.
    /// Critical for non-readers in the 3-5 age group.
    /// </summary>
    [StringLength(255)]
    public string? AudioInstructionEnPath { get; set; }

    /// <summary>
    /// Gets or sets the audio instruction file path in Spanish.
    /// Bilingual audio support for Spanish-speaking children.
    /// </summary>
    [StringLength(255)]
    public string? AudioInstructionEsPath { get; set; }

    /// <summary>
    /// Gets or sets the audio instruction URL (alias for AudioInstructionEnPath for test compatibility).
    /// </summary>
    public string? AudioInstructionUrl
    {
        get => AudioInstructionEnPath;
        set => AudioInstructionEnPath = value;
    }

    /// <summary>
    /// Gets or sets the thumbnail image path for the activity.
    /// Visual representation in activity lists and menus.
    /// </summary>
    [StringLength(255)]
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image URL (alias for ThumbnailPath for test compatibility).
    /// </summary>
    public string? ThumbnailImageUrl
    {
        get => ThumbnailPath;
        set => ThumbnailPath = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this activity is active.
    /// Controls content availability and visibility.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this activity requires premium access.
    /// Implements freemium model restrictions.
    /// </summary>
    public bool RequiresPremium { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this is a crown challenge activity.
    /// Advanced content for high-performing children.
    /// </summary>
    public bool IsCrownChallenge { get; set; } = false;

    /// <summary>
    /// Gets or sets the curriculum standards this activity aligns with.
    /// References US educational standards for validation.
    /// Stored as JSON array of standard codes.
    /// </summary>
    public string? CurriculumStandards { get; set; }

    /// <summary>
    /// Gets or sets the activity configuration data.
    /// Flexible JSON structure for activity-specific settings.
    /// Includes scoring rules, interaction parameters, etc.
    /// </summary>
    public string? ConfigurationData { get; set; }

    /// <summary>
    /// Gets or sets the success criteria for 3-star rating.
    /// Defines what constitutes perfect completion.
    /// Stored as JSON for complex criteria.
    /// </summary>
    public string? SuccessCriteria { get; set; }

    /// <summary>
    /// Gets or sets the number of times this activity has been played.
    /// Used for popularity analytics and content optimization.
    /// </summary>
    public int PlayCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the average completion time in seconds.
    /// Analytics data for performance optimization.
    /// </summary>
    public decimal AverageCompletionTime { get; set; } = 0;

    /// <summary>
    /// Gets or sets the average star rating received.
    /// Quality metric for content effectiveness.
    /// </summary>
    public decimal AverageStarRating { get; set; } = 0;

    /// <summary>
    /// Gets or sets the foreign key to the subject this activity belongs to.
    /// Establishes the curriculum hierarchy relationship.
    /// </summary>
    [Required]
    public int SubjectId { get; set; }

    /// <summary>
    /// Navigation property: The subject this activity belongs to.
    /// </summary>
    public virtual Subject Subject { get; set; } = null!;

    /// <summary>
    /// Navigation property: Collection of questions within this activity.
    /// Contains the interactive content and assessment items.
    /// </summary>
    public virtual ICollection<ActivityQuestion> Questions { get; set; } = new List<ActivityQuestion>();

    /// <summary>
    /// Navigation property: Collection of progress records for this activity.
    /// Tracks completion status and performance across all children.
    /// </summary>
    public virtual ICollection<UserProgress> ProgressRecords { get; set; } = new List<UserProgress>();

    /// <summary>
    /// Gets or sets the progress collection (alias for ProgressRecords for test compatibility).
    /// </summary>
    public virtual ICollection<UserProgress> Progress { get; set; } = new List<UserProgress>();

    /// <summary>
    /// Gets the activity title in the current locale (defaults to English).
    /// For ViewModel compatibility and simple access scenarios.
    /// </summary>
    public string Title => TitleEn;

    /// <summary>
    /// Initializes a new instance of the Activity class.
    /// Sets default values for new activity creation.
    /// </summary>
    public Activity()
    {
        IsActive = true;
        RequiresPremium = false;
        IsCrownChallenge = false;
        DifficultyLevel = "Easy";
        EstimatedMinutes = 5;
        MinAge = 3;
        MaxAge = 8;
        PlayCount = 0;
        AverageCompletionTime = 0;
        AverageStarRating = 0;
        Questions = new List<ActivityQuestion>();
        ProgressRecords = new List<UserProgress>();
        Progress = new List<UserProgress>();
    }

    /// <summary>
    /// Gets the localized title based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The activity title in the requested language.</returns>
    public string GetLocalizedTitle(string language)
    {
        return language.ToLower() switch
        {
            "en" => TitleEn,
            "es" => TitleEs,
            _ => TitleEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized description based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The activity description in the requested language.</returns>
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
    /// Gets the localized instruction based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The activity instruction in the requested language.</returns>
    public string? GetLocalizedInstruction(string language)
    {
        return language.ToLower() switch
        {
            "en" => InstructionEn,
            "es" => InstructionEs,
            _ => InstructionEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized audio instruction path based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The audio instruction path in the requested language.</returns>
    public string? GetLocalizedAudioPath(string language)
    {
        return language.ToLower() switch
        {
            "en" => AudioInstructionEnPath,
            "es" => AudioInstructionEsPath,
            _ => AudioInstructionEnPath // Default to English
        };
    }

    /// <summary>
    /// Determines if this activity is appropriate for the given child's age.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <returns>True if the activity is age-appropriate, false otherwise.</returns>
    public bool IsAgeAppropriate(int childAge)
    {
        return childAge >= MinAge && childAge <= MaxAge;
    }

    /// <summary>
    /// Determines if this activity is available for the given child considering all restrictions.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <param name="hasPremiumAccess">Whether the user has premium access.</param>
    /// <param name="completedActivityIds">List of completed activity IDs for prerequisite checking.</param>
    /// <returns>True if the activity is available, false otherwise.</returns>
    public bool IsAvailableForChild(int childAge, bool hasPremiumAccess, IEnumerable<int> completedActivityIds)
    {
        // Check basic availability criteria
        if (!IsActive || !IsAgeAppropriate(childAge))
            return false;

        // Check premium requirements
        if (RequiresPremium && !hasPremiumAccess)
            return false;

        // Check prerequisites
        return ArePrerequisitesMet(completedActivityIds);
    }

    /// <summary>
    /// Checks if all prerequisite activities have been completed.
    /// </summary>
    /// <param name="completedActivityIds">List of completed activity IDs.</param>
    /// <returns>True if all prerequisites are met, false otherwise.</returns>
    public bool ArePrerequisitesMet(IEnumerable<int> completedActivityIds)
    {
        if (string.IsNullOrEmpty(Prerequisites))
            return true;

        try
        {
            var requiredIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(Prerequisites);
            return requiredIds?.All(id => completedActivityIds.Contains(id)) ?? true;
        }
        catch
        {
            // If prerequisites can't be parsed, assume no prerequisites
            return true;
        }
    }

    /// <summary>
    /// Updates the activity statistics based on a completed session.
    /// </summary>
    /// <param name="completionTimeSeconds">Time taken to complete the activity.</param>
    /// <param name="starsEarned">Number of stars earned (1-3).</param>
    public void UpdateStatistics(int completionTimeSeconds, int starsEarned)
    {
        PlayCount++;

        // Update average completion time
        if (PlayCount == 1)
        {
            AverageCompletionTime = completionTimeSeconds;
        }
        else
        {
            AverageCompletionTime = ((AverageCompletionTime * (PlayCount - 1)) + completionTimeSeconds) / PlayCount;
        }

        // Update average star rating
        if (PlayCount == 1)
        {
            AverageStarRating = starsEarned;
        }
        else
        {
            AverageStarRating = ((AverageStarRating * (PlayCount - 1)) + starsEarned) / PlayCount;
        }
    }
}