using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Child entity operations.
/// Manages individual child profiles, learning preferences, and educational progress.
/// Implements age-specific content filtering and personalized learning experiences.
/// </summary>
public interface IChildRepository : IGenericRepository<Child>
{
    #region Age-Specific Operations

    /// <summary>
    /// Gets children within a specific age range for targeted educational content.
    /// Essential for age-appropriate curriculum delivery (Pre-K 3-4, Kindergarten 5, Grade 1-2 6-8).
    /// </summary>
    /// <param name="minAge">Minimum age in years</param>
    /// <param name="maxAge">Maximum age in years</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children within the specified age range</returns>
    Task<IEnumerable<Child>> GetChildrenByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children by specific age for precise educational targeting.
    /// Used for birthday transitions and age-specific content recommendations.
    /// </summary>
    /// <param name="age">The exact age in years</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children of the specified age</returns>
    Task<IEnumerable<Child>> GetChildrenByAgeAsync(int age, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children in Pre-K age group (3-4 years) for foundational learning content.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Pre-K children</returns>
    Task<IEnumerable<Child>> GetPreKChildrenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children in Kindergarten age group (5 years) for transitional learning content.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Kindergarten children</returns>
    Task<IEnumerable<Child>> GetKindergartenChildrenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children in Primary grade age group (6-8 years) for advanced learning content.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Primary grade children</returns>
    Task<IEnumerable<Child>> GetPrimaryGradeChildrenAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Family and User Association

    /// <summary>
    /// Gets all children associated with a specific parent/guardian user.
    /// Essential for multi-child family management and individual progress tracking.
    /// </summary>
    /// <param name="userId">The parent/guardian user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children belonging to the specified user</returns>
    Task<IEnumerable<Child>> GetChildrenByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a child with their associated user/parent information.
    /// Useful for parental control verification and family context.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The child with loaded user information</returns>
    Task<Child?> GetChildWithUserAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a child belongs to a specific user for security purposes.
    /// Critical for preventing unauthorized access to child data.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the child belongs to the user, false otherwise</returns>
    Task<bool> ValidateChildOwnershipAsync(int childId, int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Learning Preferences and Personalization

    /// <summary>
    /// Gets children with their complete learning profiles including progress and achievements.
    /// Optimized for loading comprehensive educational data efficiently.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The child with loaded progress and achievement data</returns>
    Task<Child?> GetChildWithLearningProfileAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children by their preferred learning difficulty level.
    /// Used for adaptive learning and personalized content delivery.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level (Easy, Medium, Hard)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children with the specified difficulty preference</returns>
    Task<IEnumerable<Child>> GetChildrenByDifficultyLevelAsync(string difficultyLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a child's learning preferences and personalization settings.
    /// Affects content recommendations and adaptive learning algorithms.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="preferredDifficulty">The new preferred difficulty level</param>
    /// <param name="learningStyle">The child's learning style preferences</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateLearningPreferencesAsync(int childId, string? preferredDifficulty = null, string? learningStyle = null, CancellationToken cancellationToken = default);

    #endregion

    #region Progress and Achievement Tracking

    /// <summary>
    /// Gets children with outstanding educational progress for recognition and celebration.
    /// Based on star ratings, completion rates, and learning streaks.
    /// </summary>
    /// <param name="minStarAverage">Minimum average star rating</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of high-performing children</returns>
    Task<IEnumerable<Child>> GetHighPerformingChildrenAsync(double minStarAverage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children who need additional learning support based on performance metrics.
    /// Helps identify children who may benefit from adjusted content or parental involvement.
    /// </summary>
    /// <param name="maxStarAverage">Maximum average star rating indicating need for support</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children who may need additional support</returns>
    Task<IEnumerable<Child>> GetChildrenNeedingSupportAsync(double maxStarAverage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets learning progress summary for a specific child.
    /// Includes completion rates, star averages, and recent activity statistics.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing progress metrics and statistics</returns>
    Task<Dictionary<string, object>> GetChildProgressSummaryAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Avatar and Personalization

    /// <summary>
    /// Updates a child's avatar and visual personalization settings.
    /// Important for child engagement and personal connection to the app.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="avatarImagePath">Path to the new avatar image</param>
    /// <param name="favoriteColor">The child's favorite color for UI theming</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateAvatarAndPersonalizationAsync(int childId, string? avatarImagePath = null, string? favoriteColor = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children by their favorite color for personalized UI theming.
    /// Used for creating color-coordinated learning experiences.
    /// </summary>
    /// <param name="favoriteColor">The color preference</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children with the specified color preference</returns>
    Task<IEnumerable<Child>> GetChildrenByFavoriteColorAsync(string favoriteColor, CancellationToken cancellationToken = default);

    #endregion

    #region Activity and Engagement Tracking

    /// <summary>
    /// Gets children who have been active within a specified number of days.
    /// Used for engagement analytics and identifying regular users.
    /// </summary>
    /// <param name="daysFromNow">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recently active children</returns>
    Task<IEnumerable<Child>> GetRecentlyActiveChildrenAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets children who haven't been active for a specified number of days.
    /// Important for re-engagement strategies and identifying at-risk learners.
    /// </summary>
    /// <param name="daysInactive">Number of days of inactivity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of inactive children</returns>
    Task<IEnumerable<Child>> GetInactiveChildrenAsync(int daysInactive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a child's last activity timestamp for engagement tracking.
    /// Called whenever a child interacts with educational content.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityTime">The timestamp of the last activity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateLastActivityAsync(int childId, DateTime activityTime, CancellationToken cancellationToken = default);

    #endregion

    #region Birthday and Age Transitions

    /// <summary>
    /// Gets children whose birthday is within a specified number of days.
    /// Used for birthday celebrations and age-appropriate content transitions.
    /// </summary>
    /// <param name="daysFromNow">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children with upcoming birthdays</returns>
    Task<IEnumerable<Child>> GetChildrenWithUpcomingBirthdaysAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a child's age when they have a birthday.
    /// Triggers content level adjustments and age-appropriate curriculum transitions.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="newAge">The child's new age</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateChildAgeAsync(int childId, int newAge, CancellationToken cancellationToken = default);

    #endregion

    #region Analytics and Reporting

    /// <summary>
    /// Gets child registration statistics by age group for analytics.
    /// Used for understanding user demographics and content planning.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with age group distribution statistics</returns>
    Task<Dictionary<string, int>> GetChildrenByAgeGroupStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets learning engagement statistics for all children.
    /// Provides insights into app usage patterns and educational effectiveness.
    /// </summary>
    /// <param name="fromDate">Start date for the analysis period</param>
    /// <param name="toDate">End date for the analysis period</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing engagement metrics and statistics</returns>
    Task<Dictionary<string, object>> GetLearningEngagementStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion
}