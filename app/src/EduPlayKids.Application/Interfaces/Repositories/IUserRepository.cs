using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations.
/// Handles parent/guardian authentication, profile management, and premium subscription features.
/// Implements COPPA-compliant parent verification and child safety requirements.
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    #region Authentication and Security

    /// <summary>
    /// Validates user credentials for login authentication.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="passwordHash">Hashed password for verification</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The authenticated user if credentials are valid, null otherwise</returns>
    Task<User?> ValidateCredentialsAsync(string email, string passwordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the parental PIN for accessing parent dashboard and premium features.
    /// Essential for child safety and parental control compliance.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="pin">The 4-digit PIN to validate</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if PIN is correct, false otherwise</returns>
    Task<bool> ValidateParentalPinAsync(int userId, string pin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address for authentication and account recovery.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's last login timestamp for activity tracking.
    /// Important for security monitoring and user engagement analytics.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="loginTime">The timestamp of the login</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateLastLoginAsync(int userId, DateTime loginTime, CancellationToken cancellationToken = default);

    #endregion

    #region Multi-Child Family Management

    /// <summary>
    /// Gets a user with all their associated children for family management.
    /// Optimized for loading family profiles efficiently.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user with loaded children collection</returns>
    Task<User?> GetWithChildrenAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who have children within a specific age range.
    /// Useful for targeted educational content and parental communication.
    /// </summary>
    /// <param name="minAge">Minimum child age</param>
    /// <param name="maxAge">Maximum child age</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with children in the specified age range</returns>
    Task<IEnumerable<User>> GetUsersWithChildrenInAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of children associated with a user.
    /// Used for family size analytics and subscription management.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The number of children associated with the user</returns>
    Task<int> GetChildrenCountAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Premium Subscription Management

    /// <summary>
    /// Gets all users who are currently in their free trial period.
    /// Used for conversion tracking and trial expiration notifications.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users in free trial</returns>
    Task<IEnumerable<User>> GetUsersInFreeTrialAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users whose free trial expires within a specified number of days.
    /// Critical for conversion campaigns and trial extension notifications.
    /// </summary>
    /// <param name="daysFromNow">Number of days from current date</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with expiring trials</returns>
    Task<IEnumerable<User>> GetUsersWithTrialExpiringAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with active premium subscriptions.
    /// Used for premium feature access control and revenue analytics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of premium users</returns>
    Task<IEnumerable<User>> GetPremiumUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users whose premium subscription expires within a specified number of days.
    /// Important for renewal campaigns and subscription management.
    /// </summary>
    /// <param name="daysFromNow">Number of days from current date</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with expiring subscriptions</returns>
    Task<IEnumerable<User>> GetUsersWithSubscriptionExpiringAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's premium subscription status and expiration date.
    /// Used when users purchase or renew their subscription.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="isPremium">The new premium status</param>
    /// <param name="expiresAt">The subscription expiration date (null for unlimited)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdatePremiumStatusAsync(int userId, bool isPremium, DateTime? expiresAt, CancellationToken cancellationToken = default);

    #endregion

    #region Parental Controls and Safety

    /// <summary>
    /// Gets users with parental controls enabled for safety compliance reporting.
    /// Essential for COPPA compliance and child safety monitoring.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with parental controls enabled</returns>
    Task<IEnumerable<User>> GetUsersWithParentalControlsEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's daily usage limit for all their children.
    /// Supports healthy screen time management and parental control features.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="dailyLimitMinutes">The new daily usage limit in minutes</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateDailyUsageLimitAsync(int userId, int dailyLimitMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's parental PIN for security purposes.
    /// Important for maintaining secure access to parental controls.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="newPin">The new 4-digit PIN</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateParentalPinAsync(int userId, string newPin, CancellationToken cancellationToken = default);

    #endregion

    #region Localization and Regional Support

    /// <summary>
    /// Gets users by their preferred language for localized communication.
    /// Supports bilingual Spanish/English user base.
    /// </summary>
    /// <param name="language">The language code (e.g., "es", "en")</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with the specified language preference</returns>
    Task<IEnumerable<User>> GetUsersByLanguageAsync(string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by their country code for regional analytics and curriculum alignment.
    /// Important for understanding geographic distribution and compliance requirements.
    /// </summary>
    /// <param name="countryCode">The country code (e.g., "US", "MX", "CO")</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users from the specified country</returns>
    Task<IEnumerable<User>> GetUsersByCountryAsync(string countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's language preference and regional settings.
    /// Affects app interface, audio instructions, and content localization.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="language">The new language preference</param>
    /// <param name="countryCode">The new country code (optional)</param>
    /// <param name="timeZone">The new timezone (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateLocalizationSettingsAsync(int userId, string language, string? countryCode = null, string? timeZone = null, CancellationToken cancellationToken = default);

    #endregion

    #region Analytics and Reporting

    /// <summary>
    /// Gets user registration statistics for a specified date range.
    /// Used for growth analytics and user acquisition reporting.
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users registered within the date range</returns>
    Task<IEnumerable<User>> GetNewUsersInPeriodAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who haven't logged in within a specified number of days.
    /// Used for re-engagement campaigns and user retention analysis.
    /// </summary>
    /// <param name="daysInactive">Number of days of inactivity</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of inactive users</returns>
    Task<IEnumerable<User>> GetInactiveUsersAsync(int daysInactive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets usage statistics for premium vs free users.
    /// Important for conversion optimization and feature development.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with usage statistics by user type</returns>
    Task<Dictionary<string, int>> GetUserTypeStatisticsAsync(CancellationToken cancellationToken = default);

    #endregion
}