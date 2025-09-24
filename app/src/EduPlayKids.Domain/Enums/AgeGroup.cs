namespace EduPlayKids.Domain.Enums;

/// <summary>
/// Represents age groups for educational content targeting and curriculum alignment.
/// Based on US education system grade levels for children aged 3-8.
/// </summary>
public enum AgeGroup
{
    /// <summary>
    /// Pre-K ages 3-4 - early learning readiness, basic concepts
    /// </summary>
    PreK = 1,

    /// <summary>
    /// Kindergarten age 5 - school readiness, foundational skills
    /// </summary>
    Kindergarten = 2,

    /// <summary>
    /// Grade 1 age 6 - formal learning introduction, reading/math basics
    /// </summary>
    Grade1 = 3,

    /// <summary>
    /// Grade 2 ages 7-8 - skill building, complex problem solving
    /// </summary>
    Grade2 = 4,

    /// <summary>
    /// All age groups - universal content suitable for all children 3-8
    /// </summary>
    All = 5,

    /// <summary>
    /// Primary grades - general elementary education (grades 1-2, ages 6-8)
    /// </summary>
    Primary = 6
}