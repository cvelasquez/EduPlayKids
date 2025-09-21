using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Achievement entity operations.
/// Manages gamification elements, motivational milestones, and child-safe recognition systems.
/// Supports crown challenges, learning streaks, and age-appropriate achievement celebrations.
/// </summary>
public interface IAchievementRepository : IGenericRepository<Achievement>
{
    #region Age-Appropriate Achievement Filtering

    /// <summary>
    /// Gets achievements appropriate for a specific age range.
    /// Essential for delivering age-appropriate recognition and motivation.
    /// </summary>
    /// <param name="minAge">Minimum age for the achievement</param>
    /// <param name="maxAge">Maximum age for the achievement</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of age-appropriate achievements</returns>
    Task<IEnumerable<Achievement>> GetAchievementsByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements specifically designed for Pre-K children (ages 3-4).
    /// Focuses on basic milestones and foundational learning recognition.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Pre-K appropriate achievements</returns>
    Task<IEnumerable<Achievement>> GetPreKAchievementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements for Kindergarten children (age 5).
    /// Focuses on school readiness and transitional skill recognition.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Kindergarten appropriate achievements</returns>
    Task<IEnumerable<Achievement>> GetKindergartenAchievementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements for Primary grade children (ages 6-8).
    /// Focuses on advanced learning milestones and skill mastery recognition.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Primary grade appropriate achievements</returns>
    Task<IEnumerable<Achievement>> GetPrimaryGradeAchievementsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Achievement Categories and Types

    /// <summary>
    /// Gets achievements filtered by category for organized recognition systems.
    /// Categories include Learning, Progress, Streak, Special, Crown, etc.
    /// </summary>
    /// <param name="category">The achievement category</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements in the specified category</returns>
    Task<IEnumerable<Achievement>> GetAchievementsByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets crown challenge achievements for high-performing children.
    /// Special recognition for consistently excellent performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of crown challenge achievements</returns>
    Task<IEnumerable<Achievement>> GetCrownAchievementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets streak-based achievements for consistency recognition.
    /// Motivates regular learning habits and daily engagement.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of streak-based achievements</returns>
    Task<IEnumerable<Achievement>> GetStreakAchievementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subject-specific achievements for targeted recognition.
    /// Celebrates mastery and progress in specific learning areas.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subject-specific achievements</returns>
    Task<IEnumerable<Achievement>> GetAchievementsBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Difficulty and Progression Levels

    /// <summary>
    /// Gets achievements ordered by difficulty level for progressive recognition.
    /// Ensures appropriate challenge levels for sustained motivation.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level (Easy, Medium, Hard)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements at the specified difficulty</returns>
    Task<IEnumerable<Achievement>> GetAchievementsByDifficultyAsync(string difficultyLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next achievement a child can work towards in a specific category.
    /// Provides clear goals and motivation for continued learning.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="category">The achievement category</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The next available achievement for the child</returns>
    Task<Achievement?> GetNextAchievementForChildAsync(int childId, string category, CancellationToken cancellationToken = default);

    #endregion

    #region Child Eligibility and Progress

    /// <summary>
    /// Gets achievements that a child is eligible to earn based on their progress.
    /// Considers current performance, completed activities, and learning metrics.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements the child can earn</returns>
    Task<IEnumerable<Achievement>> GetEligibleAchievementsForChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements that a child is close to earning for motivation.
    /// Shows progress towards goals and encourages continued effort.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="progressThreshold">Minimum progress percentage to consider "close"</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of nearly-achievable achievements</returns>
    Task<IEnumerable<Achievement>> GetNearlyEarnedAchievementsForChildAsync(int childId, double progressThreshold = 0.75, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a child meets the criteria for a specific achievement.
    /// Validates achievement eligibility based on performance and progress data.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the child meets the criteria, false otherwise</returns>
    Task<bool> IsChildEligibleForAchievementAsync(int childId, int achievementId, CancellationToken cancellationToken = default);

    #endregion

    #region Popular and Featured Achievements

    /// <summary>
    /// Gets the most commonly earned achievements for popularity insights.
    /// Used for understanding engagement patterns and effective motivators.
    /// </summary>
    /// <param name="topCount">Number of top achievements to return</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of most popular achievements</returns>
    Task<IEnumerable<Achievement>> GetMostPopularAchievementsAsync(int topCount = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets featured achievements for highlighting special recognition opportunities.
    /// Used for promoting specific learning goals and seasonal celebrations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of featured achievements</returns>
    Task<IEnumerable<Achievement>> GetFeaturedAchievementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets rare achievements that few children have earned.
    /// Highlights exceptional accomplishments and elite recognition.
    /// </summary>
    /// <param name="maxEarnedCount">Maximum number of children who have earned it</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of rare achievements</returns>
    Task<IEnumerable<Achievement>> GetRareAchievementsAsync(int maxEarnedCount = 10, CancellationToken cancellationToken = default);

    #endregion

    #region Achievement Statistics and Analytics

    /// <summary>
    /// Gets achievement statistics including earn rates and completion data.
    /// Provides insights into achievement system effectiveness and engagement.
    /// </summary>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing achievement statistics</returns>
    Task<Dictionary<string, object>> GetAchievementStatisticsAsync(int achievementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overall achievement system analytics for performance monitoring.
    /// Includes earn rates, popular categories, and engagement metrics.
    /// </summary>
    /// <param name="fromDate">Start date for analysis period</param>
    /// <param name="toDate">End date for analysis period</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing system-wide achievement analytics</returns>
    Task<Dictionary<string, object>> GetAchievementSystemAnalyticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion

    #region Seasonal and Special Event Achievements

    /// <summary>
    /// Gets achievements associated with seasonal events and celebrations.
    /// Includes holiday-themed and time-limited recognition opportunities.
    /// </summary>
    /// <param name="season">The season or event identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of seasonal achievements</returns>
    Task<IEnumerable<Achievement>> GetSeasonalAchievementsAsync(string season, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets currently active time-limited achievements.
    /// Shows special recognition opportunities available for a limited time.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of active time-limited achievements</returns>
    Task<IEnumerable<Achievement>> GetActiveTimeLimitedAchievementsAsync(CancellationToken cancellationToken = default);

    #endregion
}