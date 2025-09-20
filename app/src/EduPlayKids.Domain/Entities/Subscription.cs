using System.ComponentModel.DataAnnotations;
using EduPlayKids.Domain.Common;

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Represents a premium subscription for the EduPlayKids freemium model.
/// Manages subscription lifecycle, billing information, and feature access control.
/// Core entity for monetization strategy: 3-day free trial, then $4.99 premium access.
/// </summary>
public class Subscription : AuditableEntity
{
    /// <summary>
    /// Gets or sets the foreign key to the user who owns this subscription.
    /// Links subscription to the parent/guardian account.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the subscription plan identifier.
    /// Current plans: "free_trial", "premium_monthly", "premium_annual"
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PlanType { get; set; } = "free_trial";

    /// <summary>
    /// Gets or sets the current status of the subscription.
    /// Values: Active, Expired, Cancelled, Suspended, Trial, Payment_Failed
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Trial";

    /// <summary>
    /// Gets or sets the date when the subscription started.
    /// Set when user first registers (beginning of free trial).
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the subscription ends or renews.
    /// For trials: 3 days from start. For premium: monthly/annual renewal.
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the free trial expires.
    /// All users get 3 days of full access upon registration.
    /// </summary>
    [Required]
    public DateTime TrialEndDate { get; set; }

    /// <summary>
    /// Gets or sets the subscription price in cents.
    /// Stored in cents to avoid decimal precision issues.
    /// Example: $4.99 = 499 cents
    /// </summary>
    public int PriceCents { get; set; } = 0;

    /// <summary>
    /// Gets or sets the currency code for the subscription price.
    /// ISO 4217 currency codes (USD, MXN, EUR, etc.).
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the billing cycle for recurring subscriptions.
    /// Values: Monthly, Annual, One_Time
    /// </summary>
    [StringLength(20)]
    public string BillingCycle { get; set; } = "Monthly";

    /// <summary>
    /// Gets or sets the date of the next billing cycle.
    /// When the next payment will be processed.
    /// </summary>
    public DateTime? NextBillingDate { get; set; }

    /// <summary>
    /// Gets or sets the date of the last successful payment.
    /// Used for billing history and payment tracking.
    /// </summary>
    public DateTime? LastPaymentDate { get; set; }

    /// <summary>
    /// Gets or sets the external payment provider subscription ID.
    /// Reference to App Store, Google Play, or payment processor subscription.
    /// </summary>
    [StringLength(255)]
    public string? ExternalSubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the payment provider used for this subscription.
    /// Values: Apple_App_Store, Google_Play, Stripe, PayPal, etc.
    /// </summary>
    [StringLength(50)]
    public string? PaymentProvider { get; set; }

    /// <summary>
    /// Gets or sets the external transaction ID for the last payment.
    /// Reference for payment verification and dispute resolution.
    /// </summary>
    [StringLength(255)]
    public string? LastTransactionId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether auto-renewal is enabled.
    /// Controls automatic subscription renewal for recurring plans.
    /// </summary>
    public bool AutoRenewalEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the subscription was cancelled.
    /// Null if subscription hasn't been cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for subscription cancellation.
    /// Values: User_Request, Payment_Failed, Billing_Issue, Policy_Violation, etc.
    /// </summary>
    [StringLength(50)]
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets or sets the number of grace period days for failed payments.
    /// Number of days to maintain access after payment failure.
    /// </summary>
    public int GracePeriodDays { get; set; } = 3;

    /// <summary>
    /// Gets or sets the date when grace period ends.
    /// Final date of access after payment failure.
    /// </summary>
    public DateTime? GracePeriodEndDate { get; set; }

    /// <summary>
    /// Gets or sets the number of payment retry attempts.
    /// How many times payment has been retried after failure.
    /// </summary>
    public int PaymentRetryAttempts { get; set; } = 0;

    /// <summary>
    /// Gets or sets the date of the last payment retry attempt.
    /// Used for retry scheduling and failure tracking.
    /// </summary>
    public DateTime? LastPaymentRetryDate { get; set; }

    /// <summary>
    /// Gets or sets promotional information as JSON.
    /// Details about discounts, coupons, or special offers applied:
    /// - Promo code used
    /// - Discount percentage or amount
    /// - Promotion expiry date
    /// - Campaign tracking information
    /// </summary>
    public string? PromotionalData { get; set; }

    /// <summary>
    /// Gets or sets features included in this subscription as JSON.
    /// Flexible structure defining access rights:
    /// - Unlimited activities access
    /// - Crown challenges enabled
    /// - Progress reports frequency
    /// - Multi-child support
    /// - Offline content downloads
    /// </summary>
    public string? FeatureAccess { get; set; }

    /// <summary>
    /// Gets or sets usage limits for this subscription as JSON.
    /// Defines restrictions for free users:
    /// - Daily activity limit
    /// - Subject access restrictions
    /// - Feature limitations
    /// - Content availability windows
    /// </summary>
    public string? UsageLimits { get; set; }

    /// <summary>
    /// Gets or sets subscription metadata as JSON.
    /// Additional tracking and configuration data:
    /// - Acquisition source
    /// - Marketing attribution
    /// - User behavior insights
    /// - Support interaction history
    /// </summary>
    public new string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this subscription should be synced.
    /// Used for cross-platform subscription verification.
    /// </summary>
    public bool NeedsSync { get; set; } = false;

    /// <summary>
    /// Gets or sets the subscription version for schema evolution.
    /// Supports future subscription model changes.
    /// </summary>
    public int SubscriptionVersion { get; set; } = 1;

    /// <summary>
    /// Navigation property: The user who owns this subscription.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the Subscription class.
    /// Sets default values for new subscription with 3-day free trial.
    /// </summary>
    public Subscription()
    {
        PlanType = "free_trial";
        Status = "Trial";
        StartDate = DateTime.UtcNow;
        TrialEndDate = DateTime.UtcNow.AddDays(3);
        EndDate = TrialEndDate;
        PriceCents = 0;
        Currency = "USD";
        BillingCycle = "Monthly";
        AutoRenewalEnabled = true;
        GracePeriodDays = 3;
        PaymentRetryAttempts = 0;
        NeedsSync = false;
        SubscriptionVersion = 1;
    }

    /// <summary>
    /// Determines if the subscription is currently active and provides premium access.
    /// </summary>
    /// <returns>True if user has premium access, false otherwise.</returns>
    public bool IsActive()
    {
        var now = DateTime.UtcNow;

        return Status switch
        {
            "Active" => now <= EndDate,
            "Trial" => now <= TrialEndDate,
            "Payment_Failed" when GracePeriodEndDate.HasValue => now <= GracePeriodEndDate.Value,
            _ => false
        };
    }

    /// <summary>
    /// Determines if the user is currently in the free trial period.
    /// </summary>
    /// <returns>True if in trial period, false otherwise.</returns>
    public bool IsInTrial()
    {
        return Status == "Trial" && DateTime.UtcNow <= TrialEndDate;
    }

    /// <summary>
    /// Determines if the subscription has expired.
    /// </summary>
    /// <returns>True if subscription has expired, false otherwise.</returns>
    public bool IsExpired()
    {
        var now = DateTime.UtcNow;

        return Status switch
        {
            "Expired" => true,
            "Cancelled" => CancelledAt.HasValue && now > EndDate,
            "Active" => now > EndDate,
            "Trial" => now > TrialEndDate,
            "Payment_Failed" when GracePeriodEndDate.HasValue => now > GracePeriodEndDate.Value,
            "Payment_Failed" => now > EndDate.AddDays(GracePeriodDays),
            _ => false
        };
    }

    /// <summary>
    /// Gets the number of days remaining in the current subscription period.
    /// </summary>
    /// <returns>Days remaining, or 0 if expired.</returns>
    public int GetDaysRemaining()
    {
        if (IsExpired()) return 0;

        var expiryDate = Status == "Trial" ? TrialEndDate : EndDate;
        var remaining = (expiryDate - DateTime.UtcNow).Days;

        return Math.Max(0, remaining);
    }

    /// <summary>
    /// Upgrades the subscription from trial to premium.
    /// </summary>
    /// <param name="planType">The premium plan type (e.g., "premium_monthly").</param>
    /// <param name="priceCents">The subscription price in cents.</param>
    /// <param name="billingCycle">The billing cycle (Monthly/Annual).</param>
    /// <param name="paymentProvider">The payment provider used.</param>
    /// <param name="externalSubscriptionId">The external subscription ID.</param>
    public void UpgradeToPremium(string planType, int priceCents, string billingCycle,
        string paymentProvider, string? externalSubscriptionId = null)
    {
        PlanType = planType;
        Status = "Active";
        PriceCents = priceCents;
        BillingCycle = billingCycle;
        PaymentProvider = paymentProvider;
        ExternalSubscriptionId = externalSubscriptionId;
        LastPaymentDate = DateTime.UtcNow;

        // Set end date based on billing cycle
        EndDate = billingCycle switch
        {
            "Monthly" => DateTime.UtcNow.AddMonths(1),
            "Annual" => DateTime.UtcNow.AddYears(1),
            _ => DateTime.UtcNow.AddMonths(1)
        };

        NextBillingDate = EndDate;
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Renews the subscription for another billing cycle.
    /// </summary>
    /// <param name="transactionId">The transaction ID for the renewal payment.</param>
    public void RenewSubscription(string? transactionId = null)
    {
        if (Status != "Active") return;

        LastPaymentDate = DateTime.UtcNow;
        LastTransactionId = transactionId;
        PaymentRetryAttempts = 0;
        GracePeriodEndDate = null;

        // Extend end date based on billing cycle
        EndDate = BillingCycle switch
        {
            "Monthly" => EndDate.AddMonths(1),
            "Annual" => EndDate.AddYears(1),
            _ => EndDate.AddMonths(1)
        };

        NextBillingDate = EndDate;
        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Cancels the subscription.
    /// </summary>
    /// <param name="reason">The reason for cancellation.</param>
    /// <param name="immediate">Whether to cancel immediately or at end of billing period.</param>
    public void CancelSubscription(string reason, bool immediate = false)
    {
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        AutoRenewalEnabled = false;

        if (immediate)
        {
            Status = "Cancelled";
            EndDate = DateTime.UtcNow;
        }
        else
        {
            // Let subscription continue until end of current billing period
            Status = "Active"; // Will become "Expired" when EndDate passes
        }

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Handles payment failure for the subscription.
    /// </summary>
    /// <param name="retryInDays">Number of days until next retry attempt.</param>
    public void HandlePaymentFailure(int retryInDays = 3)
    {
        Status = "Payment_Failed";
        PaymentRetryAttempts++;
        LastPaymentRetryDate = DateTime.UtcNow;
        GracePeriodEndDate = DateTime.UtcNow.AddDays(GracePeriodDays);

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Restores the subscription after resolving payment issues.
    /// </summary>
    /// <param name="transactionId">The successful transaction ID.</param>
    public void RestoreSubscription(string? transactionId = null)
    {
        if (Status != "Payment_Failed") return;

        Status = "Active";
        LastPaymentDate = DateTime.UtcNow;
        LastTransactionId = transactionId;
        PaymentRetryAttempts = 0;
        GracePeriodEndDate = null;

        UpdatedAt = DateTime.UtcNow;
        NeedsSync = true;
    }

    /// <summary>
    /// Gets the effective daily activity limit based on subscription status.
    /// </summary>
    /// <returns>Daily activity limit, or null for unlimited access.</returns>
    public int? GetDailyActivityLimit()
    {
        if (IsActive())
        {
            // Premium users have unlimited access
            return null;
        }

        // Free users have limited access after trial
        return 10; // Configurable limit for free users
    }

    /// <summary>
    /// Determines if crown challenges are available for this subscription.
    /// </summary>
    /// <returns>True if crown challenges are available.</returns>
    public bool AreCrownChallengesAvailable()
    {
        return IsActive(); // Premium feature
    }

    /// <summary>
    /// Gets the subscription price in dollars (formatted).
    /// </summary>
    /// <returns>Formatted price string (e.g., "$4.99").</returns>
    public string GetFormattedPrice()
    {
        if (PriceCents == 0) return "Free";

        var price = PriceCents / 100.0;
        return Currency switch
        {
            "USD" => $"${price:F2}",
            "EUR" => $"€{price:F2}",
            "MXN" => $"${price:F2} MXN",
            _ => $"{price:F2} {Currency}"
        };
    }

    /// <summary>
    /// Gets a comprehensive subscription summary for display.
    /// </summary>
    /// <returns>Subscription summary object.</returns>
    public object GetSubscriptionSummary()
    {
        return new
        {
            PlanType,
            Status,
            IsActive = IsActive(),
            IsInTrial = IsInTrial(),
            IsExpired = IsExpired(),
            DaysRemaining = GetDaysRemaining(),
            FormattedPrice = GetFormattedPrice(),
            BillingCycle,
            TrialEndDate,
            EndDate,
            NextBillingDate,
            AutoRenewalEnabled,
            CancelledAt,
            CancellationReason,
            Features = new
            {
                UnlimitedActivities = IsActive(),
                CrownChallenges = AreCrownChallengesAvailable(),
                DailyActivityLimit = GetDailyActivityLimit()
            }
        };
    }

    /// <summary>
    /// Determines if the subscription is approaching expiry and needs renewal reminder.
    /// </summary>
    /// <param name="reminderDays">Number of days before expiry to trigger reminder.</param>
    /// <returns>True if renewal reminder should be shown.</returns>
    public bool NeedsRenewalReminder(int reminderDays = 3)
    {
        if (Status != "Active" || !AutoRenewalEnabled) return false;

        var daysUntilExpiry = (EndDate - DateTime.UtcNow).Days;
        return daysUntilExpiry <= reminderDays && daysUntilExpiry > 0;
    }

    /// <summary>
    /// Calculates the total revenue generated by this subscription.
    /// </summary>
    /// <returns>Total revenue in cents.</returns>
    public int GetTotalRevenue()
    {
        // This would typically be calculated from payment history
        // For now, return the price if subscription is active/renewed
        if (LastPaymentDate.HasValue && PriceCents > 0)
        {
            var monthsActive = Status == "Active" ?
                Math.Max(1, (DateTime.UtcNow - StartDate).Days / 30) : 1;
            return BillingCycle == "Annual" ? PriceCents : PriceCents * monthsActive;
        }

        return 0;
    }
}