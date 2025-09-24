using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Services;

/// <summary>
/// Service interface for validating educational content answers and providing feedback.
/// Handles different question types, age-appropriate feedback, and learning analytics.
/// </summary>
public interface IAnswerValidationService
{
    #region Answer Validation

    /// <summary>
    /// Validates a child's answer for a specific question.
    /// Provides comprehensive validation results with educational feedback.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="userAnswer">The answer provided by the child</param>
    /// <param name="timeSpentSeconds">Time spent on the question in seconds</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Validation result with feedback and learning insights</returns>
    Task<AnswerValidationResult> ValidateAnswerAsync(int questionId, int childId, object userAnswer, int timeSpentSeconds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates multiple answers for a complete activity.
    /// Calculates overall performance and provides activity-level feedback.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="answers">Collection of question answers</param>
    /// <param name="totalTimeSpentSeconds">Total time spent on the activity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Activity validation result with star rating and achievements</returns>
    Task<ActivityValidationResult> ValidateActivityAnswersAsync(int activityId, int childId, IEnumerable<QuestionAnswer> answers, int totalTimeSpentSeconds, CancellationToken cancellationToken = default);

    #endregion

    #region Feedback Generation

    /// <summary>
    /// Generates age-appropriate feedback messages for correct answers.
    /// Provides encouraging and celebratory messages to reinforce learning.
    /// </summary>
    /// <param name="childAge">The child's age for age-appropriate messaging</param>
    /// <param name="questionType">The type of question answered</param>
    /// <param name="isFirstAttempt">Whether this was the first attempt</param>
    /// <param name="language">Language preference for feedback</param>
    /// <returns>Encouraging feedback message</returns>
    string GeneratePositiveFeedback(int childAge, string questionType, bool isFirstAttempt, string language = "en");

    /// <summary>
    /// Generates supportive feedback messages for incorrect answers.
    /// Provides encouraging guidance without negative language.
    /// </summary>
    /// <param name="childAge">The child's age for age-appropriate messaging</param>
    /// <param name="questionType">The type of question answered</param>
    /// <param name="attemptNumber">Current attempt number</param>
    /// <param name="hasHintsAvailable">Whether hints are available for this question</param>
    /// <param name="language">Language preference for feedback</param>
    /// <returns>Supportive and encouraging feedback message</returns>
    string GenerateSupportiveFeedback(int childAge, string questionType, int attemptNumber, bool hasHintsAvailable, string language = "en");

    /// <summary>
    /// Generates completion feedback for an entire activity.
    /// Celebrates achievements and provides motivation for continued learning.
    /// </summary>
    /// <param name="childAge">The child's age for age-appropriate messaging</param>
    /// <param name="starsEarned">Number of stars earned (1-3)</param>
    /// <param name="correctAnswers">Number of correct answers</param>
    /// <param name="totalQuestions">Total number of questions</param>
    /// <param name="language">Language preference for feedback</param>
    /// <returns>Activity completion feedback message</returns>
    string GenerateActivityCompletionFeedback(int childAge, int starsEarned, int correctAnswers, int totalQuestions, string language = "en");

    #endregion

    #region Learning Analytics

    /// <summary>
    /// Analyzes answer patterns to identify learning strengths and areas for improvement.
    /// Provides insights for adaptive difficulty adjustment and content recommendations.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for analysis</param>
    /// <param name="daysBack">Number of days to include in analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Learning pattern analysis with recommendations</returns>
    Task<LearningPatternAnalysis> AnalyzeAnswerPatternsAsync(int childId, int? subjectId = null, int daysBack = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies common error patterns for a specific question or question type.
    /// Helps improve content design and targeted remediation.
    /// </summary>
    /// <param name="questionId">Optional specific question to analyze</param>
    /// <param name="questionType">Optional question type filter</param>
    /// <param name="ageGroup">Optional age group filter</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Error pattern analysis with improvement suggestions</returns>
    Task<ErrorPatternAnalysis> IdentifyErrorPatternsAsync(int? questionId = null, string? questionType = null, string? ageGroup = null, CancellationToken cancellationToken = default);

    #endregion

    #region Star Rating and Achievement Calculation

    /// <summary>
    /// Calculates star rating for an activity based on performance criteria.
    /// Uses age-appropriate standards and encourages improvement.
    /// </summary>
    /// <param name="correctAnswers">Number of correct answers</param>
    /// <param name="totalQuestions">Total number of questions</param>
    /// <param name="timeSpentSeconds">Time spent on the activity</param>
    /// <param name="estimatedMinutes">Estimated completion time for the activity</param>
    /// <param name="isFirstAttempt">Whether this was the first attempt at the activity</param>
    /// <param name="childAge">Child's age for age-appropriate criteria</param>
    /// <returns>Star rating (1-3) with calculation details</returns>
    StarRatingResult CalculateStarRating(int correctAnswers, int totalQuestions, int timeSpentSeconds, int estimatedMinutes, bool isFirstAttempt, int childAge);

    /// <summary>
    /// Checks for achievement milestones based on recent performance.
    /// Identifies badges, streaks, and other motivational achievements.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityCompletion">Latest activity completion data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>List of newly earned achievements</returns>
    Task<IEnumerable<AchievementResult>> CheckForAchievementsAsync(int childId, ActivityValidationResult activityCompletion, CancellationToken cancellationToken = default);

    #endregion

    #region Difficulty Adjustment Recommendations

    /// <summary>
    /// Analyzes performance to recommend difficulty adjustments.
    /// Ensures optimal challenge level for continued engagement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="recentAnswers">Recent answer validation results</param>
    /// <param name="currentDifficulty">Current difficulty level</param>
    /// <returns>Difficulty adjustment recommendation with reasoning</returns>
    DifficultyAdjustmentRecommendation AnalyzeDifficultyNeed(int childId, IEnumerable<AnswerValidationResult> recentAnswers, string currentDifficulty);

    /// <summary>
    /// Suggests personalized learning adjustments based on answer patterns.
    /// Recommends content types, support features, and learning strategies.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="analysisWindowDays">Days to analyze for recommendations</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Personalized learning recommendations</returns>
    Task<PersonalizedLearningRecommendations> GenerateLearningRecommendationsAsync(int childId, int analysisWindowDays = 14, CancellationToken cancellationToken = default);

    #endregion
}

#region Result Models

/// <summary>
/// Result of answer validation with feedback and analytics.
/// </summary>
public class AnswerValidationResult
{
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public string FeedbackMessage { get; set; } = string.Empty;
    public string? ExplanationMessage { get; set; }
    public int AttemptNumber { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime ValidationTime { get; set; }
    public Dictionary<string, object> Analytics { get; set; } = new();
}

/// <summary>
/// Result of activity validation with overall performance metrics.
/// </summary>
public class ActivityValidationResult
{
    public int ActivityId { get; set; }
    public int ChildId { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public int StarsEarned { get; set; }
    public int TotalTimeSpentSeconds { get; set; }
    public double AccuracyPercentage { get; set; }
    public string CompletionMessage { get; set; } = string.Empty;
    public List<AnswerValidationResult> QuestionResults { get; set; } = new();
    public DateTime CompletionTime { get; set; }
    public bool IsFirstCompletion { get; set; }
}

/// <summary>
/// Question answer data for validation.
/// </summary>
public class QuestionAnswer
{
    public int QuestionId { get; set; }
    public object UserAnswer { get; set; } = new();
    public int TimeSpentSeconds { get; set; }
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the answer was correct.
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Gets or sets the number of attempts made for this question.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the time spent on this question (alias for TimeSpentSeconds).
    /// </summary>
    public int TimeSpent => TimeSpentSeconds;
}

/// <summary>
/// Learning pattern analysis results.
/// </summary>
public class LearningPatternAnalysis
{
    public int ChildId { get; set; }
    public Dictionary<string, double> StrengthsBySubject { get; set; } = new();
    public Dictionary<string, double> StrengthsByQuestionType { get; set; } = new();
    public List<string> RecommendedTopics { get; set; } = new();
    public string LearningStyle { get; set; } = string.Empty;
    public double OverallProgress { get; set; }
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Error pattern analysis results.
/// </summary>
public class ErrorPatternAnalysis
{
    public Dictionary<string, int> CommonErrors { get; set; } = new();
    public List<string> ImprovementSuggestions { get; set; } = new();
    public double ErrorRate { get; set; }
    public string MostChallengingConcept { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Star rating calculation result.
/// </summary>
public class StarRatingResult
{
    public int Stars { get; set; }
    public string Criteria { get; set; } = string.Empty;
    public double AccuracyScore { get; set; }
    public double TimeScore { get; set; }
    public double BonusScore { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}

/// <summary>
/// Achievement result for newly earned achievements.
/// </summary>
public class AchievementResult
{
    public int AchievementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BadgeImagePath { get; set; } = string.Empty;
    public DateTime EarnedDate { get; set; }
    public string CelebrationMessage { get; set; } = string.Empty;

    // Bilingual support properties
    public string? NameEn { get; set; }
    public string? DescriptionEn { get; set; }
    public string? BadgeIcon { get; set; }
}

/// <summary>
/// Difficulty adjustment recommendation.
/// </summary>
public class DifficultyAdjustmentRecommendation
{
    public string CurrentDifficulty { get; set; } = string.Empty;
    public string RecommendedDifficulty { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public bool ShouldAdjust { get; set; }
}

/// <summary>
/// Personalized learning recommendations.
/// </summary>
public class PersonalizedLearningRecommendations
{
    public List<string> RecommendedActivityTypes { get; set; } = new();
    public Dictionary<string, bool> SupportFeatures { get; set; } = new();
    public List<string> FocusAreas { get; set; } = new();
    public string OptimalSessionLength { get; set; } = string.Empty;
    public List<string> MotivationalStrategies { get; set; } = new();
}

#endregion