using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserAchievement entity operations.
/// Manages earned achievements, progress tracking, and celebration systems.
/// Supports child-safe recognition and motivational milestone management.
/// </summary>
public interface IUserAchievementRepository : IGenericRepository<UserAchievement>
{
    #region Child Achievement Tracking

    /// <summary>
    /// Gets all achievements earned by a specific child.
    /// Essential for displaying child's accomplishments and progress history.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements earned by the child</returns>
    Task<IEnumerable<UserAchievement>> GetAchievementsByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently earned achievements by a child for celebration features.
    /// Used for highlighting recent accomplishments and maintaining motivation.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="daysFromNow">Number of days to look back for recent achievements</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recently earned achievements</returns>
    Task<IEnumerable<UserAchievement>> GetRecentAchievementsByChildAsync(int childId, int daysFromNow = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a child has earned a specific achievement.
    /// Prevents duplicate achievement awards and validates recognition status.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the child has earned the achievement, false otherwise</returns>
    Task<bool> HasChildEarnedAchievementAsync(int childId, int achievementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the specific user achievement record for a child and achievement.
    /// Used for accessing earn date, celebration status, and achievement details.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user achievement record if exists, null otherwise</returns>
    Task<UserAchievement?> GetUserAchievementAsync(int childId, int achievementId, CancellationToken cancellationToken = default);

    #endregion

    #region Achievement Categories and Progress

    /// <summary>
    /// Gets achievements earned by a child in a specific category.
    /// Used for category-specific recognition displays and progress tracking.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="category">The achievement category</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of earned achievements in the category</returns>
    Task<IEnumerable<UserAchievement>> GetAchievementsByChildAndCategoryAsync(int childId, string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets crown achievements earned by a child for elite recognition.
    /// Highlights exceptional performance and advanced accomplishments.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of crown achievements earned by the child</returns>
    Task<IEnumerable<UserAchievement>> GetCrownAchievementsByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets streak achievements earned by a child for consistency recognition.
    /// Shows recognition for regular learning habits and sustained engagement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of streak achievements earned by the child</returns>
    Task<IEnumerable<UserAchievement>> GetStreakAchievementsByChildAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Achievement Statistics and Leaderboards

    /// <summary>
    /// Gets the total count of achievements earned by a child.
    /// Key metric for overall accomplishment tracking and motivation.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of achievements earned</returns>
    Task<int> GetAchievementCountByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievement counts by category for a child.
    /// Provides detailed breakdown of accomplishments across learning areas.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with category names and achievement counts</returns>
    Task<Dictionary<string, int>> GetAchievementCountsByCategoryAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets child-safe leaderboard data for achievement counts.
    /// Anonymous ranking system that motivates without revealing identities.
    /// </summary>
    /// <param name="ageGroup">Optional age group filter for fair comparison</param>
    /// <param name="topCount">Number of top performers to include</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Anonymous leaderboard data with achievement counts</returns>
    Task<List<Dictionary<string, object>>> GetAnonymousAchievementLeaderboardAsync(string? ageGroup = null, int topCount = 10, CancellationToken cancellationToken = default);

    #endregion

    #region Celebration and Notification Management

    /// <summary>
    /// Gets achievements that haven't been celebrated yet for notification systems.
    /// Ensures all earned achievements receive proper recognition and celebration.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of uncelebrated achievements</returns>
    Task<IEnumerable<UserAchievement>> GetUncelebratedAchievementsByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks achievements as celebrated after recognition display.
    /// Prevents repeated notifications for the same accomplishments.
    /// </summary>
    /// <param name="userAchievementIds">Collection of user achievement identifiers</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task MarkAchievementsAsCelebratedAsync(IEnumerable<int> userAchievementIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Awards an achievement to a child with proper validation and celebration setup.
    /// Central method for recognizing accomplishments and triggering celebrations.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created user achievement record</returns>
    Task<UserAchievement> AwardAchievementToChildAsync(int childId, int achievementId, CancellationToken cancellationToken = default);

    #endregion

    #region Subject and Activity Specific Achievements

    /// <summary>
    /// Gets achievements earned by a child in a specific subject.
    /// Used for subject-specific recognition and progress visualization.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subject-specific achievements</returns>
    Task<IEnumerable<UserAchievement>> GetAchievementsByChildAndSubjectAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievements related to specific activities completed by a child.
    /// Links accomplishments to specific learning experiences and content.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of activity-related achievements</returns>
    Task<IEnumerable<UserAchievement>> GetAchievementsByChildAndActivityAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Time-Based and Streak Analysis

    /// <summary>
    /// Gets achievements earned within a specific date range for periodic analysis.
    /// Used for tracking achievement velocity and motivation patterns.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements earned within the date range</returns>
    Task<IEnumerable<UserAchievement>> GetAchievementsInDateRangeAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the child's longest achievement earning streak for recognition.
    /// Measures sustained engagement and consistent accomplishment patterns.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The longest streak of consecutive days with achievement earnings</returns>
    Task<int> GetLongestAchievementStreakByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the child's current achievement earning streak for motivation.
    /// Shows ongoing accomplishment patterns and encourages continued effort.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The current streak of consecutive days with achievement earnings</returns>
    Task<int> GetCurrentAchievementStreakByChildAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Family and Multi-Child Features

    /// <summary>
    /// Gets achievements earned by all children in a family for family celebration.
    /// Supports multi-child family dynamics and shared accomplishment recognition.
    /// </summary>
    /// <param name="userId">The parent/guardian user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements earned by all family children</returns>
    Task<IEnumerable<UserAchievement>> GetFamilyAchievementsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets family achievement statistics for parental dashboard.
    /// Provides comprehensive overview of all children's accomplishments.
    /// </summary>
    /// <param name="userId">The parent/guardian user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing family achievement statistics</returns>
    Task<Dictionary<string, object>> GetFamilyAchievementStatisticsAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Milestone and Progress Tracking

    /// <summary>
    /// Gets milestone achievements that represent significant learning progress.
    /// Highlights major educational accomplishments and developmental markers.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of milestone achievements earned by the child</returns>
    Task<IEnumerable<UserAchievement>> GetMilestoneAchievementsByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress towards uncompleted achievements for motivation display.
    /// Shows how close a child is to earning specific recognitions.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Progress percentage towards the achievement (0-100)</returns>
    Task<double> GetProgressTowardsAchievementAsync(int childId, int achievementId, CancellationToken cancellationToken = default);

    #endregion
}