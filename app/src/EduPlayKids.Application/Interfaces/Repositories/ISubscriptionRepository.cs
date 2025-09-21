using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Subscription entity operations.
/// Manages freemium model, premium subscriptions, and billing information.
/// Supports trial periods, subscription lifecycle, and revenue analytics.
/// </summary>
public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    #region User Subscription Management

    /// <summary>
    /// Gets the active subscription for a specific user.
    /// Essential for determining premium access and feature availability.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The active subscription if exists, null otherwise</returns>
    Task<Subscription?> GetActiveSubscriptionByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subscription history for a user for billing and support purposes.
    /// Includes past subscriptions, renewals, and cancellations.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all subscriptions for the user</returns>
    Task<IEnumerable<Subscription>> GetSubscriptionHistoryByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new subscription for a user with proper validation.
    /// Central method for subscription activation and premium access granting.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="subscriptionType">The type of subscription (Monthly, Yearly, etc.)</param>
    /// <param name="amount">The subscription amount</param>
    /// <param name="expiresAt">The subscription expiration date</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created subscription</returns>
    Task<Subscription> CreateSubscriptionAsync(int userId, string subscriptionType, decimal amount, DateTime expiresAt, CancellationToken cancellationToken = default);

    #endregion

    #region Free Trial Management

    /// <summary>
    /// Gets users currently in their free trial period.
    /// Used for trial expiration notifications and conversion tracking.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of users with active free trials</returns>
    Task<IEnumerable<Subscription>> GetActiveTrialSubscriptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users whose free trial expires within a specified number of days.
    /// Critical for conversion campaigns and trial extension offers.
    /// </summary>
    /// <param name="daysFromNow">Number of days from current date</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subscriptions with expiring trials</returns>
    Task<IEnumerable<Subscription>> GetTrialsExpiringInDaysAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extends a user's free trial period for retention or support purposes.
    /// Used for customer service and retention campaign initiatives.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="extensionDays">Number of days to extend the trial</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated subscription with extended trial</returns>
    Task<Subscription?> ExtendTrialPeriodAsync(int userId, int extensionDays, CancellationToken cancellationToken = default);

    #endregion

    #region Premium Subscription Management

    /// <summary>
    /// Gets all active premium subscriptions for revenue and analytics tracking.
    /// Used for understanding customer base and subscription health metrics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of active premium subscriptions</returns>
    Task<IEnumerable<Subscription>> GetActivePremiumSubscriptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets premium subscriptions expiring within a specified number of days.
    /// Important for renewal campaigns and churn prevention efforts.
    /// </summary>
    /// <param name="daysFromNow">Number of days from current date</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subscriptions expiring soon</returns>
    Task<IEnumerable<Subscription>> GetPremiumSubscriptionsExpiringInDaysAsync(int daysFromNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upgrades a trial subscription to premium with billing information.
    /// Core conversion method for trial-to-paid user transitions.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="subscriptionType">The premium subscription type</param>
    /// <param name="amount">The subscription amount</param>
    /// <param name="expiresAt">The subscription expiration date</param>
    /// <param name="paymentMethod">The payment method information</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The upgraded premium subscription</returns>
    Task<Subscription> UpgradeToPremi​umAsync(int userId, string subscriptionType, decimal amount, DateTime expiresAt, string paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renews an existing premium subscription with updated terms.
    /// Handles automatic renewals and manual subscription extensions.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="newExpirationDate">The new expiration date</param>
    /// <param name="renewalAmount">The renewal amount</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The renewed subscription</returns>
    Task<Subscription> RenewSubscriptionAsync(int subscriptionId, DateTime newExpirationDate, decimal renewalAmount, CancellationToken cancellationToken = default);

    #endregion

    #region Subscription Status and Validation

    /// <summary>
    /// Checks if a user has active premium access (trial or paid).
    /// Central method for premium feature access control throughout the app.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if user has premium access, false otherwise</returns>
    Task<bool> HasActivePremiumAccessAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the subscription status for a user with detailed information.
    /// Provides comprehensive subscription state for UI and business logic.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing detailed subscription status</returns>
    Task<Dictionary<string, object>> GetSubscriptionStatusAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates subscription access for specific premium features.
    /// Ensures feature access control based on subscription level and status.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="featureName">The premium feature to validate</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if user has access to the feature, false otherwise</returns>
    Task<bool> ValidateFeatureAccessAsync(int userId, string featureName, CancellationToken cancellationToken = default);

    #endregion

    #region Cancellation and Refund Management

    /// <summary>
    /// Cancels an active subscription with proper status updates.
    /// Handles immediate and end-of-period cancellations based on policy.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="cancellationReason">The reason for cancellation</param>
    /// <param name="immediateTermination">Whether to terminate immediately or at period end</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The cancelled subscription with updated status</returns>
    Task<Subscription> CancelSubscriptionAsync(int subscriptionId, string cancellationReason, bool immediateTermination = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cancelled subscriptions for analytics and churn analysis.
    /// Used for understanding cancellation patterns and retention opportunities.
    /// </summary>
    /// <param name="fromDate">Start date for cancellation period</param>
    /// <param name="toDate">End date for cancellation period</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of cancelled subscriptions in the period</returns>
    Task<IEnumerable<Subscription>> GetCancelledSubscriptionsInPeriodAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes refund requests with proper validation and documentation.
    /// Handles full and partial refunds based on business policies.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="refundAmount">The amount to refund</param>
    /// <param name="refundReason">The reason for the refund</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The subscription with updated refund status</returns>
    Task<Subscription> ProcessRefundAsync(int subscriptionId, decimal refundAmount, string refundReason, CancellationToken cancellationToken = default);

    #endregion

    #region Revenue Analytics and Reporting

    /// <summary>
    /// Gets subscription revenue analytics for business intelligence.
    /// Provides comprehensive financial metrics and subscription health data.
    /// </summary>
    /// <param name="fromDate">Start date for analytics period</param>
    /// <param name="toDate">End date for analytics period</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing revenue analytics and metrics</returns>
    Task<Dictionary<string, object>> GetRevenueAnalyticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subscription conversion metrics from trial to premium.
    /// Critical for understanding conversion rates and optimization opportunities.
    /// </summary>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing conversion metrics and rates</returns>
    Task<Dictionary<string, object>> GetConversionMetricsAsync(int periodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets churn analysis for subscription retention insights.
    /// Provides data on cancellation patterns and customer lifetime value.
    /// </summary>
    /// <param name="periodDays">Analysis period in days</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing churn analysis and retention metrics</returns>
    Task<Dictionary<string, object>> GetChurnAnalysisAsync(int periodDays = 90, CancellationToken cancellationToken = default);

    #endregion

    #region Subscription Types and Pricing

    /// <summary>
    /// Gets available subscription types with pricing information.
    /// Used for presenting subscription options to users.
    /// </summary>
    /// <param name="countryCode">Country code for localized pricing</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of available subscription plans</returns>
    Task<Dictionary<string, object>> GetAvailableSubscriptionPlansAsync(string countryCode = "US", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subscription statistics by type for business analysis.
    /// Shows popularity and performance of different subscription tiers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with subscription type statistics</returns>
    Task<Dictionary<string, int>> GetSubscriptionTypeStatisticsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Payment and Billing Integration

    /// <summary>
    /// Updates payment method for an active subscription.
    /// Handles payment method changes for existing subscribers.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="newPaymentMethod">The new payment method information</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The subscription with updated payment information</returns>
    Task<Subscription> UpdatePaymentMethodAsync(int subscriptionId, string newPaymentMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subscriptions with failed payment attempts for retry processing.
    /// Important for handling payment failures and preventing service interruption.
    /// </summary>
    /// <param name="maxAttempts">Maximum payment attempts before considering failed</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subscriptions with payment issues</returns>
    Task<IEnumerable<Subscription>> GetSubscriptionsWithPaymentIssuesAsync(int maxAttempts = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records payment attempt for subscription billing tracking.
    /// Maintains payment history for billing reconciliation and support.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="paymentStatus">The payment attempt status</param>
    /// <param name="transactionId">The transaction identifier</param>
    /// <param name="amount">The payment amount</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task RecordPaymentAttemptAsync(int subscriptionId, string paymentStatus, string transactionId, decimal amount, CancellationToken cancellationToken = default);

    #endregion
}