using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Infrastructure.Data.Context;
using EduPlayKids.Infrastructure.Repositories;

namespace EduPlayKids.Infrastructure;

/// <summary>
/// Unit of Work implementation for coordinating multiple repository operations.
/// Ensures data consistency across educational workflows and transaction management.
/// Essential for maintaining database integrity in complex educational operations.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly EduPlayKidsDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    // Repository instances
    private IUserRepository? _users;
    private IChildRepository? _children;
    private ISubjectRepository? _subjects;
    private IActivityRepository? _activities;
    private IActivityQuestionRepository? _activityQuestions;
    private IUserProgressRepository? _userProgress;
    private IAchievementRepository? _achievements;
    private IUserAchievementRepository? _userAchievements;
    private ISessionRepository? _sessions;
    private ISettingsRepository? _settings;
    private ISubscriptionRepository? _subscriptions;
    private IAuditLogRepository? _auditLogs;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">Logger for error handling and debugging</param>
    public UnitOfWork(EduPlayKidsDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Repository Properties

    /// <inheritdoc />
    public IUserRepository Users => _users ??= new UserRepository(_context, _logger.CreateLogger<UserRepository>());

    /// <inheritdoc />
    public IChildRepository Children => _children ??= CreateRepository<IChildRepository, ChildRepository>();

    /// <inheritdoc />
    public ISubjectRepository Subjects => _subjects ??= CreateRepository<ISubjectRepository, SubjectRepository>();

    /// <inheritdoc />
    public IActivityRepository Activities => _activities ??= CreateRepository<IActivityRepository, ActivityRepository>();

    /// <inheritdoc />
    public IActivityQuestionRepository ActivityQuestions => _activityQuestions ??= CreateRepository<IActivityQuestionRepository, ActivityQuestionRepository>();

    /// <inheritdoc />
    public IUserProgressRepository UserProgress => _userProgress ??= CreateRepository<IUserProgressRepository, UserProgressRepository>();

    /// <inheritdoc />
    public IAchievementRepository Achievements => _achievements ??= CreateRepository<IAchievementRepository, AchievementRepository>();

    /// <inheritdoc />
    public IUserAchievementRepository UserAchievements => _userAchievements ??= CreateRepository<IUserAchievementRepository, UserAchievementRepository>();

    /// <inheritdoc />
    public ISessionRepository Sessions => _sessions ??= CreateRepository<ISessionRepository, SessionRepository>();

    /// <inheritdoc />
    public ISettingsRepository Settings => _settings ??= CreateRepository<ISettingsRepository, SettingsRepository>();

    /// <inheritdoc />
    public ISubscriptionRepository Subscriptions => _subscriptions ??= CreateRepository<ISubscriptionRepository, SubscriptionRepository>();

    /// <inheritdoc />
    public IAuditLogRepository AuditLogs => _auditLogs ??= CreateRepository<IAuditLogRepository, AuditLogRepository>();

    #endregion

    #region Transaction Management

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Transaction already exists. Rolling back existing transaction before starting new one.");
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
            }

            _logger.LogDebug("Beginning new database transaction");
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Committing changes to database");

            var result = await _context.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
                _logger.LogDebug("Transaction committed successfully");
            }

            _logger.LogDebug("Changes committed successfully. {Count} entities affected", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing changes to database");
            if (_transaction != null)
            {
                await RollbackAsync(cancellationToken);
            }
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                _logger.LogDebug("Rolling back database transaction");
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
                _logger.LogDebug("Transaction rolled back successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Changes saved successfully. {Count} entities affected", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    #endregion

    #region Educational Workflow Operations

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> CompleteActivityAsync(int childId, int activityId, int starsEarned, int timeSpentMinutes, int errorsCount, int sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Completing activity {ActivityId} for child {ChildId}", activityId, childId);

            await BeginTransactionAsync(cancellationToken);

            var result = new Dictionary<string, object>();

            // Update progress
            var progress = await UserProgress.UpdateProgressAsync(childId, activityId, starsEarned, timeSpentMinutes, errorsCount, true, cancellationToken);
            result["Progress"] = progress;

            // Update session
            await Sessions.UpdateSessionProgressAsync(sessionId, 1, starsEarned, cancellationToken);

            // Check for achievements
            var eligibleAchievements = await Achievements.GetEligibleAchievementsForChildAsync(childId, cancellationToken);
            var newAchievements = new List<UserAchievement>();

            foreach (var achievement in eligibleAchievements)
            {
                var hasEarned = await UserAchievements.HasChildEarnedAchievementAsync(childId, achievement.Id, cancellationToken);
                if (!hasEarned)
                {
                    var isEligible = await Achievements.IsChildEligibleForAchievementAsync(childId, achievement.Id, cancellationToken);
                    if (isEligible)
                    {
                        var userAchievement = await UserAchievements.AwardAchievementToChildAsync(childId, achievement.Id, cancellationToken);
                        newAchievements.Add(userAchievement);
                    }
                }
            }

            result["NewAchievements"] = newAchievements;

            // Log the completion
            await LogUserActionAsync(childId, "COMPLETE_ACTIVITY", "Activity", activityId.ToString(), $"Stars: {starsEarned}, Time: {timeSpentMinutes}min, Errors: {errorsCount}", cancellationToken: cancellationToken);

            await CommitAsync(cancellationToken);

            _logger.LogDebug("Activity completion workflow completed successfully for child {ChildId}, activity {ActivityId}", childId, activityId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing activity {ActivityId} for child {ChildId}", activityId, childId);
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Child> CreateChildProfileAsync(int userId, string childName, int childAge, string? avatarImagePath = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating child profile for user {UserId}, child: {ChildName}, age: {ChildAge}", userId, childName, childAge);

            await BeginTransactionAsync(cancellationToken);

            // Create child entity
            var child = new Child
            {
                UserId = userId,
                Name = childName,
                Age = childAge,
                AvatarImagePath = avatarImagePath ?? "/images/avatars/default.png",
                FavoriteColor = "#4CAF50", // Default green
                PreferredDifficulty = childAge <= 4 ? "Easy" : childAge == 5 ? "Medium" : "Hard"
            };

            var createdChild = await Children.AddAsync(child, cancellationToken);

            // Initialize default settings for the child
            var defaultSettings = await Settings.GetDefaultSettingsTemplateAsync(cancellationToken: cancellationToken);
            foreach (var setting in defaultSettings)
            {
                await Settings.UpdateChildPersonalizationSettingsAsync(userId, createdChild.Id, new Dictionary<string, string> { { setting.Key, setting.Value } }, cancellationToken);
            }

            // Log the creation
            await LogUserActionAsync(userId, "CREATE_CHILD", "Child", createdChild.Id.ToString(), $"Name: {childName}, Age: {childAge}", cancellationToken: cancellationToken);

            await CommitAsync(cancellationToken);

            _logger.LogDebug("Child profile created successfully for user {UserId}, child ID: {ChildId}", userId, createdChild.Id);
            return createdChild;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating child profile for user {UserId}", userId);
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> UpgradeToPremiumAsync(int userId, string subscriptionType, decimal amount, string paymentMethod, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Upgrading user {UserId} to premium subscription: {SubscriptionType}", userId, subscriptionType);

            await BeginTransactionAsync(cancellationToken);

            var result = new Dictionary<string, object>();

            // Calculate expiration date based on subscription type
            var expiresAt = subscriptionType.ToLower() switch
            {
                "monthly" => DateTime.UtcNow.AddMonths(1),
                "yearly" => DateTime.UtcNow.AddYears(1),
                _ => DateTime.UtcNow.AddMonths(1)
            };

            // Create subscription
            var subscription = await Subscriptions.UpgradeToPremi​umAsync(userId, subscriptionType, amount, expiresAt, paymentMethod, cancellationToken);
            result["Subscription"] = subscription;

            // Update user premium status
            await Users.UpdatePremiumStatusAsync(userId, true, expiresAt, cancellationToken);

            // Log the upgrade
            await LogUserActionAsync(userId, "UPGRADE_PREMIUM", "Subscription", subscription.Id.ToString(), $"Type: {subscriptionType}, Amount: {amount:C}", cancellationToken: cancellationToken);

            await CommitAsync(cancellationToken);

            _logger.LogDebug("Premium upgrade completed successfully for user {UserId}", userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upgrading user {UserId} to premium", userId);
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion

    #region COPPA Compliance Operations

    /// <inheritdoc />
    public async Task LogUserActionAsync(int userId, string action, string entityType, string entityId, string? details = null, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await AuditLogs.LogUserActionAsync(userId, action, entityType, entityId, details, ipAddress, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging user action for user {UserId}", userId);
            // Don't rethrow - audit logging shouldn't break the main operation
        }
    }

    /// <inheritdoc />
    public async Task<string> ExportUserDataAsync(int userId, bool includeChildren = true, string format = "JSON", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Exporting user data for user {UserId}, format: {Format}", userId, format);

            var exportData = new Dictionary<string, object>();

            // Get user data
            var user = await Users.GetWithChildrenAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found");
            }

            exportData["User"] = user;

            if (includeChildren)
            {
                // Get children data
                var children = await Children.GetChildrenByUserAsync(userId, cancellationToken);
                exportData["Children"] = children;

                // Get progress data for all children
                var progressData = new List<object>();
                foreach (var child in children)
                {
                    var progress = await UserProgress.GetProgressByChildAsync(child.Id, cancellationToken);
                    progressData.Add(new { ChildId = child.Id, Progress = progress });
                }
                exportData["Progress"] = progressData;

                // Get achievement data for all children
                var achievementData = new List<object>();
                foreach (var child in children)
                {
                    var achievements = await UserAchievements.GetAchievementsByChildAsync(child.Id, cancellationToken);
                    achievementData.Add(new { ChildId = child.Id, Achievements = achievements });
                }
                exportData["Achievements"] = achievementData;
            }

            // Get user settings
            var settings = await Settings.GetSettingsByUserAsync(userId, cancellationToken);
            exportData["Settings"] = settings;

            // Get subscriptions
            var subscriptions = await Subscriptions.GetSubscriptionHistoryByUserAsync(userId, cancellationToken);
            exportData["Subscriptions"] = subscriptions;

            // Get sessions
            var sessions = await Sessions.GetFamilySessionsAsync(userId, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow, cancellationToken);
            exportData["Sessions"] = sessions;

            // Log the export
            await LogUserActionAsync(userId, "EXPORT_DATA", "User", userId.ToString(), $"Format: {format}, IncludeChildren: {includeChildren}", cancellationToken: cancellationToken);

            var exportJson = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            _logger.LogDebug("User data export completed for user {UserId}", userId);
            return exportJson;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteUserAccountAsync(int userId, string deletionReason, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting user account for user {UserId}, reason: {DeletionReason}", userId, deletionReason);

            await BeginTransactionAsync(cancellationToken);

            // Log the deletion request first
            await LogUserActionAsync(userId, "DELETE_ACCOUNT", "User", userId.ToString(), $"Reason: {deletionReason}", cancellationToken: cancellationToken);

            // Soft delete all children and their data
            var children = await Children.GetChildrenByUserAsync(userId, cancellationToken);
            foreach (var child in children)
            {
                // Soft delete progress
                var progress = await UserProgress.GetProgressByChildAsync(child.Id, cancellationToken);
                foreach (var p in progress)
                {
                    await UserProgress.SoftDeleteAsync(p, cancellationToken);
                }

                // Soft delete achievements
                var achievements = await UserAchievements.GetAchievementsByChildAsync(child.Id, cancellationToken);
                foreach (var a in achievements)
                {
                    await UserAchievements.SoftDeleteAsync(a, cancellationToken);
                }

                // Soft delete child
                await Children.SoftDeleteAsync(child, cancellationToken);
            }

            // Soft delete user settings
            var settings = await Settings.GetSettingsByUserAsync(userId, cancellationToken);
            foreach (var setting in settings)
            {
                await Settings.SoftDeleteAsync(setting, cancellationToken);
            }

            // Cancel active subscriptions
            var activeSubscription = await Subscriptions.GetActiveSubscriptionByUserAsync(userId, cancellationToken);
            if (activeSubscription != null)
            {
                await Subscriptions.CancelSubscriptionAsync(activeSubscription.Id, deletionReason, true, cancellationToken);
            }

            // Soft delete user
            await Users.SoftDeleteAsync(userId, cancellationToken);

            await CommitAsync(cancellationToken);

            _logger.LogDebug("User account deletion completed for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user account for user {UserId}", userId);
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion

    #region Performance and Analytics Operations

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> GetChildLearningAnalyticsAsync(int childId, int periodDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting learning analytics for child {ChildId}, period: {PeriodDays} days", childId, periodDays);

            var analytics = new Dictionary<string, object>();

            // Get progress analytics
            var progressAnalytics = await UserProgress.GetLearningAnalyticsSummaryAsync(childId, periodDays, cancellationToken);
            analytics["Progress"] = progressAnalytics;

            // Get achievement statistics
            var achievementCount = await UserAchievements.GetAchievementCountByChildAsync(childId, cancellationToken);
            var achievementsByCategory = await UserAchievements.GetAchievementCountsByCategoryAsync(childId, cancellationToken);
            analytics["Achievements"] = new { Count = achievementCount, ByCategory = achievementsByCategory };

            // Get session statistics
            var sessionStats = await Sessions.GetLearningSessionStatisticsAsync(childId, periodDays, cancellationToken);
            analytics["Sessions"] = sessionStats;

            // Get subject progress
            var subjectsWithProgress = await Subjects.GetSubjectsWithProgressByChildAsync(childId, cancellationToken);
            analytics["SubjectProgress"] = subjectsWithProgress;

            _logger.LogDebug("Learning analytics retrieved for child {ChildId}", childId);
            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting learning analytics for child {ChildId}", childId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> GetFamilyDashboardDataAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting family dashboard data for user {UserId}", userId);

            var dashboardData = new Dictionary<string, object>();

            // Get user with children
            var user = await Users.GetWithChildrenAsync(userId, cancellationToken);
            dashboardData["User"] = user;

            // Get family achievements
            var familyAchievements = await UserAchievements.GetFamilyAchievementsAsync(userId, cancellationToken);
            var familyAchievementStats = await UserAchievements.GetFamilyAchievementStatisticsAsync(userId, cancellationToken);
            dashboardData["FamilyAchievements"] = new { Recent = familyAchievements, Statistics = familyAchievementStats };

            // Get family usage statistics
            var familyUsageStats = await Sessions.GetFamilyUsageStatisticsAsync(userId, cancellationToken: cancellationToken);
            dashboardData["UsageStatistics"] = familyUsageStats;

            // Get subscription status
            var subscriptionStatus = await Subscriptions.GetSubscriptionStatusAsync(userId, cancellationToken);
            dashboardData["SubscriptionStatus"] = subscriptionStatus;

            _logger.LogDebug("Family dashboard data retrieved for user {UserId}", userId);
            return dashboardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting family dashboard data for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> PerformMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Performing database maintenance operations");

            var results = new Dictionary<string, object>();

            // Archive old audit logs (older than 1 year)
            var archiveDate = DateTime.UtcNow.AddYears(-1);
            var archivedLogs = await AuditLogs.ArchiveOldLogsAsync(archiveDate, cancellationToken);
            results["ArchivedLogs"] = archivedLogs;

            // Purge very old archived logs (older than 7 years for COPPA compliance)
            var purgeDate = DateTime.UtcNow.AddYears(-7);
            var purgedLogs = await AuditLogs.PurgeArchivedLogsAsync(purgeDate, cancellationToken);
            results["PurgedLogs"] = purgedLogs;

            // Get storage statistics
            var storageStats = await AuditLogs.GetLogStorageStatisticsAsync(cancellationToken);
            results["StorageStatistics"] = storageStats;

            await SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Database maintenance completed successfully");
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing database maintenance");
            throw;
        }
    }

    #endregion

    #region Health Check and Monitoring

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> PerformHealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Performing system health check");

            var healthResults = new Dictionary<string, object>();

            // Check database connectivity
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            healthResults["DatabaseConnectivity"] = canConnect;

            // Check basic repository operations
            try
            {
                var userCount = await Users.GetCountAsync(cancellationToken);
                healthResults["UserRepository"] = new { Status = "Healthy", UserCount = userCount };
            }
            catch (Exception ex)
            {
                healthResults["UserRepository"] = new { Status = "Unhealthy", Error = ex.Message };
            }

            // Add more health checks for other repositories as needed

            _logger.LogDebug("System health check completed");
            return healthResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing system health check");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, object>> GetSystemPerformanceMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting system performance metrics");

            var metrics = new Dictionary<string, object>();

            // Get entity counts
            metrics["EntityCounts"] = new
            {
                Users = await Users.GetCountAsync(cancellationToken),
                Children = await Children.GetCountAsync(cancellationToken),
                Activities = await Activities.GetCountAsync(cancellationToken),
                Progress = await UserProgress.GetCountAsync(cancellationToken),
                Sessions = await Sessions.GetCountAsync(cancellationToken)
            };

            // Get recent activity
            var recentUsers = await Users.GetNewUsersInPeriodAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, cancellationToken);
            metrics["RecentActivity"] = new
            {
                NewUsersThisWeek = recentUsers.Count(),
                // Add more recent activity metrics
            };

            _logger.LogDebug("System performance metrics retrieved");
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system performance metrics");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Creates a repository instance with proper logger injection.
    /// </summary>
    /// <typeparam name="TInterface">The repository interface type</typeparam>
    /// <typeparam name="TImplementation">The repository implementation type</typeparam>
    /// <returns>The repository instance</returns>
    private TInterface CreateRepository<TInterface, TImplementation>()
        where TImplementation : class, TInterface
    {
        var logger = _logger.CreateLogger<TImplementation>();
        return (TInterface)Activator.CreateInstance(typeof(TImplementation), _context, logger)!;
    }

    #endregion

    #region Dispose Pattern

    /// <summary>
    /// Disposes the unit of work and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method for proper resource cleanup.
    /// </summary>
    /// <param name="disposing">Whether the method is being called from Dispose()</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing UnitOfWork resources");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    #endregion
}