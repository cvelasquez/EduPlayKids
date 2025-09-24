using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Activity entity operations.
/// Manages educational activities, difficulty progression, and curriculum alignment.
/// Implements sequential unlocking, star rating systems, and adaptive learning features.
/// </summary>
public interface IActivityRepository : IGenericRepository<Activity>
{
    #region Difficulty and Progression Management

    /// <summary>
    /// Gets activities filtered by difficulty level for appropriate content delivery.
    /// Essential for age-appropriate learning and progressive skill development.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level (Easy, Medium, Hard)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities at the specified difficulty level</returns>
    Task<IEnumerable<Activity>> GetActivitiesByDifficultyAsync(string difficultyLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities within a range of difficulty levels for adaptive learning.
    /// Used when children need content between two difficulty levels.
    /// </summary>
    /// <param name="minDifficultyLevel">Minimum difficulty level</param>
    /// <param name="maxDifficultyLevel">Maximum difficulty level</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities within the difficulty range</returns>
    Task<IEnumerable<Activity>> GetActivitiesByDifficultyRangeAsync(string minDifficultyLevel, string maxDifficultyLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next activity in sequence for a child's learning progression.
    /// Implements sequential unlocking based on completion and mastery.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The next activity to be unlocked for the child</returns>
    Task<Activity?> GetNextActivityForChildAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities that are unlocked and available for a specific child.
    /// Based on their progress, age, and completed prerequisites.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject identifier (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of available activities for the child</returns>
    Task<IEnumerable<Activity>> GetAvailableActivitiesForChildAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Subject and Curriculum Organization

    /// <summary>
    /// Gets all activities for a specific subject ordered by sequence.
    /// Essential for structured curriculum delivery and learning path management.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities for the subject in sequential order</returns>
    Task<IEnumerable<Activity>> GetActivitiesBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities by subject with their associated questions for complete activity loading.
    /// Optimized for reducing database queries when loading activity content.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities with loaded questions</returns>
    Task<IEnumerable<Activity>> GetActivitiesWithQuestionsBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities by multiple subjects for cross-curricular learning.
    /// Used for integrated learning experiences and subject correlation.
    /// </summary>
    /// <param name="subjectIds">Collection of subject identifiers</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities from the specified subjects</returns>
    Task<IEnumerable<Activity>> GetActivitiesBySubjectsAsync(IEnumerable<int> subjectIds, CancellationToken cancellationToken = default);

    #endregion

    #region Age-Appropriate Content Filtering

    /// <summary>
    /// Gets activities appropriate for a specific age group.
    /// Critical for delivering age-appropriate educational content to children 3-8.
    /// </summary>
    /// <param name="minAge">Minimum age for the activity</param>
    /// <param name="maxAge">Maximum age for the activity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of age-appropriate activities</returns>
    Task<IEnumerable<Activity>> GetActivitiesByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities specifically designed for Pre-K children (ages 3-4).
    /// Focuses on foundational skills and basic concepts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Pre-K appropriate activities</returns>
    Task<IEnumerable<Activity>> GetPreKActivitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities specifically designed for Kindergarten children (age 5).
    /// Focuses on school readiness and transitional skills.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Kindergarten appropriate activities</returns>
    Task<IEnumerable<Activity>> GetKindergartenActivitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities specifically designed for Primary grade children (ages 6-8).
    /// Focuses on advanced learning concepts and skill mastery.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Primary grade appropriate activities</returns>
    Task<IEnumerable<Activity>> GetPrimaryGradeActivitiesAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Star Rating and Performance Analysis

    /// <summary>
    /// Gets activities with high average star ratings for featuring quality content.
    /// Used for showcasing excellent educational activities and curriculum highlights.
    /// </summary>
    /// <param name="minStarRating">Minimum average star rating</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of highly-rated activities</returns>
    Task<IEnumerable<Activity>> GetHighRatedActivitiesAsync(double minStarRating, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities that may need content review based on low star ratings.
    /// Helps identify activities that might require educational improvements.
    /// </summary>
    /// <param name="maxStarRating">Maximum average star rating indicating potential issues</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities that may need review</returns>
    Task<IEnumerable<Activity>> GetActivitiesNeedingReviewAsync(double maxStarRating, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance statistics for a specific activity.
    /// Includes completion rates, average star ratings, and difficulty analytics.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing activity performance metrics</returns>
    Task<Dictionary<string, object>> GetActivityPerformanceStatisticsAsync(int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Crown Challenges and Advanced Content

    /// <summary>
    /// Gets activities designated as crown challenges for high-performing children.
    /// These are advanced activities that provide additional challenges and recognition.
    /// </summary>
    /// <param name="subjectId">The subject identifier (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of crown challenge activities</returns>
    Task<IEnumerable<Activity>> GetCrownChallengeActivitiesAsync(int? subjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets crown challenge activities available for a specific child based on their performance.
    /// Only children who consistently achieve high star ratings should access these.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of crown challenges available to the child</returns>
    Task<IEnumerable<Activity>> GetAvailableCrownChallengesForChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if a child qualifies for crown challenge activities.
    /// Based on their performance history and star rating achievements.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the child qualifies for crown challenges</returns>
    Task<bool> IsChildEligibleForCrownChallengesAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Prerequisite and Dependency Management

    /// <summary>
    /// Gets activities that are prerequisites for a specific activity.
    /// Essential for maintaining proper learning sequence and skill building.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of prerequisite activities</returns>
    Task<IEnumerable<Activity>> GetPrerequisiteActivitiesAsync(int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities that depend on a specific activity as a prerequisite.
    /// Used for understanding learning progression and content unlocking.
    /// </summary>
    /// <param name="activityId">The prerequisite activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of dependent activities</returns>
    Task<IEnumerable<Activity>> GetDependentActivitiesAsync(int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if all prerequisites are completed for a specific child and activity.
    /// Critical for enforcing proper learning sequence and content gating.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if all prerequisites are met</returns>
    Task<bool> ArePrerequisitesMetAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Content Search and Discovery

    /// <summary>
    /// Searches activities by title or description for content discovery.
    /// Supports parent and educator content exploration.
    /// </summary>
    /// <param name="searchTerm">The search term to match against activity content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities matching the search criteria</returns>
    Task<IEnumerable<Activity>> SearchActivitiesAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recommended activities for a child based on their learning profile.
    /// Uses adaptive algorithms to suggest appropriate next activities.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="maxRecommendations">Maximum number of recommendations to return</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recommended activities</returns>
    Task<IEnumerable<Activity>> GetRecommendedActivitiesForChildAsync(int childId, int maxRecommendations = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently added activities for featuring new content.
    /// Used for highlighting fresh educational materials and content updates.
    /// </summary>
    /// <param name="daysFromNow">Number of days to look back for recent additions</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recently added activities</returns>
    Task<IEnumerable<Activity>> GetRecentlyAddedActivitiesAsync(int daysFromNow, CancellationToken cancellationToken = default);

    #endregion

    #region Progress and Completion Tracking

    /// <summary>
    /// Gets completion statistics for an activity across all children.
    /// Provides insights into activity difficulty and engagement levels.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing completion statistics</returns>
    Task<Dictionary<string, object>> GetActivityCompletionStatisticsAsync(int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities with the highest completion rates for showcasing popular content.
    /// Used for identifying engaging and successful educational activities.
    /// </summary>
    /// <param name="minCompletionRate">Minimum completion rate percentage</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of highly completed activities</returns>
    Task<IEnumerable<Activity>> GetMostCompletedActivitiesAsync(double minCompletionRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities that a child has completed with their performance details.
    /// Includes star ratings, completion times, and attempt history.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of completed activities with performance data</returns>
    Task<IEnumerable<Activity>> GetCompletedActivitiesByChildAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Additional Missing Methods (for compilation fixes)

    /// <summary>
    /// Gets activities by subject ID for curriculum organization and content delivery.
    /// Essential for loading subject-specific activities in proper sequence.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activities for the specified subject</returns>
    Task<IEnumerable<Activity>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single activity with all its associated questions loaded.
    /// Optimized for activity delivery with complete question sets.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The activity with loaded questions, or null if not found</returns>
    Task<Activity?> GetByIdWithQuestionsAsync(int activityId, CancellationToken cancellationToken = default);

    #endregion
}