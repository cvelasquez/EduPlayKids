using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserProgress entity operations.
/// Manages individual child learning progress, star ratings, and adaptive learning analytics.
/// Essential for tracking educational achievement and personalizing learning experiences.
/// </summary>
public interface IUserProgressRepository : IGenericRepository<UserProgress>
{
    #region Child Progress Tracking

    /// <summary>
    /// Gets all progress records for a specific child across all activities.
    /// Essential for comprehensive learning analytics and progress reporting.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records for the child</returns>
    Task<IEnumerable<UserProgress>> GetProgressByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress for a specific child and activity combination.
    /// Used for tracking individual activity performance and determining next steps.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The progress record if exists, null otherwise</returns>
    Task<UserProgress?> GetProgressByChildAndActivityAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completed activities for a child with their star ratings.
    /// Used for achievement tracking and progress visualization.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of completed activities with star ratings</returns>
    Task<IEnumerable<UserProgress>> GetCompletedProgressByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets in-progress activities for a child (started but not completed).
    /// Important for allowing children to resume their learning sessions.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of in-progress activity records</returns>
    Task<IEnumerable<UserProgress>> GetInProgressByChildAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Star Rating and Performance Analysis

    /// <summary>
    /// Gets the average star rating for a child across all completed activities.
    /// Key metric for understanding overall learning performance and achievement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The average star rating for the child</returns>
    Task<double> GetAverageStarRatingByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the average star rating for a child within a specific subject.
    /// Used for subject-specific performance analysis and curriculum adaptation.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The average star rating for the subject</returns>
    Task<double> GetAverageStarRatingByChildAndSubjectAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress records with 3-star ratings (perfect scores).
    /// Used for identifying mastery and celebrating excellent performance.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of 3-star progress records</returns>
    Task<IEnumerable<UserProgress>> GetPerfectScoresByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress records below a specified star rating threshold.
    /// Helps identify areas where a child may need additional support.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="maxStarRating">Maximum star rating threshold</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records below the threshold</returns>
    Task<IEnumerable<UserProgress>> GetLowPerformanceByChildAsync(int childId, double maxStarRating, CancellationToken cancellationToken = default);

    #endregion

    #region Subject and Activity Analysis

    /// <summary>
    /// Gets progress statistics for a specific activity across all children.
    /// Provides insights into activity difficulty and engagement levels.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing activity performance statistics</returns>
    Task<Dictionary<string, object>> GetActivityProgressStatisticsAsync(int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress for all children in a specific subject.
    /// Used for subject performance analysis and curriculum effectiveness assessment.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records for the subject</returns>
    Task<IEnumerable<UserProgress>> GetProgressBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completion rate for a specific activity.
    /// Important metric for understanding content effectiveness and difficulty.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The completion rate as a percentage</returns>
    Task<double> GetActivityCompletionRateAsync(int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Learning Streaks and Consistency

    /// <summary>
    /// Gets the current learning streak for a child (consecutive days with activity).
    /// Important for gamification and encouraging consistent learning habits.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The number of consecutive days with learning activity</returns>
    Task<int> GetCurrentStreakByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the longest learning streak achieved by a child.
    /// Used for achievement recognition and motivation.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The longest streak of consecutive learning days</returns>
    Task<int> GetLongestStreakByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress records for a child within a specific date range.
    /// Used for analyzing learning patterns and session frequency.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records within the date range</returns>
    Task<IEnumerable<UserProgress>> GetProgressInDateRangeAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion

    #region Adaptive Learning and Recommendations

    /// <summary>
    /// Gets progress data for determining the next appropriate difficulty level.
    /// Used by adaptive learning algorithms to personalize content difficulty.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Recommended difficulty level based on performance</returns>
    Task<string> GetRecommendedDifficultyLevelAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects where a child is performing exceptionally well.
    /// Used for identifying strengths and providing crown challenges.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="minStarRating">Minimum average star rating for excellence</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects where child excels</returns>
    Task<IEnumerable<int>> GetExcellentSubjectsByChildAsync(int childId, double minStarRating, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects where a child may need additional support.
    /// Used for identifying areas requiring more practice or different approaches.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="maxStarRating">Maximum average star rating indicating struggle</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects needing support</returns>
    Task<IEnumerable<int>> GetStrugglingSubjectsByChildAsync(int childId, double maxStarRating, CancellationToken cancellationToken = default);

    #endregion

    #region Time and Session Analysis

    /// <summary>
    /// Gets the total time spent by a child on learning activities.
    /// Important for screen time monitoring and usage analytics.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for calculation (optional)</param>
    /// <param name="toDate">End date for calculation (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total time spent in minutes</returns>
    Task<int> GetTotalTimeSpentByChildAsync(int childId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the average time spent per activity for a child.
    /// Used for understanding engagement levels and pacing preferences.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Average time per activity in minutes</returns>
    Task<double> GetAverageTimePerActivityByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress records with unusually long completion times.
    /// May indicate activities that are too challenging or technical issues.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="maxTimeMinutes">Maximum expected time in minutes</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records with long completion times</returns>
    Task<IEnumerable<UserProgress>> GetLongSessionsByChildAsync(int childId, int maxTimeMinutes, CancellationToken cancellationToken = default);

    #endregion

    #region Progress Updates and Tracking

    /// <summary>
    /// Updates or creates progress for a child's activity completion.
    /// Central method for recording learning achievements and performance.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="starsEarned">Number of stars earned (1-3)</param>
    /// <param name="timeSpentMinutes">Time spent on the activity</param>
    /// <param name="errorsCount">Number of errors made</param>
    /// <param name="isCompleted">Whether the activity was completed</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated or created progress record</returns>
    Task<UserProgress> UpdateProgressAsync(int childId, int activityId, int starsEarned, int timeSpentMinutes, int errorsCount, bool isCompleted, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an activity as started for a child (creates initial progress record).
    /// Important for tracking learning session beginnings and resumption.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The initial progress record</returns>
    Task<UserProgress> StartActivityAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    #endregion

    #region Analytics and Reporting

    /// <summary>
    /// Gets learning analytics summary for parental dashboard reporting.
    /// Provides comprehensive overview of child's educational progress.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing comprehensive learning analytics</returns>
    Task<Dictionary<string, object>> GetLearningAnalyticsSummaryAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comparative performance data across all children (anonymized).
    /// Used for understanding relative performance and curriculum effectiveness.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier (optional)</param>
    /// <param name="ageGroup">Age group filter (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing comparative performance metrics</returns>
    Task<Dictionary<string, object>> GetComparativePerformanceDataAsync(int? activityId = null, string? ageGroup = null, CancellationToken cancellationToken = default);

    #endregion

    #region Additional Missing Methods (for compilation fixes)

    /// <summary>
    /// Gets list of completed activity IDs for a specific child.
    /// Used for determining which activities are unlocked and which need to be shown.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>List of completed activity IDs</returns>
    Task<List<int>> GetCompletedActivityIdsAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent progress records for a child within a specified number of days.
    /// Used for analyzing recent learning patterns and performance.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="days">Number of days to look back for recent progress</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recent progress records</returns>
    Task<IEnumerable<UserProgress>> GetRecentProgressAsync(int childId, int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets progress records for a child filtered by subject.
    /// Used for subject-specific progress analysis and reporting.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of progress records for the subject</returns>
    Task<IEnumerable<UserProgress>> GetSubjectProgressAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets question attempt history for adaptive learning and difficulty adjustment.
    /// Used for tracking individual question performance and learning analytics.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing question attempt history</returns>
    Task<Dictionary<int, object>> GetQuestionAttemptHistoryAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a child has completed a specific activity.
    /// Used for prerequisite validation and content unlocking logic.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if the activity has been completed</returns>
    Task<bool> HasCompletedActivityAsync(int childId, int activityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records the completion of an activity with comprehensive progress data.
    /// Used for creating detailed progress records when activities are finished.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="starsEarned">Number of stars earned (1-3)</param>
    /// <param name="timeSpentMinutes">Time spent on the activity in minutes</param>
    /// <param name="errorsCount">Number of errors made during the activity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created progress record</returns>
    Task<UserProgress> RecordActivityCompletionAsync(int childId, int activityId, int starsEarned, int timeSpentMinutes, int errorsCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance data for specific activity types to analyze learning patterns.
    /// Used for understanding how children perform across different activity types.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityType">The activity type to analyze</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing activity type performance metrics</returns>
    Task<Dictionary<string, object>> GetActivityTypePerformanceAsync(int childId, string activityType, CancellationToken cancellationToken = default);

    #endregion
}