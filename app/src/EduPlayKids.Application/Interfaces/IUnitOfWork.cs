using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating multiple repository operations.
/// Ensures data consistency across educational workflows and transaction management.
/// Essential for maintaining database integrity in complex educational operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    #region Repository Properties

    /// <summary>
    /// Gets the User repository for parent/guardian operations.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Gets the Child repository for child profile and progress operations.
    /// </summary>
    IChildRepository Children { get; }

    /// <summary>
    /// Gets the Subject repository for educational content organization.
    /// </summary>
    ISubjectRepository Subjects { get; }

    /// <summary>
    /// Gets the Activity repository for learning activity management.
    /// </summary>
    IActivityRepository Activities { get; }

    /// <summary>
    /// Gets the ActivityQuestion repository for interactive content.
    /// </summary>
    IActivityQuestionRepository ActivityQuestions { get; }

    /// <summary>
    /// Gets the UserProgress repository for learning analytics and progress tracking.
    /// </summary>
    IUserProgressRepository UserProgress { get; }

    /// <summary>
    /// Gets the Achievement repository for gamification and recognition systems.
    /// </summary>
    IAchievementRepository Achievements { get; }

    /// <summary>
    /// Gets the UserAchievement repository for earned recognition tracking.
    /// </summary>
    IUserAchievementRepository UserAchievements { get; }

    /// <summary>
    /// Gets the Session repository for usage analytics and screen time monitoring.
    /// </summary>
    ISessionRepository Sessions { get; }

    /// <summary>
    /// Gets the Settings repository for user preferences and configuration.
    /// </summary>
    ISettingsRepository Settings { get; }

    /// <summary>
    /// Gets the Subscription repository for freemium model and billing management.
    /// </summary>
    ISubscriptionRepository Subscriptions { get; }

    /// <summary>
    /// Gets the AuditLog repository for COPPA compliance and security monitoring.
    /// </summary>
    IAuditLogRepository AuditLogs { get; }

    /// <summary>
    /// Gets the ParentalPin repository for secure parental controls access.
    /// </summary>
    IParentalPinRepository ParentalPins { get; }

    #endregion

    #region Transaction Management

    /// <summary>
    /// Begins a new database transaction for coordinated operations.
    /// Ensures all operations within the transaction are atomic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction and saves all changes to the database.
    /// Ensures all pending changes are persisted atomically.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction and discards all pending changes.
    /// Used when operations fail and data consistency must be maintained.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database without transaction management.
    /// Used for simple operations that don't require transaction coordination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Educational Workflow Operations

    /// <summary>
    /// Completes an activity for a child with full progress tracking and achievement checking.
    /// Coordinates progress updates, achievement validation, and session tracking.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="starsEarned">Number of stars earned (1-3)</param>
    /// <param name="timeSpentMinutes">Time spent on the activity</param>
    /// <param name="errorsCount">Number of errors made</param>
    /// <param name="sessionId">The current session identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing completion results and any earned achievements</returns>
    Task<Dictionary<string, object>> CompleteActivityAsync(int childId, int activityId, int starsEarned, int timeSpentMinutes, int errorsCount, int sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new child profile with default settings and initial progress setup.
    /// Coordinates child creation, default settings, and initial data seeding.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="childName">The child's name</param>
    /// <param name="childAge">The child's age</param>
    /// <param name="avatarImagePath">Optional avatar image path</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created child with initialized data</returns>
    Task<Child> CreateChildProfileAsync(int userId, string childName, int childAge, string? avatarImagePath = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upgrades a user from trial to premium with subscription and feature activation.
    /// Coordinates subscription creation, user status update, and feature unlocking.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="subscriptionType">The premium subscription type</param>
    /// <param name="amount">The subscription amount</param>
    /// <param name="paymentMethod">The payment method information</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing upgrade results and subscription details</returns>
    Task<Dictionary<string, object>> UpgradeToPremiumAsync(int userId, string subscriptionType, decimal amount, string paymentMethod, CancellationToken cancellationToken = default);

    #endregion

    #region COPPA Compliance Operations

    /// <summary>
    /// Logs user action with COPPA compliance requirements.
    /// Ensures all data access and modifications are properly audited.
    /// </summary>
    /// <param name="userId">The user performing the action</param>
    /// <param name="action">The action being performed</param>
    /// <param name="entityType">The type of entity being affected</param>
    /// <param name="entityId">The identifier of the affected entity</param>
    /// <param name="details">Additional details about the action</param>
    /// <param name="ipAddress">The IP address of the user</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task LogUserActionAsync(int userId, string action, string entityType, string entityId, string? details = null, string? ipAddress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports user data for COPPA compliance and data portability requirements.
    /// Coordinates data collection from multiple repositories for complete export.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="includeChildren">Whether to include children's data</param>
    /// <param name="format">Export format (JSON, CSV, XML)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Exported user data in the specified format</returns>
    Task<string> ExportUserDataAsync(int userId, bool includeChildren = true, string format = "JSON", CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes user account and all associated data for COPPA compliance.
    /// Coordinates complete data removal across all repositories while maintaining audit trail.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="deletionReason">The reason for account deletion</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task DeleteUserAccountAsync(int userId, string deletionReason, CancellationToken cancellationToken = default);

    #endregion

    #region Performance and Analytics Operations

    /// <summary>
    /// Gets comprehensive learning analytics for a child across all educational activities.
    /// Coordinates data collection from multiple repositories for complete analysis.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing comprehensive learning analytics</returns>
    Task<Dictionary<string, object>> GetChildLearningAnalyticsAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets family dashboard data with all children's progress and statistics.
    /// Coordinates data from multiple repositories for parental overview.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing family dashboard data</returns>
    Task<Dictionary<string, object>> GetFamilyDashboardDataAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs database maintenance operations including cleanup and optimization.
    /// Coordinates maintenance tasks across all repositories for optimal performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing maintenance operation results</returns>
    Task<Dictionary<string, object>> PerformMaintenanceAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Health Check and Monitoring

    /// <summary>
    /// Checks the health of all repositories and database connections.
    /// Ensures system reliability and identifies potential issues.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing health check results for all components</returns>
    Task<Dictionary<string, object>> PerformHealthCheckAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets system performance metrics across all repositories.
    /// Provides insights into database performance and usage patterns.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing performance metrics and statistics</returns>
    Task<Dictionary<string, object>> GetSystemPerformanceMetricsAsync(CancellationToken cancellationToken = default);

    #endregion
}