using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Services;

/// <summary>
/// Service interface for delivering educational activities to children.
/// Handles dynamic content loading, age-appropriate filtering, and activity progression.
/// Core service for educational content delivery in the EduPlayKids application.
/// </summary>
public interface IActivityDeliveryService
{
    #region Activity Discovery and Loading

    /// <summary>
    /// Gets activities appropriate for a child's age and current progress level.
    /// Filters by subject, difficulty, and prerequisite completion status.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject to load activities from</param>
    /// <param name="includeCompleted">Whether to include already completed activities</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of available activities with metadata</returns>
    Task<IEnumerable<Activity>> GetAvailableActivitiesAsync(int childId, int subjectId, bool includeCompleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a complete activity with all questions and multimedia assets.
    /// Prepares activity for interactive delivery to the child.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="childId">The child's unique identifier for personalization</param>
    /// <param name="language">Language preference ("en" or "es")</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Complete activity with questions and child-specific adaptations</returns>
    Task<Activity?> LoadActivityForChildAsync(int activityId, int childId, string language = "en", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next recommended activity for a child based on learning progression.
    /// Uses adaptive algorithms to select optimal next learning experience.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for recommendations</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Recommended next activity with reasoning metadata</returns>
    Task<Activity?> GetNextRecommendedActivityAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Question Delivery and Management

    /// <summary>
    /// Gets questions for an activity in the correct sequence with child-appropriate adaptations.
    /// Handles question randomization, difficulty adjustment, and language preferences.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="childId">The child's unique identifier for personalization</param>
    /// <param name="language">Language preference ("en" or "es")</param>
    /// <param name="randomizeOrder">Whether to randomize question order</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Ordered collection of activity questions</returns>
    Task<IEnumerable<ActivityQuestion>> GetActivityQuestionsAsync(int activityId, int childId, string language = "en", bool randomizeOrder = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a specific question with all multimedia assets and configuration data.
    /// Prepares question for interactive rendering in the UI.
    /// </summary>
    /// <param name="questionId">The question's unique identifier</param>
    /// <param name="childId">The child's unique identifier for personalization</param>
    /// <param name="language">Language preference ("en" or "es")</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Complete question with all assets and child-specific adaptations</returns>
    Task<ActivityQuestion?> LoadQuestionForChildAsync(int questionId, int childId, string language = "en", CancellationToken cancellationToken = default);

    #endregion

    #region Content Personalization and Adaptation

    /// <summary>
    /// Adapts activity content based on child's learning profile and performance history.
    /// Adjusts difficulty, hint availability, and multimedia preferences.
    /// </summary>
    /// <param name="activity">The base activity to adapt</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Personalized activity adapted for the specific child</returns>
    Task<Activity> PersonalizeActivityForChildAsync(Activity activity, int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if additional support features should be enabled for a child.
    /// Considers age, performance history, and special needs accommodations.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityType">The type of activity being attempted</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary of support features to enable</returns>
    Task<Dictionary<string, bool>> GetSupportFeaturesForChildAsync(int childId, string activityType, CancellationToken cancellationToken = default);

    #endregion

    #region Progress and Unlocking Logic

    /// <summary>
    /// Checks if an activity is unlocked and available for a specific child.
    /// Validates prerequisites, age appropriateness, and premium access requirements.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if activity is available, false with reason if locked</returns>
    Task<(bool IsUnlocked, string? LockReason)> IsActivityUnlockedForChildAsync(int activityId, int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the progression requirements for unlocking the next set of activities.
    /// Provides clear goals for continued learning advancement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject to check progression in</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Progression requirements with completion percentages</returns>
    Task<Dictionary<string, object>> GetProgressionRequirementsAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes activity completion and updates child's progression status.
    /// Unlocks new activities, awards achievements, and adjusts difficulty preferences.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The completed activity's identifier</param>
    /// <param name="completionData">Activity completion results and performance data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Updated progression status with newly unlocked content</returns>
    Task<Dictionary<string, object>> ProcessActivityCompletionAsync(int childId, int activityId, Dictionary<string, object> completionData, CancellationToken cancellationToken = default);

    #endregion

    #region Crown Challenges and Advanced Content

    /// <summary>
    /// Determines if a child is eligible for crown challenge activities.
    /// Analyzes performance patterns and mastery levels across subjects.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for crown challenge eligibility</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Crown challenge eligibility with recommended activities</returns>
    Task<Dictionary<string, object>> GetCrownChallengeEligibilityAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads crown challenge activities for high-performing children.
    /// Provides advanced content with increased difficulty and time challenges.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject to load crown challenges from</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of available crown challenge activities</returns>
    Task<IEnumerable<Activity>> GetCrownChallengeActivitiesAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Content Analytics and Optimization

    /// <summary>
    /// Tracks content engagement and effectiveness for optimization.
    /// Records child interactions, completion rates, and learning outcomes.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity being tracked</param>
    /// <param name="interactionData">Detailed interaction and performance data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Tracking confirmation with updated content metrics</returns>
    Task<bool> TrackContentEngagementAsync(int childId, int activityId, Dictionary<string, object> interactionData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content performance metrics for activity optimization.
    /// Analyzes success rates, engagement patterns, and learning effectiveness.
    /// </summary>
    /// <param name="activityId">Optional specific activity to analyze</param>
    /// <param name="ageGroup">Optional age group filter</param>
    /// <param name="daysBack">Number of days to include in analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Content performance metrics and optimization recommendations</returns>
    Task<Dictionary<string, object>> GetContentPerformanceMetricsAsync(int? activityId = null, string? ageGroup = null, int daysBack = 30, CancellationToken cancellationToken = default);

    #endregion
}