using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents an educational subject area in the EduPlayKids curriculum.
/// Defines the core learning domains with bilingual support and age-appropriate content organization.
/// Covers: Math, Reading/Phonics, Basic Concepts, Logic & Thinking, and Science.
/// </summary>
public class Subject : BaseEntity
{
    /// <summary>
    /// Gets or sets the subject name in English.
    /// Primary identifier for the educational domain.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject name in Spanish.
    /// Supports bilingual content delivery for Hispanic families.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NameEs { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject description in English.
    /// Explains the learning objectives and content scope.
    /// </summary>
    [StringLength(500)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// Gets or sets the subject description in Spanish.
    /// Bilingual description for parent understanding.
    /// </summary>
    [StringLength(500)]
    public string? DescriptionEs { get; set; }

    /// <summary>
    /// Gets or sets the subject identifier/code.
    /// Used for internal references and curriculum mapping.
    /// Examples: MATH, READING, CONCEPTS, LOGIC, SCIENCE
    /// </summary>
    [Required]
    [StringLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject icon identifier.
    /// References the icon asset for visual representation.
    /// </summary>
    [StringLength(50)]
    public string? IconId { get; set; }

    /// <summary>
    /// Gets or sets the subject's primary color in hex format.
    /// Used for consistent visual theming across the app.
    /// </summary>
    [StringLength(7)]
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the subject's secondary color in hex format.
    /// Used for gradients and complementary design elements.
    /// </summary>
    [StringLength(7)]
    public string? SecondaryColor { get; set; }

    /// <summary>
    /// Gets or sets the display order for subject listing.
    /// Determines the sequence in navigation and menus.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the minimum age for this subject.
    /// Determines content availability based on child's age.
    /// </summary>
    [Range(3, 8)]
    public int MinAge { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum age for this subject.
    /// Upper bound for age-appropriate content.
    /// </summary>
    [Range(3, 8)]
    public int MaxAge { get; set; } = 8;

    /// <summary>
    /// Gets or sets a value indicating whether this subject is active.
    /// Allows for content management and seasonal availability.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this subject requires premium access.
    /// Controls freemium model content restrictions.
    /// </summary>
    public bool RequiresPremium { get; set; } = false;

    /// <summary>
    /// Gets or sets the curriculum standards this subject aligns with.
    /// References US educational standards for content validation.
    /// Stored as JSON for multiple standard references.
    /// </summary>
    public string? CurriculumStandards { get; set; }

    /// <summary>
    /// Gets or sets learning objectives for this subject.
    /// Defines what children should achieve upon completion.
    /// Stored as JSON for structured data.
    /// </summary>
    public string? LearningObjectives { get; set; }

    /// <summary>
    /// Gets or sets prerequisite subjects for this learning area.
    /// Defines learning dependencies and recommended sequences.
    /// Stored as JSON array of subject IDs.
    /// </summary>
    public string? Prerequisites { get; set; }

    /// <summary>
    /// Gets or sets the estimated completion time in hours.
    /// Helps parents understand the scope of each subject area.
    /// </summary>
    public decimal? EstimatedCompletionHours { get; set; }

    /// <summary>
    /// Navigation property: Collection of activities within this subject.
    /// Organizes learning content by educational domain.
    /// </summary>
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    /// <summary>
    /// Navigation property: Collection of children who have this as their favorite subject.
    /// </summary>
    public virtual ICollection<Child> FavoritedByChildren { get; set; } = new List<Child>();

    /// <summary>
    /// Initializes a new instance of the Subject class.
    /// Sets default values for new subject creation.
    /// </summary>
    public Subject()
    {
        IsActive = true;
        RequiresPremium = false;
        MinAge = 3;
        MaxAge = 8;
    }

    /// <summary>
    /// Gets the localized name based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The subject name in the requested language.</returns>
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
    /// <returns>The subject description in the requested language.</returns>
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
    /// Determines if this subject is appropriate for the given child's age.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <returns>True if the subject is age-appropriate, false otherwise.</returns>
    public bool IsAgeAppropriate(int childAge)
    {
        return childAge >= MinAge && childAge <= MaxAge;
    }

    /// <summary>
    /// Determines if this subject is available for the given child considering age and premium status.
    /// </summary>
    /// <param name="childAge">The child's age in years.</param>
    /// <param name="hasPremiumAccess">Whether the user has premium access.</param>
    /// <returns>True if the subject is available, false otherwise.</returns>
    public bool IsAvailableForChild(int childAge, bool hasPremiumAccess)
    {
        return IsActive &&
               IsAgeAppropriate(childAge) &&
               (!RequiresPremium || hasPremiumAccess);
    }

    /// <summary>
    /// Gets the total number of activities in this subject for a specific age group.
    /// </summary>
    /// <param name="childAge">The child's age to filter activities.</param>
    /// <returns>Number of age-appropriate activities.</returns>
    public int GetActivityCountForAge(int childAge)
    {
        return Activities.Count(a => a.IsActive && a.IsAgeAppropriate(childAge));
    }
}