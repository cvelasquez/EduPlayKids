using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents an interactive question or task within an activity.
/// Contains the educational content, answer options, and multimedia assets.
/// Core component for delivering interactive learning experiences to children.
/// </summary>
public class ActivityQuestion : BaseEntity
{
    /// <summary>
    /// Gets or sets the question text in English.
    /// The main prompt or instruction for the child.
    /// </summary>
    [StringLength(1000)]
    public string? QuestionTextEn { get; set; }

    /// <summary>
    /// Gets or sets the question text in Spanish.
    /// Bilingual support for Spanish-speaking children.
    /// </summary>
    [StringLength(1000)]
    public string? QuestionTextEs { get; set; }

    /// <summary>
    /// Gets or sets the question type identifier.
    /// Categories: MultipleChoice, DragDrop, Matching, Tracing, TrueFalse, etc.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string QuestionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display order within the activity.
    /// Determines the sequence of questions in the activity.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the points awarded for correct completion.
    /// Used for scoring and star rating calculations.
    /// </summary>
    [Range(1, 100)]
    public int Points { get; set; } = 10;

    /// <summary>
    /// Gets or sets the question configuration data.
    /// Flexible JSON structure containing question-specific settings:
    /// - Answer options for multiple choice
    /// - Drag/drop zone definitions
    /// - Matching pairs
    /// - Tracing paths
    /// - Interactive element positions
    /// </summary>
    public string? ConfigurationData { get; set; }

    /// <summary>
    /// Gets or sets the correct answer(s) for this question.
    /// Stored as JSON to support various answer formats:
    /// - Single values for simple questions
    /// - Arrays for multiple correct answers
    /// - Objects for complex matching/positioning
    /// </summary>
    public string? CorrectAnswer { get; set; }

    /// <summary>
    /// Gets or sets the explanation text in English.
    /// Shown after answering to reinforce learning.
    /// </summary>
    [StringLength(500)]
    public string? ExplanationEn { get; set; }

    /// <summary>
    /// Gets or sets the explanation text in Spanish.
    /// Bilingual explanation for reinforcement.
    /// </summary>
    [StringLength(500)]
    public string? ExplanationEs { get; set; }

    /// <summary>
    /// Gets or sets the question audio file path in English.
    /// Critical for non-readers in the 3-5 age group.
    /// </summary>
    [StringLength(255)]
    public string? QuestionAudioEnPath { get; set; }

    /// <summary>
    /// Gets or sets the question audio file path in Spanish.
    /// Bilingual audio support for Spanish-speaking children.
    /// </summary>
    [StringLength(255)]
    public string? QuestionAudioEsPath { get; set; }

    /// <summary>
    /// Gets or sets the main image file path for the question.
    /// Visual content to support comprehension and engagement.
    /// </summary>
    [StringLength(255)]
    public string? ImagePath { get; set; }

    /// <summary>
    /// Gets or sets additional media assets for the question.
    /// JSON array of media objects with paths and types.
    /// Supports animations, sound effects, and interactive elements.
    /// </summary>
    public string? MediaAssets { get; set; }

    /// <summary>
    /// Gets or sets the hint text in English.
    /// Shown when the child needs help with the question.
    /// </summary>
    [StringLength(300)]
    public string? HintEn { get; set; }

    /// <summary>
    /// Gets or sets the hint text in Spanish.
    /// Bilingual hint for additional support.
    /// </summary>
    [StringLength(300)]
    public string? HintEs { get; set; }

    /// <summary>
    /// Gets or sets the hint audio file path in English.
    /// Audio support for hints to assist struggling children.
    /// </summary>
    [StringLength(255)]
    public string? HintAudioEnPath { get; set; }

    /// <summary>
    /// Gets or sets the hint audio file path in Spanish.
    /// Bilingual audio hint support.
    /// </summary>
    [StringLength(255)]
    public string? HintAudioEsPath { get; set; }

    /// <summary>
    /// Gets or sets the time limit for this question in seconds.
    /// Zero means no time limit. Used for crown challenges.
    /// </summary>
    public int TimeLimitSeconds { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether this question allows multiple attempts.
    /// Educational approach: allow learning from mistakes.
    /// </summary>
    public bool AllowMultipleAttempts { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of attempts allowed.
    /// Zero means unlimited attempts.
    /// </summary>
    public int MaxAttempts { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether hints are available.
    /// Can be disabled for assessment or crown challenge questions.
    /// </summary>
    public bool HintsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this question is active.
    /// Controls question availability within the activity.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the difficulty level of this specific question.
    /// May differ from the overall activity difficulty.
    /// Values: Easy, Medium, Hard
    /// </summary>
    [StringLength(20)]
    public string DifficultyLevel { get; set; } = "Easy";

    /// <summary>
    /// Gets or sets learning tags for this question.
    /// JSON array of skill/concept tags for adaptive learning.
    /// Examples: ["counting", "addition", "shapes", "colors"]
    /// </summary>
    public string? LearningTags { get; set; }

    /// <summary>
    /// Gets or sets accessibility features for this question.
    /// JSON object with special needs accommodations.
    /// Supports WCAG compliance and inclusive design.
    /// </summary>
    public string? AccessibilityFeatures { get; set; }

    /// <summary>
    /// Gets or sets the number of times this question has been answered correctly.
    /// Analytics data for content optimization.
    /// </summary>
    public int CorrectAnswerCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of attempts on this question.
    /// Used to calculate success rates and difficulty assessment.
    /// </summary>
    public int TotalAttemptCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the foreign key to the activity this question belongs to.
    /// Establishes the hierarchical relationship.
    /// </summary>
    [Required]
    public int ActivityId { get; set; }

    /// <summary>
    /// Navigation property: The activity this question belongs to.
    /// </summary>
    public virtual Activity Activity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the ActivityQuestion class.
    /// Sets default values for new question creation.
    /// </summary>
    public ActivityQuestion()
    {
        Points = 10;
        TimeLimitSeconds = 0;
        AllowMultipleAttempts = true;
        MaxAttempts = 0;
        HintsEnabled = true;
        IsActive = true;
        DifficultyLevel = "Easy";
        CorrectAnswerCount = 0;
        TotalAttemptCount = 0;
    }

    /// <summary>
    /// Gets the localized question text based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The question text in the requested language.</returns>
    public string? GetLocalizedQuestionText(string language)
    {
        return language.ToLower() switch
        {
            "en" => QuestionTextEn,
            "es" => QuestionTextEs,
            _ => QuestionTextEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized explanation based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The explanation in the requested language.</returns>
    public string? GetLocalizedExplanation(string language)
    {
        return language.ToLower() switch
        {
            "en" => ExplanationEn,
            "es" => ExplanationEs,
            _ => ExplanationEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized hint based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The hint in the requested language.</returns>
    public string? GetLocalizedHint(string language)
    {
        return language.ToLower() switch
        {
            "en" => HintEn,
            "es" => HintEs,
            _ => HintEn // Default to English
        };
    }

    /// <summary>
    /// Gets the localized question audio path based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The audio path in the requested language.</returns>
    public string? GetLocalizedQuestionAudioPath(string language)
    {
        return language.ToLower() switch
        {
            "en" => QuestionAudioEnPath,
            "es" => QuestionAudioEsPath,
            _ => QuestionAudioEnPath // Default to English
        };
    }

    /// <summary>
    /// Gets the localized hint audio path based on the specified language.
    /// </summary>
    /// <param name="language">Language code: "en" for English, "es" for Spanish.</param>
    /// <returns>The hint audio path in the requested language.</returns>
    public string? GetLocalizedHintAudioPath(string language)
    {
        return language.ToLower() switch
        {
            "en" => HintAudioEnPath,
            "es" => HintAudioEsPath,
            _ => HintAudioEnPath // Default to English
        };
    }

    /// <summary>
    /// Validates if the provided answer is correct.
    /// Handles various answer formats based on question type.
    /// </summary>
    /// <param name="userAnswer">The answer provided by the user.</param>
    /// <returns>True if the answer is correct, false otherwise.</returns>
    public bool IsAnswerCorrect(string userAnswer)
    {
        if (string.IsNullOrEmpty(CorrectAnswer) || string.IsNullOrEmpty(userAnswer))
            return false;

        try
        {
            // For simple string comparison
            if (CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase))
                return true;

            // For JSON array answers (multiple correct answers)
            var correctAnswers = System.Text.Json.JsonSerializer.Deserialize<string[]>(CorrectAnswer);
            if (correctAnswers != null)
            {
                return correctAnswers.Any(answer =>
                    answer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase));
            }
        }
        catch
        {
            // If JSON parsing fails, fall back to string comparison
            return CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Records an attempt on this question for analytics.
    /// </summary>
    /// <param name="wasCorrect">Whether the attempt was correct.</param>
    public void RecordAttempt(bool wasCorrect)
    {
        TotalAttemptCount++;
        if (wasCorrect)
        {
            CorrectAnswerCount++;
        }
    }

    /// <summary>
    /// Calculates the success rate for this question.
    /// </summary>
    /// <returns>Success rate as a percentage (0-100).</returns>
    public double GetSuccessRate()
    {
        if (TotalAttemptCount == 0) return 0;
        return Math.Round((double)CorrectAnswerCount / TotalAttemptCount * 100, 1);
    }

    /// <summary>
    /// Determines if this question should show hints based on difficulty and settings.
    /// </summary>
    /// <param name="attemptNumber">Current attempt number for this child.</param>
    /// <returns>True if hints should be available, false otherwise.</returns>
    public bool ShouldShowHints(int attemptNumber)
    {
        if (!HintsEnabled) return false;

        // Show hints after first failed attempt for Easy questions
        // After second failed attempt for Medium questions
        // After third failed attempt for Hard questions
        var hintThreshold = DifficultyLevel switch
        {
            "Easy" => 1,
            "Medium" => 2,
            "Hard" => 3,
            _ => 1
        };

        return attemptNumber > hintThreshold;
    }
}