using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ActivityQuestion entity operations.
/// Manages educational questions, answer validation, and bilingual audio content.
/// Supports interactive learning experiences and accessibility features for children 3-8.
/// </summary>
public interface IActivityQuestionRepository : IGenericRepository<ActivityQuestion>
{
    #region Activity-Specific Questions

    /// <summary>
    /// Gets all questions for a specific activity in proper sequence.
    /// Essential for structured question presentation and activity flow.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions ordered by sequence</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsByActivityAsync(int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific question by its sequence number within an activity.
    /// Used for direct question navigation and progress tracking.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="sequenceNumber">The question's sequence number</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The question at the specified sequence</returns>
    Task<ActivityQuestion?> GetQuestionBySequenceAsync(int activityId, int sequenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of questions in an activity.
    /// Important for progress indicators and completion tracking.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total count of questions in the activity</returns>
    Task<int> GetQuestionCountByActivityAsync(int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Question Type Filtering

    /// <summary>
    /// Gets questions filtered by question type for specific learning interactions.
    /// Supports various question formats (MultipleChoice, TrueFalse, DragDrop, etc.).
    /// </summary>
    /// <param name="questionType">The type of questions to retrieve</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions of the specified type</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsByTypeAsync(string questionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions with audio content for non-readers and accessibility.
    /// Critical for supporting children who cannot read yet (ages 3-5).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions with audio instructions</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsWithAudioAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions by difficulty level for adaptive learning.
    /// Used for personalizing question complexity based on child performance.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level (Easy, Medium, Hard)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions at the specified difficulty</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsByDifficultyAsync(string difficultyLevel, CancellationToken cancellationToken = default);

    #endregion

    #region Bilingual Audio Content

    /// <summary>
    /// Gets questions with audio content in a specific language.
    /// Supports Spanish/English bilingual learning experiences.
    /// </summary>
    /// <param name="language">The language code ("es" for Spanish, "en" for English)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions with audio in the specified language</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsWithAudioByLanguageAsync(string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the audio file path for a question in a specific language.
    /// Used for managing bilingual audio content and localization.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="language">The language code</param>
    /// <param name="audioFilePath">The new audio file path</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateAudioFilePathAsync(int questionId, string language, string audioFilePath, CancellationToken cancellationToken = default);

    #endregion

    #region Answer Validation and Analytics

    /// <summary>
    /// Validates a child's answer for a specific question.
    /// Core method for learning assessment and immediate feedback.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="childAnswer">The child's provided answer</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the answer is correct, false otherwise</returns>
    Task<bool> ValidateAnswerAsync(int questionId, string childAnswer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions where children commonly make errors.
    /// Used for identifying challenging content that may need review.
    /// </summary>
    /// <param name="minErrorRate">Minimum error rate percentage to consider</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions with high error rates</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsWithHighErrorRatesAsync(double minErrorRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance statistics for a specific question.
    /// Includes correct/incorrect answer rates and timing data.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing question performance metrics</returns>
    Task<Dictionary<string, object>> GetQuestionPerformanceStatisticsAsync(int questionId, CancellationToken cancellationToken = default);

    #endregion

    #region Accessibility and Special Needs Support

    /// <summary>
    /// Gets questions with visual accessibility features for children with special needs.
    /// Includes high contrast, large text, and visual cue support.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of accessibility-enhanced questions</returns>
    Task<IEnumerable<ActivityQuestion>> GetAccessibleQuestionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions that support motor skill adaptations.
    /// Important for children with fine motor skill development needs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of motor-adaptive questions</returns>
    Task<IEnumerable<ActivityQuestion>> GetMotorAdaptiveQuestionsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Content Management and Updates

    /// <summary>
    /// Updates question content and metadata for curriculum improvements.
    /// Used for educational content updates and quality enhancements.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="questionText">Updated question text (optional)</param>
    /// <param name="correctAnswer">Updated correct answer (optional)</param>
    /// <param name="explanation">Updated explanation (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateQuestionContentAsync(int questionId, string? questionText = null, string? correctAnswer = null, string? explanation = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions that need content review based on performance metrics.
    /// Identifies questions that may require educational improvements.
    /// </summary>
    /// <param name="maxSuccessRate">Maximum success rate indicating need for review</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions needing review</returns>
    Task<IEnumerable<ActivityQuestion>> GetQuestionsNeedingReviewAsync(double maxSuccessRate, CancellationToken cancellationToken = default);

    #endregion

    #region Random Question Selection

    /// <summary>
    /// Gets a random selection of questions from an activity for varied experiences.
    /// Used for adaptive testing and preventing memorization.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="count">Number of random questions to select</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of randomly selected questions</returns>
    Task<IEnumerable<ActivityQuestion>> GetRandomQuestionsFromActivityAsync(int activityId, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets questions randomly selected by difficulty level.
    /// Supports adaptive learning with varied difficulty presentation.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level for selection</param>
    /// <param name="count">Number of questions to select</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of randomly selected questions at the difficulty level</returns>
    Task<IEnumerable<ActivityQuestion>> GetRandomQuestionsByDifficultyAsync(string difficultyLevel, int count, CancellationToken cancellationToken = default);

    #endregion

    #region Additional Missing Methods (for compilation fixes)

    /// <summary>
    /// Gets all questions for a specific activity by activity ID.
    /// Alternative method name for activity question retrieval.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of questions for the activity</returns>
    Task<IEnumerable<ActivityQuestion>> GetByActivityIdAsync(int activityId, CancellationToken cancellationToken = default);

    #endregion
}