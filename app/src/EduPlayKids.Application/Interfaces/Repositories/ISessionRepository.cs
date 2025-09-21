using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Session entity operations.
/// Manages COPPA-compliant usage analytics, screen time monitoring, and learning session tracking.
/// Essential for parental controls, educational analytics, and child safety compliance.
/// </summary>
public interface ISessionRepository : IGenericRepository<Session>
{
    #region Child Session Tracking

    /// <summary>
    /// Gets all sessions for a specific child for comprehensive usage analysis.
    /// Essential for parental monitoring and learning pattern analysis.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of sessions for the child</returns>
    Task<IEnumerable<Session>> GetSessionsByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sessions for a child within a specific date range.
    /// Used for periodic usage reports and screen time monitoring.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of sessions within the date range</returns>
    Task<IEnumerable<Session>> GetSessionsByChildInDateRangeAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current active session for a child if one exists.
    /// Important for session continuation and proper session management.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The active session if exists, null otherwise</returns>
    Task<Session?> GetActiveSessionByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent completed session for a child.
    /// Used for resuming learning activities and progress tracking.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The most recent completed session</returns>
    Task<Session?> GetLastCompletedSessionByChildAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Screen Time and Usage Analytics

    /// <summary>
    /// Gets total screen time for a child within a specific period.
    /// Critical for parental controls and healthy usage monitoring.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for calculation</param>
    /// <param name="toDate">End date for calculation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total screen time in minutes</returns>
    Task<int> GetTotalScreenTimeAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets daily screen time for a child for the past specified days.
    /// Used for daily usage tracking and limit enforcement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="days">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with dates and daily screen time in minutes</returns>
    Task<Dictionary<DateTime, int>> GetDailyScreenTimeAsync(int childId, int days = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets average session duration for a child to understand usage patterns.
    /// Helps identify optimal session lengths and engagement levels.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodDays">Period in days for calculation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Average session duration in minutes</returns>
    Task<double> GetAverageSessionDurationAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a child has exceeded their daily usage limit.
    /// Essential for enforcing parental controls and healthy screen time.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="dailyLimitMinutes">The daily limit in minutes</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if limit is exceeded, false otherwise</returns>
    Task<bool> HasExceededDailyLimitAsync(int childId, int dailyLimitMinutes, CancellationToken cancellationToken = default);

    #endregion

    #region Learning Session Analytics

    /// <summary>
    /// Gets sessions with learning activities completed for educational analytics.
    /// Filters productive learning sessions from general app usage.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="minActivitiesCompleted">Minimum activities completed to qualify</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of productive learning sessions</returns>
    Task<IEnumerable<Session>> GetLearningSessionsByChildAsync(int childId, int minActivitiesCompleted = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sessions where the child achieved high star ratings.
    /// Used for identifying successful learning periods and optimal conditions.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="minAverageStars">Minimum average star rating for the session</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of high-performance sessions</returns>
    Task<IEnumerable<Session>> GetHighPerformanceSessionsByChildAsync(int childId, double minAverageStars = 2.5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets learning session statistics for educational effectiveness analysis.
    /// Provides comprehensive metrics for session quality and engagement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing learning session statistics</returns>
    Task<Dictionary<string, object>> GetLearningSessionStatisticsAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Session Management Operations

    /// <summary>
    /// Starts a new session for a child with proper initialization.
    /// Creates session record and begins tracking for analytics and monitoring.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The newly created session</returns>
    Task<Session> StartSessionAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends an active session with final metrics and completion data.
    /// Calculates duration, activities completed, and performance metrics.
    /// </summary>
    /// <param name="sessionId">The session's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The completed session with final metrics</returns>
    Task<Session> EndSessionAsync(int sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates session progress with activity completion and performance data.
    /// Called throughout the session to track real-time learning metrics.
    /// </summary>
    /// <param name="sessionId">The session's unique identifier</param>
    /// <param name="activitiesCompleted">Current count of completed activities</param>
    /// <param name="averageStarRating">Current average star rating</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateSessionProgressAsync(int sessionId, int activitiesCompleted, double averageStarRating, CancellationToken cancellationToken = default);

    #endregion

    #region Family and Multi-Child Analytics

    /// <summary>
    /// Gets session data for all children in a family for parental overview.
    /// Supports multi-child family monitoring and comparative analysis.
    /// </summary>
    /// <param name="userId">The parent/guardian user's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of sessions for all family children</returns>
    Task<IEnumerable<Session>> GetFamilySessionsAsync(int userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets family usage statistics for comprehensive parental dashboard.
    /// Provides overview of all children's app usage and learning patterns.
    /// </summary>
    /// <param name="userId">The parent/guardian user's unique identifier</param>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing family usage statistics</returns>
    Task<Dictionary<string, object>> GetFamilyUsageStatisticsAsync(int userId, int periodDays = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Time-Based Usage Patterns

    /// <summary>
    /// Gets usage patterns by time of day for understanding optimal learning times.
    /// Helps identify when children are most engaged and productive.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with hour of day and usage statistics</returns>
    Task<Dictionary<int, int>> GetUsagePatternsByHourAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets weekly usage patterns for understanding learning schedule preferences.
    /// Identifies which days of the week show highest engagement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodWeeks">Analysis period in weeks</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with day of week and usage statistics</returns>
    Task<Dictionary<DayOfWeek, int>> GetWeeklyUsagePatternsAsync(int childId, int periodWeeks = 4, CancellationToken cancellationToken = default);

    #endregion

    #region Compliance and Safety Monitoring

    /// <summary>
    /// Gets sessions that exceeded recommended duration for safety monitoring.
    /// Helps identify potentially excessive usage that may need intervention.
    /// </summary>
    /// <param name="maxDurationMinutes">Maximum recommended session duration</param>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of sessions exceeding recommended duration</returns>
    Task<IEnumerable<Session>> GetLongSessionsAsync(int maxDurationMinutes, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets COPPA-compliant usage report for a child within specified period.
    /// Provides anonymized usage data for compliance and safety purposes.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for the report</param>
    /// <param name="toDate">End date for the report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing COPPA-compliant usage metrics</returns>
    Task<Dictionary<string, object>> GetCOPPACompliantUsageReportAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion

    #region Performance and Optimization Analytics

    /// <summary>
    /// Gets sessions with performance issues for technical monitoring.
    /// Identifies sessions with crashes, slow performance, or technical problems.
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of sessions with performance issues</returns>
    Task<IEnumerable<Session>> GetSessionsWithIssuesAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets session analytics for app performance optimization.
    /// Provides insights into app stability, loading times, and user experience.
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing performance analytics</returns>
    Task<Dictionary<string, object>> GetPerformanceAnalyticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion
}