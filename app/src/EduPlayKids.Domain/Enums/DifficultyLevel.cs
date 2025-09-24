namespace EduPlayKids.Domain.Enums;

/// <summary>
/// Represents difficulty levels for educational activities targeting children aged 3-8.
/// Progressive difficulty system aligned with pre-K through grade 2 curriculum standards.
/// </summary>
public enum DifficultyLevel
{
    /// <summary>
    /// Easy level - suitable for ages 3-4 (Pre-K)
    /// Simple recognition, basic concepts, large visual elements
    /// </summary>
    Easy = 1,

    /// <summary>
    /// Medium level - suitable for ages 5-6 (Kindergarten to Grade 1)
    /// Introduction of logic, pattern recognition, basic problem solving
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Hard level - suitable for ages 7-8 (Grade 1-2)
    /// Complex problem solving, multi-step activities, abstract thinking
    /// </summary>
    Hard = 3
}