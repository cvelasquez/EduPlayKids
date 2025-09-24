namespace EduPlayKids.Domain.Enums;

/// <summary>
/// Represents different types of educational activities available in the application.
/// Each type corresponds to core curriculum areas for early childhood education.
/// </summary>
public enum ActivityType
{
    /// <summary>
    /// Mathematics activities - counting, numbers, basic operations, shapes
    /// </summary>
    Mathematics = 1,

    /// <summary>
    /// Reading and phonics activities - alphabet, phonics, sight words, comprehension
    /// </summary>
    Reading = 2,

    /// <summary>
    /// Basic concepts - colors, shapes, patterns, classification
    /// </summary>
    BasicConcepts = 3,

    /// <summary>
    /// Logic and thinking activities - puzzles, memory games, problem solving
    /// </summary>
    Logic = 4,

    /// <summary>
    /// Science activities - animals, plants, weather, nature exploration
    /// </summary>
    Science = 5,

    /// <summary>
    /// Mixed activities combining multiple subject areas
    /// </summary>
    Mixed = 6,

    /// <summary>
    /// Multiple choice questions with selectable options
    /// </summary>
    MultipleChoice = 7,

    /// <summary>
    /// Drag and drop activities for interactive learning
    /// </summary>
    DragAndDrop = 8,

    /// <summary>
    /// Matching activities connecting related items
    /// </summary>
    Matching = 9,

    /// <summary>
    /// Fill in the blank questions for reading comprehension
    /// </summary>
    FillInTheBlank = 10
}